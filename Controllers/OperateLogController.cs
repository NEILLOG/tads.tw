using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Models.Enums;
using TADS_Web.Models.Extend;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class OperateLogController : BaseController
    {
        private readonly OperateLogService _operateLogService;

        public OperateLogController(OperateLogService operateLogService)
        {
            _operateLogService = operateLogService;
        }

        private UserSessionModel? GetUserInfo()
        {
            return HttpContext.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo);
        }

        public async Task<IActionResult> List(VM_OperateLog data)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Login", "Sdatback");

            ViewData["Title"] = "操作紀錄查詢";

            // 功能下拉
            foreach (var f in _operateLogService.GetDistinctFeatures())
                data.ddlFeature.Add(new SelectListItem() { Text = f, Value = f });

            try
            {
                IQueryable<TbBackendOperateLog>? query = _operateLogService.GetLogList(ref _message);

                if (query != null && !string.IsNullOrWhiteSpace(data.Search.sAccount))
                {
                    List<string> userIds = _operateLogService.GetUserIdsByKeyword(data.Search.sAccount.Trim());
                    query = query.Where(x => userIds.Contains(x.UserId));
                }

                if (query != null && !string.IsNullOrWhiteSpace(data.Search.sFeature))
                    query = query.Where(x => x.Feature == data.Search.sFeature);

                if (query != null && !string.IsNullOrWhiteSpace(data.Search.sAction))
                    query = query.Where(x => x.Action.Contains(data.Search.sAction.Trim()));

                if (query != null && data.Search.sResult == "success")
                    query = query.Where(x => x.IsSuccess);
                if (query != null && data.Search.sResult == "fail")
                    query = query.Where(x => !x.IsSuccess);

                if (query != null && !string.IsNullOrEmpty(data.Search.sTime))
                {
                    DateTime s = Convert.ToDateTime(data.Search.sTime);
                    query = query.Where(x => x.CreateDate >= s);
                }
                if (query != null && !string.IsNullOrEmpty(data.Search.eTime))
                {
                    DateTime e = Convert.ToDateTime(data.Search.eTime).AddDays(1);
                    query = query.Where(x => x.CreateDate < e);
                }

                if (query != null)
                {
                    List<TbBackendOperateLog>? paged = await PagerInfoService.GetRange(
                        query.OrderByDescending(x => x.CreateDate), data.Search.PagerInfo);

                    Dictionary<string, TbUser> userMap = _operateLogService.GetUserMap();
                    data.LogList = (paged ?? new List<TbBackendOperateLog>()).Select(x =>
                    {
                        userMap.TryGetValue(x.UserId, out TbUser? u);
                        return new OperateLogExtend
                        {
                            Log = x,
                            Account = u?.Account,
                            UserName = u?.UserName
                        };
                    }).ToList();
                }
            }
            catch
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = "查詢操作紀錄失敗，請稍後再試";
                data.LogList = new List<OperateLogExtend>();
            }

            return View(data);
        }

        public async Task<IActionResult> Detail(long id)
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Login", "Sdatback");

            ViewData["Title"] = "操作紀錄明細";

            TbBackendOperateLog? log = await _operateLogService.GetLog(id);
            if (log == null)
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = "查無此筆紀錄";
                return RedirectToAction("List");
            }

            Dictionary<string, TbUser> userMap = _operateLogService.GetUserMap();
            userMap.TryGetValue(log.UserId, out TbUser? u);

            VM_OperateLog data = new VM_OperateLog
            {
                LogItem = new OperateLogExtend
                {
                    Log = log,
                    Account = u?.Account,
                    UserName = u?.UserName
                }
            };

            return View(data);
        }
    }
}
