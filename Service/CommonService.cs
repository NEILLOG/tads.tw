using System.Text.Json;
using System.Text.Json.Serialization;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class CommonService : ServiceBase
    {
        private readonly AllCommonService _allCommonService;
        private readonly IConfiguration _conf;

        public CommonService(DBContext context,
                             AllCommonService allCommonService,
                             IConfiguration configuration) : base(context)
        {
            _allCommonService = allCommonService;
            _conf = configuration;
        }

        public async Task<bool> OperateLog<TRequest>(string UserID, string Feature, string Action, string? id = null, TRequest? T_request = null, string? Message = null, string? response = null, bool IsSuccess = true) where TRequest : class
        {
            ActionResultModel<TbBackendOperateLog> result = new ActionResultModel<TbBackendOperateLog>();

            TbBackendOperateLog log = new TbBackendOperateLog()
            {
                Request = JsonSerializer.Serialize(T_request, new JsonSerializerOptions()
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }),
                Response = response,
                Ip = _allCommonService.GetIPAddress_IPv4(),
                Action = Action,
                Message = Message,
                Url = _allCommonService.GetAbsoluteUrl(),
                UserAgent = _allCommonService.GetUserAgent(),
                CreateDate = DateTime.Now,
                Feature = Feature,
                UserId = UserID,
                IsSuccess = IsSuccess,
                DataKey = id,
            };

            if (_conf.GetValue<bool>("Site:BackendOperateLog"))
                result = await base.Insert(log);

            return result.IsSuccess;
        }

        public async Task<bool> OperateLog(string UserID, string Feature, string Action, string? id = null, string? request = null, string? Message = null, string? response = null, bool IsSuccess = true)
        {
            ActionResultModel<TbBackendOperateLog> result = new ActionResultModel<TbBackendOperateLog>();

            TbBackendOperateLog log = new TbBackendOperateLog()
            {
                Request = request,
                Response = response,
                Ip = _allCommonService.GetIPAddress_IPv4(),
                Action = Action,
                Message = Message,
                Url = _allCommonService.GetAbsoluteUrl(),
                UserAgent = _allCommonService.GetUserAgent(),
                CreateDate = DateTime.Now,
                Feature = Feature,
                UserId = UserID,
                IsSuccess = IsSuccess,
                DataKey = id,
            };

            if (_conf.GetValue<bool>("Site:BackendOperateLog"))
                result = await base.Insert(log);

            return result.IsSuccess;
        }
    }
}
