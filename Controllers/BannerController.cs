using Microsoft.AspNetCore.Mvc;
using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Models.Enums;
using TADS_Web.Models.Extend;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class BannerController : BaseController
    {
        private readonly AllCommonService _allCommonService;
        private readonly CommonService _commonService;
        private readonly BannerService _bannerService;
        private readonly FileService _fileService;

        public BannerController(AllCommonService allCommonService,
            FileService fileService,
            CommonService commonService,
            BannerService bannerService)
        {
            _allCommonService = allCommonService;
            _fileService = fileService;
            _commonService = commonService;
            _bannerService = bannerService;
        }

        private UserSessionModel GetUserInfo()
        {
            return HttpContext.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo)
                ?? new UserSessionModel { UserID = "system", UserName = "System" };
        }

        public async Task<IActionResult> List(VM_Banner data)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "橫幅管理", Action = "檢視";

            try
            {
                IQueryable<BannerExtend>? dataList = _bannerService.GetExtendList(ref _message);

                if (dataList != null && !string.IsNullOrEmpty(data.Search.sTitle))
                    dataList = dataList.Where(x => x.Banner.Title != null && x.Banner.Title.Contains(data.Search.sTitle));

                if (dataList != null)
                    data.BannerList = await PagerInfoService.GetRange(
                        dataList.OrderBy(x => x.Banner.SortOrder).ThenByDescending(x => x.Banner.CreateDate),
                        data.Search.PagerInfo);

                await _commonService.OperateLog(userinfo.UserID, Feature, Action, null, data);
            }
            catch (Exception ex)
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = "伺服器連線異常，請檢查您的網路狀態後再試一次！";
                _message += ex.ToString();

                string response = (TempData["TempMsg"] ?? "").ToString() + "\r\n" + (TempData["TempMsgDetail"] ?? "").ToString();
                await _commonService.OperateLog(userinfo.UserID, Feature, Action, null, response: response, IsSuccess: false);

                return RedirectToAction("List");
            }

            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            UserSessionModel userinfo = GetUserInfo();
            VM_Banner data = new VM_Banner();
            await _commonService.OperateLog(userinfo.UserID, "新增橫幅", "新增");
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VM_Banner datapost)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "橫幅新增", Action = "新增";

            bool isSuccess = false;
            bool unCaughtError = false;
            DateTime dtnow = DateTime.Now;
            TbBanner? item = new TbBanner();
            string? target_id = null;

            try
            {
                if (datapost.BannerItem != null)
                {
                    item.Id = await _allCommonService.IDGenerator<TbBanner>();
                    item.Title = datapost.BannerItem.Banner.Title;
                    item.LinkUrl = datapost.BannerItem.Banner.LinkUrl;
                    item.SortOrder = datapost.BannerItem.Banner.SortOrder;
                    item.IsDelete = false;
                    item.IsPublish = datapost.isPublish == "上架";
                    item.CreateUser = userinfo.UserID;
                    item.CreateDate = dtnow;

                    using (var transaction = _bannerService.GetTransaction())
                    {
                        try
                        {
                            if (datapost.BannerFile != null)
                            {
                                var photo_upload = await _fileService.FileUploadAsync(datapost.BannerFile, "BannerFiles/" + item.Id, "BannerFiles", item.FileId, null, transaction);
                                if (photo_upload.IsSuccess == true && !string.IsNullOrEmpty(photo_upload.FileID))
                                    item.FileId = photo_upload.FileID;
                                else
                                    _message += photo_upload.Message;
                            }

                            await _bannerService.Insert(item, transaction);
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
                return RedirectToAction("List");

            return View(datapost);
        }

        public async Task<IActionResult> Modify(string id)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "橫幅編輯", Action = "檢視";
            VM_Banner data = new VM_Banner();
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
                    IQueryable<BannerExtend>? dataList = _bannerService.GetExtendItem(ref _message, id);
                    if (dataList != null)
                    {
                        data.BannerItem = dataList.FirstOrDefault();
                        if (data.BannerItem != null)
                            data.isPublish = data.BannerItem.Banner.IsPublish ? "上架" : "下架";
                    }

                    if (data.BannerItem == null)
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
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(string id, VM_Banner datapost)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "橫幅編輯", Action = "編輯";

            bool isSuccess = false;
            bool unCaughtError = false;
            DateTime dtnow = DateTime.Now;
            TbBanner? item = null;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["TempMsgDetail"] = "缺少參數！";
                }
                else
                {
                    IQueryable<TbBanner>? temp = _bannerService.Lookup<TbBanner>(ref _message, x => x.Id == id && x.IsDelete == false);
                    if (temp != null)
                        item = temp.FirstOrDefault();

                    if (item == null)
                        TempData["TempMsgDetail"] = "查無指定項目！";
                    else if (datapost.BannerItem == null)
                        TempData["TempMsgDetail"] = "資料回傳有誤，請重新操作！";
                    else
                    {
                        item.Id = id;
                        item.Title = datapost.BannerItem.Banner.Title;
                        item.LinkUrl = datapost.BannerItem.Banner.LinkUrl;
                        item.SortOrder = datapost.BannerItem.Banner.SortOrder;
                        item.IsPublish = datapost.isPublish == "上架";
                        item.ModifyUser = userinfo.UserID;
                        item.ModifyDate = dtnow;

                        using (var transaction = _bannerService.GetTransaction())
                        {
                            try
                            {
                                if (datapost.BannerFile != null)
                                {
                                    var photo_upload = await _fileService.FileUploadAsync(datapost.BannerFile, "BannerFiles/" + item.Id, "BannerFiles", item.FileId, null, transaction);
                                    if (photo_upload.IsSuccess == true && !string.IsNullOrEmpty(photo_upload.FileID))
                                        item.FileId = photo_upload.FileID;
                                    else
                                        _message += photo_upload.Message;
                                }
                                else if (datapost.DelFileList != null && datapost.DelFileList.Count > 0)
                                {
                                    item.FileId = "";
                                }

                                await _bannerService.Update(item, transaction);
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
                return RedirectToAction("List");

            return View(datapost);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            JsonResponse<TbBanner> result = new JsonResponse<TbBanner>();
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "橫幅管理", Action = "刪除";
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
                    TbBanner? item = null;
                    IQueryable<TbBanner>? temp = _bannerService.Lookup<TbBanner>(ref _message, x => x.Id == id && x.IsDelete == false);
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

                        using (var transaction = _bannerService.GetTransaction())
                        {
                            try
                            {
                                await _bannerService.Update(item, transaction);
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
        public async Task<IActionResult> PublishChange(string id)
        {
            JsonResponse<TbBanner> result = new JsonResponse<TbBanner>();
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "橫幅管理", Action = "編輯";
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
                    TbBanner? item = null;
                    IQueryable<TbBanner>? temp = _bannerService.Lookup<TbBanner>(ref _message, x => x.Id == id && x.IsDelete == false);
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

                        using (var transaction = _bannerService.GetTransaction())
                        {
                            try
                            {
                                await _bannerService.Update(item, transaction);
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
