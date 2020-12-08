namespace TFAGoogleDrive {
    partial class MainForm {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnAuthorize = new System.Windows.Forms.Button();
            this.listFiles = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmNewFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUpload = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.imgLargeList = new System.Windows.Forms.ImageList(this.components);
            this.imgSmallList = new System.Windows.Forms.ImageList(this.components);
            this.btnPrev = new System.Windows.Forms.Button();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.lblSelectedCount = new System.Windows.Forms.Label();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // btnAuthorize
            this.btnAuthorize.Location = new System.Drawing.Point(485, 39);
            this.btnAuthorize.Margin = new System.Windows.Forms.Padding(4);
            this.btnAuthorize.Name = "btnAuthorize";
            this.btnAuthorize.Size = new System.Drawing.Size(129, 28);
            this.btnAuthorize.TabIndex = 0;
            this.btnAuthorize.Text = "Giriş Yap";
            this.btnAuthorize.UseVisualStyleBackColor = true;
            this.btnAuthorize.Click += new System.EventHandler(this.btnAuthorize_Click);
            // listFiles
            this.listFiles.AllowDrop = true;
            this.listFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader2,
            this.columnHeader1});
            this.listFiles.ContextMenuStrip = this.contextMenu;
            this.listFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.listFiles.FullRowSelect = true;
            this.listFiles.GridLines = true;
            this.listFiles.HideSelection = false;
            this.listFiles.LabelEdit = true;
            this.listFiles.LargeImageList = this.imgLargeList;
            this.listFiles.Location = new System.Drawing.Point(40, 75);
            this.listFiles.Margin = new System.Windows.Forms.Padding(4);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(615, 288);
            this.listFiles.SmallImageList = this.imgSmallList;
            this.listFiles.TabIndex = 1;
            this.listFiles.UseCompatibleStateImageBehavior = false;
            this.listFiles.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listFiles_AfterLabelEdit);
            this.listFiles.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listFiles_ItemSelectionChanged);
            this.listFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listFiles_MouseDoubleClick);
           
            this.columnHeader1.Text = "File ID";
            this.columnHeader1.Width = 160;
            // contextMenu
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmNewFolder,
            this.tsmUpload,
            this.tsmDelete});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(159, 76);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // tsmNewFolder
            this.tsmNewFolder.Name = "tsmNewFolder";
            this.tsmNewFolder.Size = new System.Drawing.Size(158, 24);
            this.tsmNewFolder.Text = "Yeni Klasör";
            this.tsmNewFolder.Click += new System.EventHandler(this.tsmNewFolder_Click);
            // tsmUpload
            this.tsmUpload.Name = "tsmUpload";
            this.tsmUpload.Size = new System.Drawing.Size(158, 24);
            this.tsmUpload.Text = "Dosya Yükle";
            this.tsmUpload.Click += new System.EventHandler(this.tsmUpload_Click);
            // tsmDelete
            this.tsmDelete.Name = "tsmDelete";
            this.tsmDelete.Size = new System.Drawing.Size(158, 24);
            this.tsmDelete.Text = "Sil";
            this.tsmDelete.Click += new System.EventHandler(this.tsmDelete_Click);
            // imgLargeList
            this.imgLargeList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLargeList.ImageStream")));
            this.imgLargeList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgLargeList.Images.SetKeyName(0, "folder.png");
            this.imgLargeList.Images.SetKeyName(1, "empty-file.png");
            // imgSmallList
            this.imgSmallList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgSmallList.ImageStream")));
            this.imgSmallList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgSmallList.Images.SetKeyName(0, "folder-16.png");
            this.imgSmallList.Images.SetKeyName(1, "empty-file-16.png");
            // btnPrev
            this.btnPrev.Location = new System.Drawing.Point(35, 42);
            this.btnPrev.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(130, 25);
            this.btnPrev.TabIndex = 19;
            this.btnPrev.Text = "Üst Dizine Çık";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // lblFileCount
            this.lblFileCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.Location = new System.Drawing.Point(32, 291);
            this.lblFileCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(0, 17);
            this.lblFileCount.TabIndex = 26;
            // lblSelectedCount
            this.lblSelectedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectedCount.AutoSize = true;
            this.lblSelectedCount.Location = new System.Drawing.Point(1220, 291);
            this.lblSelectedCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSelectedCount.Name = "lblSelectedCount";
            this.lblSelectedCount.Size = new System.Drawing.Size(0, 17);
            this.lblSelectedCount.TabIndex = 27;
            // MainForm
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 376);
            this.Controls.Add(this.lblSelectedCount);
            this.Controls.Add(this.lblFileCount);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.listFiles);
            this.Controls.Add(this.btnAuthorize);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TFA Google Drive";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private System.Windows.Forms.Button btnAuthorize;
        private System.Windows.Forms.ListView listFiles;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.ImageList imgLargeList;
        private System.Windows.Forms.ImageList imgSmallList;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmNewFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmUpload;
        private System.Windows.Forms.ToolStripMenuItem tsmDelete;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.Label lblSelectedCount;
    }
}

