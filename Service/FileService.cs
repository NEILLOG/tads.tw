using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Service.Base;
using Microsoft.EntityFrameworkCore.Storage;

namespace TADS_Web.Service
{
    public class FileService : ServiceBase
    {
        private string _message = string.Empty;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private AllCommonService _allCommonService;

        public FileService(DBContext context,
                           IHttpContextAccessor httpContextAccessor,
                           IConfiguration configuration,
                           IWebHostEnvironment webHostEnvironment,
                           AllCommonService allCommonService) : base(context)
        {
            _contextAccessor = httpContextAccessor;
            _config = configuration;
            _webHostEnvironment = webHostEnvironment;
            _allCommonService = allCommonService;
        }

        public async Task<FileResultModel> FileUploadAsync(IFormFile item, string fd, string? FileDescription = null, string? OldFileID = null, string? filename = null, IDbContextTransaction? transaction = null)
        {
            UserSessionModel? userinfo = _contextAccessor.HttpContext!.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo);
            DateTime now = DateTime.Now;
            FileResultModel result = new FileResultModel();

            try
            {
                var fileRealName = this.GenerateFileID();
                fd = fd.Replace("..", "").Replace("'", "").Replace("/", "").Replace(@"\", "");

                string fileUploadRoot = _config["Site:FileUploadRoot"] ?? "upload";
                string path = MapPath(fileUploadRoot, fd);
                CreateDirectory(path);

                string _filted_fileName = item.FileName.Replace("..", "").Replace("'", "").Replace("/", "").Replace(@"\", "");
                string extension = Path.GetExtension(_filted_fileName).Replace("..", "").Replace("'", "").Replace("/", "").Replace(@"\", "");
                string save = MapPath(fileUploadRoot, fd, fileRealName + extension);

                using (FileStream stream = new FileStream(save, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                await _lock.WaitAsync();

                string ralativepath = String.Join('/', fileUploadRoot, fd, fileRealName + extension);

                TbFileInfo Entity = new TbFileInfo();
                Entity.FileId = await _allCommonService.IDGenerator<TbFileInfo>();
                Entity.FileName = String.IsNullOrEmpty(filename) ? _filted_fileName : filename;
                Entity.FileRealName = fileRealName;
                Entity.FileDescription = string.IsNullOrEmpty(FileDescription) ? string.Empty : FileDescription;
                Entity.FilePath = ralativepath;
                Entity.Order = 1;
                Entity.IsDelete = false;
                Entity.CreateUser = userinfo == null ? "" : userinfo.UserID;
                Entity.CreateDate = now;
                var InsertResult = await Insert<TbFileInfo>(Entity, transaction);

                _lock.Release();

                result.FilePath = Entity.FilePath;
                result.IsSuccess = InsertResult.IsSuccess;
                result.Message = InsertResult.Message;
                if (InsertResult.IsSuccess)
                {
                    result.FileID = Entity.FileId;
                    await FileDelete(OldFileID);
                }
            }
            catch (Exception ex)
            {
                if (transaction != null) throw;
                else
                {
                    result.IsSuccess = false;
                    result.Message = ex.ToString();
                    result.FileID = String.Empty;
                }
            }

            return result;
        }

        public async Task<FileResultModel> FileDelete(string? FileID, char SplitChar = ',')
        {
            FileResultModel result = new FileResultModel();
            try
            {
                if (!string.IsNullOrEmpty(FileID))
                {
                    List<string> FileIDList = FileID.Split(SplitChar).ToList();
                    result = await FileDelete(FileIDList);
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message += ex.ToString();
            }
            return result;
        }

        public async Task<FileResultModel> FileDelete(List<string> FileIDList)
        {
            UserSessionModel? userinfo = _contextAccessor.HttpContext!.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo);
            _message = string.Empty;
            DateTime now = DateTime.Now;
            FileResultModel result = new FileResultModel();

            try
            {
                if (FileIDList != null && FileIDList.Count > 0)
                {
                    List<TbFileInfo> FileInfoList = Lookup<TbFileInfo>(ref _message, x => FileIDList.Contains(x.FileId))!.ToList();
                    if (FileInfoList != null && FileInfoList.Count > 0)
                    {
                        foreach (var file in FileInfoList)
                        {
                            file.IsDelete = true;
                            file.ModifyUser = userinfo?.UserID;
                            file.ModifyDate = now;

                            if (_config.GetValue<bool>("Site:FileRealDelete"))
                            {
                                string path = MapPath(file.FilePath);
                                if (File.Exists(path))
                                    File.Delete(path);
                            }
                        }

                        var update = await UpdateRange<TbFileInfo>(FileInfoList);
                        if (!update.IsSuccess)
                        {
                            result.Message = update.Message;
                            result.IsSuccess = update.IsSuccess;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.ToString();
            }

            return result;
        }

        public string MapPath(params string[] paths)
        {
            char separator = '/';
            List<string> path_list = new List<string> { _webHostEnvironment.WebRootPath };
            foreach (var path in paths)
            {
                var path_array = path.Split(separator).Where(x => x != string.Empty && x != "~").ToList();
                path_list = path_list.Concat(path_array).ToList();
            }
            return Path.Combine(path_list.ToArray());
        }

        public string GenerateFileID()
        {
            string sCodeList = "123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
            string sCode = "";
            Random rand = new Random();
            string Prefix = DateTime.Now.ToString("yyMMddhhmmssffffff");
            string NextID = string.Empty;
            bool isExist = true;

            while (isExist)
            {
                sCode = "";
                for (Int16 i = 0; i < 5; i++)
                    sCode += sCodeList[rand.Next(sCodeList.Length)];
                NextID = Prefix + sCode;
                isExist = this.isIDExist(NextID);
            }
            return NextID;
        }

        private bool isIDExist(string FileRealName)
        {
            string errormsg = "";
            return Lookup<TbFileInfo>(ref errormsg, x => x.FileRealName == FileRealName)?.Any() ?? false;
        }

        private void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public TbFileInfo? GetFileInfo(string FileId)
        {
            return Lookup<TbFileInfo>(ref _message, x => x.FileId == FileId && !x.IsDelete)?.SingleOrDefault();
        }
    }
}
