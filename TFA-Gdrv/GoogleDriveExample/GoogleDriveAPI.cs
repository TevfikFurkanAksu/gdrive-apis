using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFAGoogleDrive {
    public class GoogleDriveAPI {

        private static GoogleDriveAPI Instance = null;
      
        public static GoogleDriveAPI GetInstance() {
            if (Instance == null) {
                Instance = new GoogleDriveAPI();
            }
            return Instance;
        }

        private DriveService service;
        public bool Running = false;
        public event Action<int, string> SetProgressValue = delegate { };
        private readonly string[] Scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile, DriveService.Scope.DriveReadonly };

        /// Authorization işlemi yapılan yer, 
        /// client_id.json dosyasını bin/debug içine attım
        public void Authorize() {
            UserCredential credential;
            if (!System.IO.File.Exists(Application.StartupPath + "/client_id.json")) {
                throw new Exception("client_id.json dosyası bulunamadı");
            }

            using (var stream = new FileStream(Application.StartupPath + "/client_id.json", FileMode.Open, FileAccess.Read)) {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    Environment.UserName,
                    CancellationToken.None,
                    new FileDataStore(Application.StartupPath + "/google-tokens", true)
                ).Result;
            }

            service = new DriveService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveExample",

            });

            service.HttpClient.Timeout = TimeSpan.FromMinutes(100);
        }

        /// dosya yüklerken dosyanın türünün belirlendiği yerdir. Drive dosyaların mimetype bilgisini istiyor çünkü
        private string GetMimeType(string file) {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(file).ToLower();

            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

            if (regKey != null && regKey.GetValue("Content Type") != null) {
                mimeType = regKey.GetValue("Content Type").ToString();
            }

            return mimeType;
        }

        /// Drive'ın root klasörünün id bilgisinin verildiği bölüm
        public string GetRootID() {
            Google.Apis.Drive.v3.Data.File file = service.Files.Get("root").Execute();
            return file.Id;
        }

        /// İlgili hesabın drive'ından dosyaları çeken kısım 
        public List<Google.Apis.Drive.v3.Data.File> GetFiles(string query = null) {
            List<Google.Apis.Drive.v3.Data.File> fileList = new List<Google.Apis.Drive.v3.Data.File>();
            FilesResource.ListRequest request = service.Files.List();
            request.PageSize = 1;
            request.Q = query ?? "mimeType != \"application/vnd.google-apps.folder\"";

            // hangi alanların gelmesini istiyorsak burada belirtiyoruz
            request.Fields = "nextPageToken, files(id, name, createdTime, modifiedTime, mimeType, description, size)";

            //dosyalar parça parça geliyor, her parçada nextPageToken dönüyor, nextPageToken null gelene kadar bu döngü devam eder.
            // null dönerse tüm dosyalar çekilmiştir
            do {
                FileList files = request.Execute();

                // her partta gelen dosyaları fileList listesine ekliyoruz
                fileList.AddRange(files.Files);
                request.PageToken = files.NextPageToken;

            } while (!string.IsNullOrEmpty(request.PageToken));

            return fileList;
        }

        /// drive'dan dosya çekme
        public async IAsyncEnumerable<Google.Apis.Drive.v3.Data.File> GetFilesAsync(string query = null) {
            Running = true;
            FilesResource.ListRequest request = service.Files.List();
            request.PageSize = 200;
            request.Q = query ?? "mimeType != \"application/vnd.google-apps.folder\"";

            // hangi alanların gelmesini istiyorsak burada belirtiyoruz
            request.Fields = "nextPageToken, files(id, name, createdTime, modifiedTime, mimeType, description, size)";

            //dosyalar parça parça geliyor, her parçada nextPageToken dönüyor, nextPageToken null gelene kadar bu döngü devam eder.
            // null dönerse tüm dosyalar çekilmiştir
            do {
                FileList files = await request.ExecuteAsync();
                foreach (var item in files.Files) {
                    yield return item;
                }

                request.PageToken = files.NextPageToken;


            } while (!string.IsNullOrEmpty(request.PageToken));

            Running = false;

            yield break;
        }

        /// klasör oluşturur eğer belirtilen isimde klasör varsa oluşturmaz
        public string CreateFolderAndGetID(string folderName, string parentId = null) {
            string query = $"mimeType = \"application/vnd.google-apps.folder\" and name = \"{folderName}\"";
            List<Google.Apis.Drive.v3.Data.File> result = GetFiles(query);
            Google.Apis.Drive.v3.Data.File file = result.FirstOrDefault();

            if (file != null) {
                return file.Id;
            }
            else {
                file = new Google.Apis.Drive.v3.Data.File {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder"
                };

                if (parentId != null) {
                    file.Parents = new List<string> { parentId };
                }

                var request = service.Files.Create(file);
                request.Fields = "id";
                var response = request.Execute();
                return response.Id;
            }
        }

        /// Yeni bir klasör oluşturulan bölümdür
        public Google.Apis.Drive.v3.Data.File CreateFolder(string folderName, string parentId = null) {
            Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };
            if (parentId != null) {
                file.Parents = new List<string> { parentId };
            }
            var request = service.Files.Create(file);
            request.Fields = "id, name, createdTime, modifiedTime, mimeType, description, size";
            var response = request.Execute();
            return response;
        }

        /// dosya yüklem işleminin yapıldığı kısım
        public async Task<Google.Apis.Drive.v3.Data.File> UploadFile(string file, string parentId = null) {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(file);
            if (System.IO.File.Exists(file)) {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File {
                    Name = System.IO.Path.GetFileName(file),
                    Description = "",
                    AppProperties = new Dictionary<string, string> { { "customKey", "customValue" } },
                    MimeType = GetMimeType(file)
                };

                if (!string.IsNullOrEmpty(parentId)) {
                    body.Parents = new List<string> { parentId };
                }
                else {
                    string folderId = CreateFolderAndGetID("DriveApiExample");
                    body.Parents = new List<string> { folderId };
                }

                using (var stream = new System.IO.FileStream(file, FileMode.Open, FileAccess.Read)) {
                    try {
                        FilesResource.CreateMediaUpload request = service.Files.Create(body, stream, GetMimeType(file));
                        request.SupportsTeamDrives = true;
                        request.Fields = "id, name, createdTime, modifiedTime, mimeType, description, size";
                        request.ProgressChanged += (e) => {
                            if (e.BytesSent > 0) {
                                int progress = (int)Math.Floor((decimal)((e.BytesSent * 100) / fileInfo.Length));
                                SetProgressValue(progress, "yükleniyor...");
                            }
                        };
                        request.ResponseReceived += (e) => {
                            SetProgressValue(100, "yüklendi");
                        };

                        SetProgressValue(0, "yükleniyor...");
                        await request.UploadAsync();
                        return request.ResponseBody;
                    }
                    catch (Exception ex) {
                        throw new Exception(ex.Message);
                    }
                }
            }
            else {
                throw new Exception("Can not found");
            }
        }
        /// dosya silme işlemi
        public async Task<string> DeleteFile(string fileId) {
            return await service.Files.Delete(fileId).ExecuteAsync();
        }
        /// dosyayı tekrar isimlendiren ksıım
        public void Rename(string fileId, string name) {
            Google.Apis.Drive.v3.Data.File file = service.Files.Get(fileId).Execute();
            file.Id = null;
            file.Name = name;
            service.Files.Update(file, fileId).Execute();
        }
    }
}
