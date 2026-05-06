using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text.Json;
using System.Text.Json.Serialization;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class AllCommonService : ServiceBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly DBContext _dbContext;
        private readonly IConfiguration _conf;
        private string _Message = string.Empty;

        public AllCommonService(DBContext context,
                                IHttpContextAccessor contextAccessor,
                                IConfiguration configuration) : base(context)
        {
            _contextAccessor = contextAccessor;
            _dbContext = context;
            _conf = configuration;
        }

        public async Task<string> IDGenerator<T>() where T : class
        {
            IDbContextTransaction transaction;
            string table_name = typeof(T).Name;
            bool has_transaction = _dbContext.Database.CurrentTransaction != null;

            if (!has_transaction)
                transaction = _dbContext.Database.BeginTransaction();
            else
                transaction = _dbContext.Database.CurrentTransaction!;

            await RowLock<TbIdSummary>(transaction, string.Format("WHERE TableName = '{0}'", table_name));

            TbIdSummary? max = await Lookup<TbIdSummary>(ref _Message, x => x.TableName == table_name).SingleOrDefaultAsync();

            if (max == null)
            {
                if (!has_transaction)
                    transaction.Commit();
                throw new Exception("請至 TbIdSummary 表中建立正確資料");
            }

            string max_id = max.MaxId++.ToString();
            ActionResultModel<TbIdSummary> result = await Update(max, transaction);

            if (!result.IsSuccess)
                throw new Exception(result.Message);

            _dbContext.Entry<TbIdSummary>(max).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            if (!has_transaction)
                transaction.Commit();

            return max.Prefix + max_id.PadLeft(max.Length - max.Prefix.Length, '0');
        }

        public async Task<bool> Error_Record<T>(string Platform, string Action, T? response) where T : class
        {
            TbLog log = new TbLog()
            {
                Ip = GetIPAddress_IPv4(),
                Action = Action,
                Date = DateTime.Now,
                Message = JsonSerializer.Serialize(response, new JsonSerializerOptions()
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }),
                Platform = Platform,
                Url = GetAbsoluteUrl(),
                UserAgent = GetUserAgent()
            };

            ActionResultModel<TbLog> result = await base.Insert(log);
            return result.IsSuccess;
        }

        public async Task<bool> LoginRecord(string Platform, string LoginMessage, string Account, string? UserID = null, bool IsSso = false, string? SsoResult = null)
        {
            TbLoginRecord log = new TbLoginRecord
            {
                Platform = Platform,
                UserId = UserID,
                Account = Account,
                Ip = GetIPAddress_IPv4(),
                LoginTime = DateTime.Now,
                LoginMsg = LoginMessage,
                UserAgent = GetUserAgent(),
                Sso = IsSso,
                Ssoresult = SsoResult == null ? null : JsonSerializer.Serialize(SsoResult)
            };
            ActionResultModel<TbLoginRecord> result = await base.Insert(log);
            return result.IsSuccess;
        }

        public string GetIPAddress_IPv4()
        {
            try { return _contextAccessor.HttpContext!.Connection.RemoteIpAddress!.ToString(); }
            catch { return string.Empty; }
        }

        public string GetAbsoluteUrl()
        {
            try
            {
                HttpRequest request = _contextAccessor.HttpContext!.Request;
                return new System.Text.StringBuilder()
                    .Append(request.Scheme).Append("://")
                    .Append(request.Host).Append(request.PathBase)
                    .Append(request.Path).Append(request.QueryString)
                    .ToString();
            }
            catch { return string.Empty; }
        }

        public string GetUserAgent()
        {
            try
            {
                int max_len = 500;
                string user_agent = _contextAccessor.HttpContext!.Request.Headers["User-Agent"].ToString();
                if (user_agent.Length > max_len)
                    user_agent = user_agent.AsSpan(0, max_len).ToString();
                return user_agent;
            }
            catch { return string.Empty; }
        }
    }
}
