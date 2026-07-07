using Microsoft.AspNetCore.Mvc;
using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Models.Enums;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    /// <summary>
    /// 內頁管理：讓後台編輯固定靜態內頁（聯絡我們、發展學領域動態、相關連結、電子報）的內容。
    /// 頁面集合固定，只提供清單與編輯（無新增 / 刪除 / 上下架）。
    /// </summary>
    public class PageContentController : BaseController
    {
        private readonly AllCommonService _allCommonService;
        private readonly CommonService _commonService;
        private readonly PageContentService _pageContentService;
        private readonly FileService _fileService;

        public PageContentController(AllCommonService allCommonService,
            FileService fileService,
            CommonService commonService,
            PageContentService pageContentService)
        {
            _allCommonService = allCommonService;
            _fileService = fileService;
            _commonService = commonService;
            _pageContentService = pageContentService;
        }

        private UserSessionModel GetUserInfo()
        {
            return HttpContext.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo)
                ?? new UserSessionModel { UserID = "system", UserName = "System" };
        }

        public async Task<IActionResult> List()
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "內頁管理", Action = "檢視";
            VM_PageContent data = new VM_PageContent();

            try
            {
                IQueryable<TbPageContent>? dataList = _pageContentService.GetList(ref _message);
                if (dataList != null)
                    data.PageContentList = dataList.OrderBy(x => x.PageName).ToList();

                await _commonService.OperateLog(userinfo.UserID, Feature, Action, null, data);
            }
            catch (Exception ex)
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = "伺服器連線異常，請檢查您的網路狀態後再試一次！";
                _message += ex.ToString();

                string response = (TempData["TempMsg"] ?? "").ToString() + "\r\n" + (TempData["TempMsgDetail"] ?? "").ToString();
                await _commonService.OperateLog(userinfo.UserID, Feature, Action, null, response: response, IsSuccess: false);
            }

            return View(data);
        }

        public async Task<IActionResult> Modify(string code)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "內頁編輯", Action = "檢視";
            VM_PageContent data = new VM_PageContent();
            bool isSuccess = false;
            bool unCaughtError = false;

            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    TempData["TempMsgDetail"] = "缺少參數！";
                }
                else
                {
                    IQueryable<TbPageContent>? dataList = _pageContentService.GetByCode(ref _message, code);
                    if (dataList != null)
                        data.PageContentItem = dataList.FirstOrDefault();

                    if (data.PageContentItem == null)
                        TempData["TempMsgDetail"] = "查無指定頁面！";
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
                await _commonService.OperateLog(userinfo.UserID, Feature, Action, code, code);
                return View(data);
            }
            else
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = TempData["TempMsg"] ?? "操作失敗";
                string response = (TempData["TempMsg"] ?? "").ToString() + "\r\n" + (TempData["TempMsgDetail"] ?? "").ToString();
                await _commonService.OperateLog(userinfo.UserID, Feature, Action, code, code, _message, response, false);
                if (unCaughtError)
                    await _allCommonService.Error_Record("Backend", Feature + "-" + Action, _message);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(string code, VM_PageContent datapost)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "內頁編輯", Action = "編輯";

            bool isSuccess = false;
            bool unCaughtError = false;
            DateTime dtnow = DateTime.Now;
            TbPageContent? item = null;

            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    TempData["TempMsgDetail"] = "缺少參數！";
                }
                else
                {
                    IQueryable<TbPageContent>? temp = _pageContentService.Lookup<TbPageContent>(ref _message, x => x.PageCode == code);
                    if (temp != null)
                        item = temp.FirstOrDefault();

                    if (item == null)
                        TempData["TempMsgDetail"] = "查無指定頁面！";
                    else if (datapost.PageContentItem == null)
                        TempData["TempMsgDetail"] = "資料回傳有誤，請重新操作！";
                    else
                    {
                        item.Contents = datapost.PageContentItem.Contents;
                        item.ModifyUser = userinfo.UserID;
                        item.ModifyDate = dtnow;

                        using (var transaction = _pageContentService.GetTransaction())
                        {
                            try
                            {
                                await _pageContentService.Update(item, transaction);
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
            await _commonService.OperateLog(userinfo.UserID, Feature, Action, code, datapost, _message, resp, isSuccess);

            if (isSuccess)
                return RedirectToAction("List");

            return View(datapost);
        }

        /// <summary>CKEditor 內文圖片上傳（回傳 { url } 或 { error: { message } }）</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(IFormFile upload)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "內頁-內文圖片上傳", Action = "上傳";

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

                var photo_upload = await _fileService.FileUploadAsync(upload, "PageContentImages", "PageContentImages");
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
