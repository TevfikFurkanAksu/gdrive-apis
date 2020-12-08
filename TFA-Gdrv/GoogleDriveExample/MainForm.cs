using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFAGoogleDrive {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private string ActiveParentID;
        private string ActiveFolderName;
        private readonly List<string> Navigation = new List<string>();
        private GoogleDriveAPI DriveApi;
        private readonly List<Tuple<string, string>> SelectedFiles = new List<Tuple<string, string>>();
        private string LastQuery;

        /// drive a login olur ve roottaki dosyaları çeker
        private void btnAuthorize_Click(object sender, EventArgs e) {
            DriveApi = GoogleDriveAPI.GetInstance();

            try {
                DriveApi.Authorize();
                btnAuthorize.Enabled = false;
                btnAuthorize.Text = "Authorized";
                GetRootFiles();
                btnPrev.Enabled = false;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshList() {
            GetList(LastQuery);
        }

        /// Root'taki dosyaları listeler
        private void GetRootFiles() {
            string rootId = DriveApi.GetRootID();
            ActiveParentID = rootId;
            Navigation.Add(rootId);

            string query = $"('{rootId}' in parents)";
            GetList(query);
        }

        /// verilen query'e göre dosya listeler
        private async void GetList(string query) {
            LastQuery = query;
            listFiles.Items.Clear();
            lblSelectedCount.Text = "";
            
            await foreach (var item in DriveApi.GetFilesAsync(query)) {
                if (Navigation.Count > 1) {
                    btnPrev.Enabled = !DriveApi.Running;
                }
                long fileSize = item.Size ?? 0;
                string[] row = { item.Name, (fileSize / 1024f).ToString("n0") + " KB", fileSize.ToString(), item.MimeType, item.CreatedTime.Value.ToString("G"), item.Id };
                ListViewItem lvi = new ListViewItem(row);

                if (item.MimeType.Contains("folder")) {
                    lvi.ImageIndex = 0;
                }
                else {
                    lvi.ImageIndex = 1;
                }

                listFiles.Items.Add(lvi);
            }
            if (Navigation.Count > 1) {
                btnPrev.Enabled = !DriveApi.Running;
            }
        }

        /// Listview e dosyaları doldurur
        private void FillListView(List<Google.Apis.Drive.v3.Data.File> files) {
            listFiles.Items.Clear();

            int i, n = files.Count;
            for (i = 0; i < n; i++) {
                long fileSize = files[i].Size ?? 0;
                string[] row = { files[i].Name, (fileSize / 1024f).ToString("n0") + " KB", fileSize.ToString(), files[i].MimeType, files[i].CreatedTime.Value.ToString("G"), files[i].Id };

                ListViewItem lvi = new ListViewItem(row);
                if (files[i].MimeType.Contains("folder")) {
                    lvi.ImageIndex = 0;
                }
                else {
                    lvi.ImageIndex = 1;
                }
                listFiles.Items.Add(lvi);
                lblFileCount.Text = "File count: " + listFiles.Items.Count.ToString();
            }
        }

        private void MainForm_Load(object sender, EventArgs e) {
            btnAuthorize.PerformClick();
        }

        /// bir önceki klasöre dönmek için button
        private void btnPrev_Click(object sender, EventArgs e) {
            // aktif klasörü navigasyondan siler
            Navigation.Remove(ActiveParentID);

            // aktif klasör silindiği için aktif klasörün üstündeki klasöre last ile ulaşabiliriz
            string query = $"('{Navigation.Last()}' in parents)";

            // yeni aktif klasörümüz geri döndüğümüz klasör olur
            ActiveParentID = Navigation.Last();
            GetList(query);

            if (Navigation.Count < 2) {
                btnPrev.Enabled = false;
            }
        }

        // klasörün içini görmek için çift tıklama
        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listFiles.SelectedItems.Count == 0) {
                return;
            }

            ListViewItem lvi = listFiles.SelectedItems[0];
            string mimeType = lvi.SubItems[3].Text;

            // tıklanan item klasör değilse bir dosya ise işlem yapmamasını sağlıyor 
            if (!mimeType.Contains("folder")) {
                return;
            }

            string folderId = lvi.SubItems[5].Text;
            string folderName = lvi.SubItems[0].Text;
            string query = $"('{folderId}' in parents)";
            GetList(query);

            // açılan klasörü navigasyona ekler ve aktif klasör olarak belirtir
            Navigation.Add(folderId);
            ActiveParentID = folderId;
            ActiveFolderName = $"{folderName}";

            if (Navigation.Count > 1) {
                btnPrev.Enabled = true;
            }

        }

        /// dosya/klasör yeniden isimlendirmek için veya yeni klasör oluşturmak için
        private void listFiles_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            ListViewItem lvi = listFiles.Items[e.Item];
            string fileId = lvi.SubItems[5].Text;

            // eğer fileId null ise bu yeni oluşacak bir klasördür, ama ismi verilmemişse işlemi geri alır
            if (string.IsNullOrEmpty(fileId) && string.IsNullOrEmpty(e.Label)) {
                lvi.Selected = false;
                lvi.Focused = false;
                listFiles.Items.Remove(lvi);
            }
            
            string newName = e.Label;
            if (string.IsNullOrEmpty(newName)) {
                return;
            }

            string oldName = lvi.SubItems[0].Text;
            // eğer fileId null ise yeni klasör oluşacaktır, e.label girilmişse klasöre isim verilmiş demektir
            if (string.IsNullOrEmpty(fileId)) {
                // klasörü oluşturur ve aktif seçili olan item'ı günceller, ardından unselect ve unfocus olur
                Google.Apis.Drive.v3.Data.File file = DriveApi.CreateFolder(newName, ActiveParentID);
                long fileSize = file.Size ?? 0;
                lvi.SubItems[1].Text = (fileSize / 1024f).ToString("n0") + " KB";
                lvi.SubItems[2].Text = fileSize.ToString();
                lvi.SubItems[3].Text = file.MimeType;
                lvi.SubItems[4].Text = file.CreatedTime?.ToString("G");
                lvi.SubItems[5].Text = file.Id;
                lvi.Selected = false;
                lvi.Focused = false;
                return;
            }

            // yeni isim ile eski isim aynı ise işlem yapmaya gerek yok
            if (oldName == newName) {
                return;
            }

            // dosya veya kalsaör ismini değişen kısım
            DriveApi.Rename(fileId, newName);

        }

        // Sağ click'e basınca menü çıkar ve duruma göre yapılabilecek işlemler gözükür
        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            // seçilen bir klasör veya dosya değil ise sil butonu notvisible olur. Yeni dosya ve dosya yükle visible olur
            if (listFiles.SelectedItems.Count == 0) {
                tsmDelete.Visible = false;
                tsmNewFolder.Visible = true;
                tsmUpload.Visible = true;
            }
            else {
                // Bir dosya veya kalsör seçilmişse silme işlemi visible diğer işlemler notvisible olur 
                tsmNewFolder.Visible = false;
                tsmUpload.Visible = false;             
                tsmDelete.Visible = true;
            }
        }

        /// Yeni klasör oluşturan kısım
        private void tsmNewFolder_Click(object sender, EventArgs e) {
            string[] row = { "Yeni Klasör", 0.ToString("n0") + " KB", 0.ToString(), "", "", "" };
            ListViewItem lvi = new ListViewItem(row) {
                ImageIndex = 0
            };
            listFiles.Items.Add(lvi);
            lvi.Selected = true;
            lvi.Focused = true;
            lvi.BeginEdit();
        }

        /// yeni dosya yüklemek için openfileDialog ile dosya seçilen kısım
        private async void tsmUpload_Click(object sender, EventArgs e) {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() != DialogResult.OK) {
                return;
            }
            string file = fileDialog.FileName;
            if (string.IsNullOrEmpty(file)) {
                MessageBox.Show("Bir dosya seçiniz");
                return;
            }
            if (!File.Exists(file)) {
                MessageBox.Show("Dosya bulunamadı");
                return;
            }
            await UploadFile(file);
        }

        /// dosya yükler 
        private async Task<bool> UploadFile(string file) {
            tsmUpload.Enabled = false;
            Google.Apis.Drive.v3.Data.File uploadedFile = await DriveApi.UploadFile(file, ActiveParentID);

            long fileSize = uploadedFile.Size ?? 0;
            string[] row = { uploadedFile.Name, (fileSize / 1024f).ToString("n0") + " KB", fileSize.ToString(), uploadedFile.MimeType, uploadedFile.CreatedTime?.ToString("G"), uploadedFile.Id };
            ListViewItem lvi = new ListViewItem(row) {
                ImageIndex = 1
            };
            listFiles.Items.Add(lvi);
            tsmUpload.Enabled = true;
            return true;
        }

        private void RefreshSelectedFiles() {
            SelectedFiles.Clear();
            foreach (ListViewItem lvi in listFiles.SelectedItems) {
                string fileId = lvi.SubItems[5].Text;
                string fileName = lvi.SubItems[0].Text;
                SelectedFiles.Add(new Tuple<string, string>(fileId, fileName));
            }
        }

        /// seçilen dosyaları siler     
        private async void tsmDelete_Click(object sender, EventArgs e) {
            if (listFiles.SelectedItems.Count == 0) {
                return;
            }
            if (MessageBox.Show("Silmek istiyor musun?", "Dikkat", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                return;
            }
            
            foreach (ListViewItem lvi in listFiles.SelectedItems) {
                string fileId = lvi.SubItems[5].Text;

                tsmDelete.Enabled = false;
                await DriveApi.DeleteFile(fileId);
                listFiles.Items.Remove(lvi);
            }

            lblSelectedCount.Text = "";
            tsmDelete.Enabled = true;
        }

        private void listFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            if (listFiles.SelectedItems.Count == 0) {
                lblSelectedCount.Text = "";
            }
            else {
                lblSelectedCount.Text = $"{listFiles.SelectedItems.Count} dosya seçildi";
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == (Keys.Delete)) {
                tsmDelete.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.A)) {
                listFiles.Items.Cast<ListViewItem>().ToList().ForEach(t => t.Selected = true);
                return true;
            }
            else if (keyData == Keys.Back) {
                btnPrev.PerformClick();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.N)) {
                tsmNewFolder.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
