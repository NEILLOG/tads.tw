using Microsoft.AspNetCore.Mvc;
using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Models.Enums;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class AnnualMeetingController : BaseController
    {
        private readonly AllCommonService _allCommonService;
        private readonly CommonService _commonService;
        private readonly AnnualMeetingService _annualMeetingService;
        private readonly FileService _fileService;

        public AnnualMeetingController(AllCommonService allCommonService,
            FileService fileService,
            CommonService commonService,
            AnnualMeetingService annualMeetingService)
        {
            _allCommonService = allCommonService;
            _fileService = fileService;
            _commonService = commonService;
            _annualMeetingService = annualMeetingService;
        }

        private UserSessionModel GetUserInfo()
        {
            return HttpContext.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo)
                ?? new UserSessionModel { UserID = "system", UserName = "System" };
        }

        public async Task<IActionResult> List(VM_AnnualMeeting data)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "歷屆年會管理", Action = "檢視";

            try
            {
                IQueryable<TbAnnualMeeting>? dataList = _annualMeetingService.GetList(ref _message);

                if (dataList != null && !string.IsNullOrEmpty(data.Search.sTitle))
                    dataList = dataList.Where(x => x.Title.Contains(data.Search.sTitle));

                if (dataList != null)
                    data.AnnualMeetingList = await PagerInfoService.GetRange(
                        dataList.OrderBy(x => x.SortOrder).ThenByDescending(x => x.CreateDate),
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
            VM_AnnualMeeting data = new VM_AnnualMeeting();
            await _commonService.OperateLog(userinfo.UserID, "新增歷屆年會", "新增");
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VM_AnnualMeeting datapost)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "歷屆年會新增", Action = "新增";

            bool isSuccess = false;
            bool unCaughtError = false;
            DateTime dtnow = DateTime.Now;
            TbAnnualMeeting? item = new TbAnnualMeeting();
            string? target_id = null;

            try
            {
                if (datapost.AnnualMeetingItem != null)
                {
                    item.Id = await _allCommonService.IDGenerator<TbAnnualMeeting>();
                    item.Title = datapost.AnnualMeetingItem.Title;
                    item.Contents = datapost.AnnualMeetingItem.Contents;
                    item.SortOrder = datapost.AnnualMeetingItem.SortOrder;
                    item.IsDelete = false;
                    item.IsPublish = datapost.isPublish == "上架";
                    item.CreateUser = userinfo.UserID;
                    item.CreateDate = dtnow;

                    using (var transaction = _annualMeetingService.GetTransaction())
                    {
                        try
                        {
                            if (datapost.AnnualFile != null)
                            {
                                var file_upload = await _fileService.FileUploadAsync(datapost.AnnualFile, "AnnualMeetingFiles/" + item.Id, "AnnualMeetingFiles", item.FileId, null, transaction);
                                if (file_upload.IsSuccess == true && !string.IsNullOrEmpty(file_upload.FileID))
                                    item.FileId = file_upload.FileID;
                                else
                                    _message += file_upload.Message;
                            }

                            await _annualMeetingService.Insert(item, transaction);
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
            string Feature = "歷屆年會編輯", Action = "檢視";
            VM_AnnualMeeting data = new VM_AnnualMeeting();
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
                    IQueryable<TADS_Web.Models.Extend.AnnualMeetingExtend>? dataList = _annualMeetingService.GetItemExtend(ref _message, id);
                    if (dataList != null)
                    {
                        var extend = dataList.FirstOrDefault();
                        if (extend != null)
                        {
                            data.AnnualMeetingItem = extend.AnnualMeeting;
                            data.FileUrl = extend.FileUrl;
                            data.FileName = extend.FileName;
                            data.isPublish = extend.AnnualMeeting.IsPublish ? "上架" : "下架";
                        }
                    }

                    if (data.AnnualMeetingItem == null)
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
        public async Task<IActionResult> Modify(string id, VM_AnnualMeeting datapost)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "歷屆年會編輯", Action = "編輯";

            bool isSuccess = false;
            bool unCaughtError = false;
            DateTime dtnow = DateTime.Now;
            TbAnnualMeeting? item = null;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["TempMsgDetail"] = "缺少參數！";
                }
                else
                {
                    IQueryable<TbAnnualMeeting>? temp = _annualMeetingService.Lookup<TbAnnualMeeting>(ref _message, x => x.Id == id && x.IsDelete == false);
                    if (temp != null)
                        item = temp.FirstOrDefault();

                    if (item == null)
                        TempData["TempMsgDetail"] = "查無指定項目！";
                    else if (datapost.AnnualMeetingItem == null)
                        TempData["TempMsgDetail"] = "資料回傳有誤，請重新操作！";
                    else
                    {
                        item.Id = id;
                        item.Title = datapost.AnnualMeetingItem.Title;
                        item.Contents = datapost.AnnualMeetingItem.Contents;
                        item.SortOrder = datapost.AnnualMeetingItem.SortOrder;
                        item.IsPublish = datapost.isPublish == "上架";
                        item.ModifyUser = userinfo.UserID;
                        item.ModifyDate = dtnow;

                        using (var transaction = _annualMeetingService.GetTransaction())
                        {
                            try
                            {
                                if (datapost.AnnualFile != null)
                                {
                                    var file_upload = await _fileService.FileUploadAsync(datapost.AnnualFile, "AnnualMeetingFiles/" + item.Id, "AnnualMeetingFiles", item.FileId, null, transaction);
                                    if (file_upload.IsSuccess == true && !string.IsNullOrEmpty(file_upload.FileID))
                                        item.FileId = file_upload.FileID;
                                    else
                                        _message += file_upload.Message;
                                }
                                else if (datapost.DelFileList != null && datapost.DelFileList.Count > 0)
                                {
                                    await _fileService.FileDelete(datapost.DelFileList);
                                    item.FileId = "";
                                }

                                await _annualMeetingService.Update(item, transaction);
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
            JsonResponse<TbAnnualMeeting> result = new JsonResponse<TbAnnualMeeting>();
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "歷屆年會管理", Action = "刪除";
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
                    TbAnnualMeeting? item = null;
                    IQueryable<TbAnnualMeeting>? temp = _annualMeetingService.Lookup<TbAnnualMeeting>(ref _message, x => x.Id == id && x.IsDelete == false);
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

                        using (var transaction = _annualMeetingService.GetTransaction())
                        {
                            try
                            {
                                await _annualMeetingService.Update(item, transaction);
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
            JsonResponse<TbAnnualMeeting> result = new JsonResponse<TbAnnualMeeting>();
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "歷屆年會管理", Action = "編輯";
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
                    TbAnnualMeeting? item = null;
                    IQueryable<TbAnnualMeeting>? temp = _annualMeetingService.Lookup<TbAnnualMeeting>(ref _message, x => x.Id == id && x.IsDelete == false);
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

                        using (var transaction = _annualMeetingService.GetTransaction())
                        {
                            try
                            {
                                await _annualMeetingService.Update(item, transaction);
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

        /// <summary>CKEditor 內文圖片上傳（回傳 { url } 或 { error: { message } }）</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "歷屆年會-內文圖片上傳", Action = "上傳";

            try
            {
                if (upload == null || upload.Length == 0)
                    return Json(new { error = new { message = "未接收到檔案" } });

                string[] allowedExt = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
                string ext = Path.GetExtension(upload.FileName).ToLowerInvariant();
                if (!allowedExt.Contains(ext))
                    return Json(new { error = new { message = "僅允許上傳圖片檔（jpg、jpeg、png、gif、webp、bmp）" } });

                const long maxSize = 5 * 1024 * 1024;
                if (upload.Length > maxSize)
                    return Json(new { error = new { message = "圖片大小不可超過 5MB" } });

                var photo_upload = await _fileService.FileUploadAsync(upload, "AnnualMeetingContentImages", "AnnualMeetingContentImages");
                if (photo_upload.IsSuccess && !string.IsNullOrEmpty(photo_upload.FilePath))
                {
                    string url = "/" + photo_upload.FilePath;
                    await _commonService.OperateLog(userinfo.UserID, Feature, Action, photo_upload.FileID, upload.FileName, IsSuccess: true);
                    return Json(new { url });
                }

                _message += photo_upload.Message;
                await _allCommonService.Error_Record("Backend", Feature + "-" + Action, _message);
                return Json(new { error = new { message = "圖片上傳失敗，請稍後再試一次" } });
            }
            catch (Exception ex)
            {
                _message += ex.ToString();
                await _allCommonService.Error_Record("Backend", Feature + "-" + Action, _message);
                return Json(new { error = new { message = "圖片上傳發生錯誤，請聯絡技術人員" } });
            }
        }
    }
}
