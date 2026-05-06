using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Models.Enums;
using TADS_Web.Models.Extend;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class NewsController : BaseController
    {
        private readonly AllCommonService _allCommonService;
        private readonly CommonService _commonService;
        private readonly NewsService _newsService;
        private readonly FileService _fileService;

        public NewsController(AllCommonService allCommonService,
            FileService fileService,
            CommonService commonService,
            NewsService newsService)
        {
            _allCommonService = allCommonService;
            _fileService = fileService;
            _commonService = commonService;
            _newsService = newsService;
        }

        private UserSessionModel GetUserInfo()
        {
            return HttpContext.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo)
                ?? new UserSessionModel { UserID = "system", UserName = "System" };
        }

        public async Task<IActionResult> NewsList(VM_News data)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "最新消息管理", Action = "檢視";

            try
            {
                IQueryable<NewsExtend>? dataList = _newsService.GetNewsExtendList(ref _message);

                if (dataList != null && !string.IsNullOrEmpty(data.Search.sCategory))
                    dataList = dataList.Where(x => x.News.Category == data.Search.sCategory);

                if (dataList != null && !string.IsNullOrEmpty(data.Search.sTitle))
                    dataList = dataList.Where(x => x.News.Title.Contains(data.Search.sTitle));

                if (dataList != null && !string.IsNullOrEmpty(data.Search.sTime))
                    dataList = dataList.Where(x => x.News.DisplayDate >= Convert.ToDateTime(data.Search.sTime));

                if (dataList != null && !string.IsNullOrEmpty(data.Search.eTime))
                    dataList = dataList.Where(x => x.News.DisplayDate <= Convert.ToDateTime(data.Search.eTime));

                if (dataList != null)
                    data.NewsExtendList = await PagerInfoService.GetRange(dataList.OrderByDescending(x => x.News.IsKeepTop).ThenByDescending(x => x.News.DisplayDate), data.Search.PagerInfo);

                await _commonService.OperateLog(userinfo.UserID, Feature, Action, null, data);
            }
            catch (Exception ex)
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = "伺服器連線異常，請檢查您的網路狀態後再試一次！";

                string response = (TempData["TempMsg"] ?? "").ToString() + "\r\n" + (TempData["TempMsgDetail"] ?? "").ToString();
                await _commonService.OperateLog(userinfo.UserID, Feature, Action, null, response: response, IsSuccess: false);

                return RedirectToAction("NewsList");
            }

            return View(data);
        }

        public async Task<IActionResult> NewsAdd()
        {
            UserSessionModel userinfo = GetUserInfo();
            VM_News data = new VM_News();
            data.TopId = _newsService.GetTopId(ref _message);
            await _commonService.OperateLog(userinfo.UserID, "新增最新消息", "新增");
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewsAdd(VM_News datapost)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "最新消息新增", Action = "新增";

            bool isSuccess = false;
            bool unCaughtError = false;
            DateTime dtnow = DateTime.Now;
            TbNews? item = new TbNews();
            string? target_id = null;

            try
            {
                if (datapost.NewsExtendItem != null)
                {
                    item.Id = await _allCommonService.IDGenerator<TbNews>();
                    item.Title = datapost.NewsExtendItem.News.Title;
                    item.Category = datapost.NewsExtendItem.News.Category;
                    item.DisplayDate = datapost.NewsExtendItem.News.DisplayDate;
                    item.Contents = datapost.NewsExtendItem.News.Contents;
                    item.IsDelete = false;
                    item.IsPublish = datapost.isPublish == "上架";
                    item.IsKeepTop = datapost.NewsExtendItem.News.IsKeepTop;
                    item.CreateUser = userinfo.UserID;
                    item.CreateDate = dtnow;

                    using (var transaction = _newsService.GetTransaction())
                    {
                        try
                        {
                            if (datapost.NewsFile != null)
                            {
                                var photo_upload = await _fileService.FileUploadAsync(datapost.NewsFile, "NewsFiles/" + item.Id, "NewsFiles", item.FileId, null, transaction);
                                if (photo_upload.IsSuccess == true && !string.IsNullOrEmpty(photo_upload.FileID))
                                    item.FileId = photo_upload.FileID;
                                else
                                    _message += photo_upload.Message;
                            }

                            await _newsService.Insert(item, transaction);
                            transaction.Commit();
                            isSuccess = true;
                            target_id = item.Id.ToString();
                        }
                        catch (Exception ex)
                        {
                            _message += ex.ToString();
                            TempData["TempMsgDetail"] = "發生技術性錯誤，請聯絡技術人員或稍後再試一次";
                            unCaughtError = true;
                        }
                    }
                }
                else
                {
                    TempData["TempMsg"] = "資料回傳有誤，請重新操作！";
                }
            }
            catch (Exception ex)
            {
                TempData["TempMsg"] = "伺服器連線異常，請檢查您的網路狀態後再試一次！";
                _message += ex.ToString();
                unCaughtError = true;
            }

            if (isSuccess)
            {
                TempData["TempMsgType"] = MsgTypeEnum.success;
                TempData["TempMsg"] = "儲存成功";
            }
            else
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = TempData["TempMsg"] ?? "儲存失敗";
                if (unCaughtError)
                    await _allCommonService.Error_Record("Backend", Feature + "-" + Action, _message);
            }

            string resp = (TempData["TempMsg"] ?? "").ToString() + "\r\n" + (TempData["TempMsgDetail"] ?? "").ToString();
            await _commonService.OperateLog(userinfo.UserID, Feature, Action, target_id, datapost, _message, resp, isSuccess);

            if (isSuccess)
                return RedirectToAction("NewsList");

            if (item == null)
                return RedirectToAction("NewsList");

            return View(datapost);
        }

        public async Task<IActionResult> NewsEdit(string id)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "最新消息編輯", Action = "檢視";
            VM_News data = new VM_News();
            bool isSuccess = false;
            bool unCaughtError = false;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["TempMsgDetail"] = "缺少參數！";
                }
                else
                {
                    IQueryable<NewsExtend>? dataList = _newsService.GetNewsExtendItem(ref _message, id);
                    data.TopId = _newsService.GetTopId(ref _message);
                    if (dataList != null)
                    {
                        data.NewsExtendItem = dataList.FirstOrDefault();
                        if (data.NewsExtendItem != null)
                            data.isPublish = data.NewsExtendItem.News.IsPublish ? "上架" : "下架";
                    }

                    if (data.NewsExtendItem == null)
                        TempData["TempMsgDetail"] = "查無指定項目！";
                    else
                        isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                TempData["TempMsg"] = "伺服器連線異常，請檢查您的網路狀態後再試一次！";
                _message += ex.ToString();
                unCaughtError = true;
            }

            if (isSuccess)
            {
                await _commonService.OperateLog(userinfo.UserID, Feature, Action, id, id);
                return View(data);
            }
            else
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = TempData["TempMsg"] ?? "操作失敗";
                string response = (TempData["TempMsg"] ?? "").ToString() + "\r\n" + (TempData["TempMsgDetail"] ?? "").ToString();
                await _commonService.OperateLog(userinfo.UserID, Feature, Action, id, id, _message, response, false);
                if (unCaughtError)
                    await _allCommonService.Error_Record("Backend", Feature + "-" + Action, _message);
                return RedirectToAction("NewsList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewsEdit(string id, VM_News datapost)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "最新消息編輯", Action = "編輯";

            bool isSuccess = false;
            bool unCaughtError = false;
            DateTime dtnow = DateTime.Now;
            TbNews? item = null;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["TempMsgDetail"] = "缺少參數！";
                }
                else
                {
                    IQueryable<TbNews>? temp = _newsService.Lookup<TbNews>(ref _message, x => x.Id == id && x.IsDelete == false);
                    if (temp != null)
                        item = temp.FirstOrDefault();

                    if (item == null)
                        TempData["TempMsgDetail"] = "查無指定項目！";
                    else if (datapost.NewsExtendItem == null)
                        TempData["TempMsgDetail"] = "資料回傳有誤，請重新操作！";
                    else
                    {
                        item.Id = id;
                        item.Title = datapost.NewsExtendItem.News.Title;
                        item.Category = datapost.NewsExtendItem.News.Category;
                        item.DisplayDate = datapost.NewsExtendItem.News.DisplayDate;
                        item.Contents = datapost.NewsExtendItem.News.Contents;
                        item.IsPublish = datapost.isPublish == "上架";
                        item.IsKeepTop = datapost.NewsExtendItem.News.IsKeepTop;
                        item.ModifyUser = userinfo.UserID;
                        item.ModifyDate = dtnow;

                        using (var transaction = _newsService.GetTransaction())
                        {
                            try
                            {
                                if (datapost.NewsFile != null)
                                {
                                    var photo_upload = await _fileService.FileUploadAsync(datapost.NewsFile, "NewsFiles/" + item.Id, "NewsFiles", item.FileId, null, transaction);
                                    if (photo_upload.IsSuccess == true && !string.IsNullOrEmpty(photo_upload.FileID))
                                        item.FileId = photo_upload.FileID;
                                    else
                                        _message += photo_upload.Message;
                                }
                                else if (datapost.DelFileList != null && datapost.DelFileList.Count > 0)
                                {
                                    item.FileId = "";
                                }

                                await _newsService.Update(item, transaction);
                                transaction.Commit();
                                isSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                _message += ex.ToString();
                                TempData["TempMsgDetail"] = "發生技術性錯誤，請聯絡技術人員或稍後再試一次";
                                unCaughtError = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["TempMsg"] = "伺服器連線異常，請檢查您的網路狀態後再試一次！";
                _message += ex.ToString();
                unCaughtError = true;
            }

            if (isSuccess)
            {
                TempData["TempMsgType"] = MsgTypeEnum.success;
                TempData["TempMsg"] = "儲存成功";
            }
            else
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = TempData["TempMsg"] ?? "儲存失敗";
                if (unCaughtError)
                    await _allCommonService.Error_Record("Backend", Feature + "-" + Action, _message);
            }

            string resp = (TempData["TempMsg"] ?? "").ToString() + "\r\n" + (TempData["TempMsgDetail"] ?? "").ToString();
            await _commonService.OperateLog(userinfo.UserID, Feature, Action, id, datapost, _message, resp, isSuccess);

            if (isSuccess)
                return RedirectToAction("NewsList");

            if (item == null)
                return RedirectToAction("NewsList");

            return View(datapost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewsDelete(string id)
        {
            JsonResponse<TbNews> result = new JsonResponse<TbNews>();
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "最新消息管理", Action = "刪除";
            DateTime dtnow = DateTime.Now;
            bool isSuccess = false;
            bool unCaughtError = false;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    result.MessageDetail = "缺少參數！";
                }
                else
                {
                    TbNews? item = null;
                    IQueryable<TbNews>? temp = _newsService.Lookup<TbNews>(ref _message, x => x.Id == id && x.IsDelete == false);
                    if (temp != null)
                        item = temp.FirstOrDefault();

                    if (item == null)
                    {
                        result.MessageDetail = "查無指定項目！";
                    }
                    else
                    {
                        item.IsDelete = true;
                        item.ModifyUser = userinfo.UserID;
                        item.ModifyDate = dtnow;

                        using (var transaction = _newsService.GetTransaction())
                        {
                            try
                            {
                                await _newsService.Update(item, transaction);
                                transaction.Commit();
                                isSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                _message += ex.ToString();
                                result.MessageDetail = "發生技術性錯誤，請聯絡技術人員或稍後再試一次";
                                unCaughtError = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = "伺服器連線異常，請檢查您的網路狀態後再試一次！";
                _message += ex.ToString();
                unCaughtError = true;
            }

            if (isSuccess)
            {
                result.alert_type = "success";
                result.Message = "刪除成功";
            }
            else
            {
                result.alert_type = "error";
                result.Message = result.Message ?? "刪除失敗";
                if (unCaughtError)
                    await _allCommonService.Error_Record("Backend", Feature + "-" + Action, _message);
            }

            string response = result.Message + "\r\n" + result.MessageDetail;
            await _commonService.OperateLog(userinfo.UserID, Feature, Action, id, id, _message, response, isSuccess);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewsPublishChange(string id)
        {
            JsonResponse<TbNews> result = new JsonResponse<TbNews>();
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "最新消息管理", Action = "編輯";
            DateTime dtnow = DateTime.Now;
            bool isSuccess = false;
            bool unCaughtError = false;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    result.MessageDetail = "缺少參數！";
                }
                else
                {
                    TbNews? item = null;
                    IQueryable<TbNews>? temp = _newsService.Lookup<TbNews>(ref _message, x => x.Id == id && x.IsDelete == false);
                    if (temp != null)
                        item = temp.FirstOrDefault();

                    if (item == null)
                    {
                        result.MessageDetail = "查無指定項目！";
                    }
                    else
                    {
                        item.IsPublish = !item.IsPublish;
                        item.ModifyUser = userinfo.UserID;
                        item.ModifyDate = dtnow;

                        using (var transaction = _newsService.GetTransaction())
                        {
                            try
                            {
                                await _newsService.Update(item, transaction);
                                transaction.Commit();
                                isSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                _message += ex.ToString();
                                result.MessageDetail = "發生技術性錯誤，請聯絡技術人員或稍後再試一次";
                                unCaughtError = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = "伺服器連線異常，請檢查您的網路狀態後再試一次！";
                _message += ex.ToString();
                unCaughtError = true;
            }

            if (isSuccess)
            {
                result.alert_type = "success";
                result.Message = "狀態變更成功";
            }
            else
            {
                result.alert_type = "error";
                result.Message = result.Message ?? "狀態變更失敗";
                if (unCaughtError)
                    await _allCommonService.Error_Record("Backend", Feature + "-" + Action, _message);
            }

            string response = result.Message + "\r\n" + result.MessageDetail;
            await _commonService.OperateLog(userinfo.UserID, Feature, Action, id, id, _message, response, isSuccess);
            return Json(result);
        }
    }
}
