using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using GFile = Google.Apis.Drive.v3.Data.File;
using GoogleException = Google.GoogleApiException;

using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading.Tasks;

namespace DriveNET
{
    public sealed class NetGDrive
    {
        public DriveService mService;

        private bool mLogEnabled = false;
        private Logger mLogger;

        public NetGDrive(string ApplicationName = "Drive.Net", bool LoggingEnabled = false)
        {
            mLogEnabled = LoggingEnabled;
            if (mLogEnabled)
                mLogger = new Logger();

            Init(ApplicationName);
        }
        public NetGDrive(string clientId, string clientSecret, string ApplicationName = "Drive.Net", bool LoggingEnabled = false)
        {
            mLogEnabled = LoggingEnabled;
            if (mLogEnabled)
                mLogger = new Logger();

            Init(ApplicationName, clientId, clientSecret);
        }


        /// <summary>
        /// Intialize Google Drive service
        /// https://console.developers.google.com
        /// </summary>
        /// <param name="AppName"></param>
        private void Init(string AppName)
        {
            //Set scopes to read and write files
            string[] scopes = new string[]
            {
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile
            };

            // Request a new token or use the existing from User/AppData folder
            try
            {


                FileStream fs = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                var ClientToken = GoogleClientSecrets.Load(fs).Secrets;


                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets()
                {
                    ClientId = ClientToken.ClientId,
                    ClientSecret = ClientToken.ClientSecret
                },
                scopes,
                Environment.UserName,
                CancellationToken.None,
                new FileDataStore("Daimto.GoogleDrive.Auth.Store")).Result;

                // Create Drive API service.
                mService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = AppName,
                });

            }
            catch (FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("client_secret.json file not found.\nUse constructor with key parameters or load a valid client_secret.json file");
            }

        }

        /// <summary>
        /// Intialize Google Drive service
        /// https://console.developers.google.com
        /// </summary>
        /// <param name="AppName"></param>
        /// <param name="clientId">Required Google Drive Client Id</param>
        /// <param name="clientSecret">Required Google Drive Client Secret</param>
        private void Init(string AppName, string clientId, string clientSecret)
        {
            //Set scopes to read and write files
            string[] scopes = new string[]
            {
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile
            };

            // Request a new token or use the existing from User/AppData folder
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets()
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            scopes,
            Environment.UserName,
            CancellationToken.None,
            new FileDataStore("Daimto.GoogleDrive.Auth.Store")).Result;

            // Create Drive API service.
            mService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });

            AuthorizationTest();

        }

        private void AuthorizationTest()
        {
            try
            {
                var t = GetFileById("test");
            }
            catch (TokenResponseException exception)
            {
                Console.WriteLine("Authorization failed - Use 'client_secret.json' file or use valid token keys");
                LogError(exception);
            }
        }

        /// <summary>
        /// Get a single file by its id value
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<GFile> GetFileById(string Id)
        {
            try
            {
                GFile file = await mService.Files.Get(Id).ExecuteAsync();
                return file;
            }
            catch (GoogleException exception)
            {
                LogError(exception);
                return null;
            }
        }
        public async Task<GFile> GetFileByName(string Name)
        {
            string search = string.Format("name = '{0}'",Name);
            try
            {
                var response = await GetFiles(search, 1);
                return response[0];
            }
            catch (GoogleException exception)
            {
                LogError(exception);
                return null;
            }
        }

        public async Task<IList<Revision>> GetFileRevisions(string Id)
        {
            try
            {
                var revisionList = await mService.Revisions.List(Id).ExecuteAsync();
                return revisionList.Revisions;
            }
            catch (GoogleException exception)
            {
                LogError(exception);
                return null;
            }
        }

        /// <summary>
        /// List all of the files and directories for the current user. 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="resultSize"></param>
        /// <returns></returns>
        public async Task<IList<GFile>> GetFiles(string search, int resultSize = 1000)
        {
            Console.Write("Retrieving files... (Max : {0} files) - SEARCH =" + search,resultSize);

            IList<GFile> Files = new List<GFile>();
            try
            {
                var request = mService.Files.List();
                request.PageSize = resultSize;

                if (string.IsNullOrEmpty(search) == false)
                {
                    request.Q = search;
                }

                var task = await request.ExecuteAsync();
                Files = task.Files;
                //Files = request.Execute().Files;

                if (Files == null || Files.Count == 0)
                    //Console.WriteLine("\nNo files found");
                    return null;
                //else
                    //Console.WriteLine(" - {0} Files Found.", Files.Count);

            }
            catch (GoogleException exception)
            {
                LogError(exception);
            }

            return Files;
        }
        public async Task<IList<GFile>> GetFiles(NetGSearchBuilder builder, int resultSize = 1000)
        {
            var files = await this.GetFiles(builder.Search, resultSize);
            return files;
        }

        /// <summary>
        /// Download a file and get it's byte stream
        /// </summary>
        /// <param name="FileId"></param>
        /// <returns></returns>
        public async Task<byte[]> Download(string FileId)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                await mService.Files.Get(FileId).DownloadAsync(ms);

                byte[] data = ms.ToArray();
                return data;
            }
            catch (GoogleException exception)
            {
                LogError(exception);
                return null;
            }
        }
        /// <summary>
        /// Download a file with the specified revision
        /// </summary>
        /// <param name="FileId"></param>
        /// <param name="RevisionId"></param>
        /// <returns></returns>
        public async Task<byte[]> Download(string FileId, string RevisionId)
        {
            try
            {

                MemoryStream ms = new MemoryStream();

                await mService.Revisions.Get(FileId, RevisionId).DownloadAsync(ms);
                return ms.ToArray();
            }
            catch (GoogleException exception)
            {
                LogError(exception);
                return new byte[0];
            }
        }

        /// <summary>
        /// Upload a file with specified values
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Name"></param>
        /// <param name="parentId"></param>
        /// <param name="Description"></param>
        /// <param name="mimeType"></param>
        public async void Upload(byte[] data, string Name, string parentId = "", string mimeType = "")
        {

            if (data == null || data.Length == 0)
            {
                //Console.WriteLine("No data specified");
                return;
            }

            //Our new file
            GFile body = new GFile()
            {
                Name = Name,
                MimeType = mimeType,
            };

            if (!string.IsNullOrEmpty(parentId))
            {
                body.Parents = new List<string>();
                body.Parents.Add(parentId);
            }

            MemoryStream stream = new MemoryStream(data);

            try
            {
                //Upload our file
                FilesResource.CreateMediaUpload request = mService.Files.Create(body, stream, body.MimeType);
                await request.UploadAsync();
            }
            catch (GoogleException exception)
            {
                LogError(exception);
            }

        }

        /// <summary>
        /// Delete a file with specified id
        /// </summary>
        /// <param name="FileId"></param>
        public async void Thrash(string FileId)
        {
            try
            {
                GFile file = new GFile();
                file.Trashed = true;
                await mService.Files.Update(file, FileId).ExecuteAsync();
            }
            catch (GoogleException exception)
            {
                LogError(exception);
            }
        }

        /// <summary>
        /// Unthrashes a file with specified Id.
        /// </summary>
        /// <param name="FileId"></param>
        public async void Unthrash(string FileId)
        {
            try
            {
                GFile file = new GFile();
                file.Trashed = false;
                await mService.Files.Update(file, FileId).ExecuteAsync();
            }
            catch (GoogleException exception)
            {
                LogError(exception);
            }
        }

        /// <summary>
        /// Removes all files in the thrash PERMAMENTLY.
        /// </summary>
        public async void EmptyThrash()
        {
            try
            {
                await mService.Files.EmptyTrash().ExecuteAsync();
            }
            catch (GoogleException exception)
            {
                LogError(exception);
            }
        }

        /// <summary>
        /// Log an error
        /// </summary>
        /// <param name="error"></param>

        private void LogError(Exception exception)
        {
            if (mLogEnabled)
                mLogger.Log(exception);
        }

        public void Dispose()
        {
            mLogger.Dispose();
        }

    }

}
