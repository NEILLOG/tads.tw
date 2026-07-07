using Microsoft.AspNetCore.Mvc;
using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Models.Enums;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    /// <summary>
    /// 關於我們管理：以結構化欄位編輯 About 頁各區塊（單筆記錄 + 6 張會費卡片 + 3 個上傳覆蓋檔案）。
    /// </summary>
    public class AboutManageController : BaseController
    {
        private readonly AllCommonService _allCommonService;
        private readonly CommonService _commonService;
        private readonly AboutContentService _aboutService;
        private readonly FileService _fileService;

        private static readonly string[] ImageExt = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

        public AboutManageController(AllCommonService allCommonService,
            FileService fileService,
            CommonService commonService,
            AboutContentService aboutService)
        {
            _allCommonService = allCommonService;
            _fileService = fileService;
            _commonService = commonService;
            _aboutService = aboutService;
        }

        private UserSessionModel GetUserInfo()
        {
            return HttpContext.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo)
                ?? new UserSessionModel { UserID = "system", UserName = "System" };
        }

        private string? FileUrl(string? fileId)
        {
            if (string.IsNullOrEmpty(fileId)) return null;
            var info = _fileService.GetFileInfo(fileId);
            return info == null ? null : "/" + info.FilePath;
        }

        public async Task<IActionResult> Edit()
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "關於我們管理", Action = "檢視";
            VM_AboutContent data = new VM_AboutContent();

            try
            {
                var about = _aboutService.GetAbout(ref _message);
                if (about != null) data.AboutItem = about;
                data.PricingList = _aboutService.GetPricing(ref _message);
                data.IntroImageUrl = FileUrl(data.AboutItem.IntroImageFileId);
                data.OrgChartUrl = FileUrl(data.AboutItem.OrgChartFileId);
                data.ConstitutionPdfUrl = FileUrl(data.AboutItem.ConstitutionPdfFileId);

                await _commonService.OperateLog(userinfo.UserID, Feature, Action, AboutContentService.AboutId, AboutContentService.AboutId);
            }
            catch (Exception ex)
            {
                TempData["TempMsgType"] = MsgTypeEnum.error;
                TempData["TempMsg"] = "伺服器連線異常，請檢查您的網路狀態後再試一次！";
                _message += ex.ToString();
                await _commonService.OperateLog(userinfo.UserID, Feature, Action, AboutContentService.AboutId, response: _message, IsSuccess: false);
            }

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VM_AboutContent datapost,
            IFormFile? IntroImageFile, IFormFile? OrgChartFile, IFormFile? ConstitutionPdfFile)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "關於我們管理", Action = "編輯";
            DateTime dtnow = DateTime.Now;
            bool isSuccess = false;
            bool unCaughtError = false;

            try
            {
                TbAboutContent? item = _aboutService.Lookup<TbAboutContent>(ref _message, x => x.Id == AboutContentService.AboutId)?.FirstOrDefault();
                if (item == null)
                {
                    TempData["TempMsgDetail"] = "查無關於我們資料！";
                }
                else if (datapost.AboutItem == null)
                {
                    TempData["TempMsgDetail"] = "資料回傳有誤，請重新操作！";
                }
                else
                {
                    // 文字欄位
                    var d = datapost.AboutItem;
                    item.IntroNameZh = d.IntroNameZh;
                    item.IntroNameEn = d.IntroNameEn;
                    item.IntroSummary = d.IntroSummary;
                    item.IntroDetail = d.IntroDetail;
                    item.BoardTermNo = d.BoardTermNo;
                    item.BoardPresident = d.BoardPresident;
                    item.BoardVicePresident = d.BoardVicePresident;
                    item.BoardExecDirectors = d.BoardExecDirectors;
                    item.BoardDirectors = d.BoardDirectors;
                    item.BoardExecSupervisor = d.BoardExecSupervisor;
                    item.BoardSupervisors = d.BoardSupervisors;
                    item.BoardSecretaryGeneral = d.BoardSecretaryGeneral;
                    item.BoardExecSecretary = d.BoardExecSecretary;
                    item.PastBoardListUrl = d.PastBoardListUrl;
                    item.MembershipInviteZh = d.MembershipInviteZh;
                    item.MembershipInviteEn = d.MembershipInviteEn;
                    item.ConstitutionText = d.ConstitutionText;
                    item.MemberResearchDesc = d.MemberResearchDesc;

                    // 檔案上傳（上傳覆蓋：帶入舊 FileId，FileService 會自動軟刪舊檔）
                    string? fileErr = null;
                    var introUp = await HandleUpload(IntroImageFile, ImageExt, "About-IntroImage", item.IntroImageFileId, v => fileErr = v);
                    if (introUp != null) item.IntroImageFileId = introUp;
                    var orgUp = await HandleUpload(OrgChartFile, ImageExt, "About-OrgChart", item.OrgChartFileId, v => fileErr = v);
                    if (orgUp != null) item.OrgChartFileId = orgUp;
                    var pdfUp = await HandleUpload(ConstitutionPdfFile, new[] { ".pdf" }, "About-ConstitutionPdf", item.ConstitutionPdfFileId, v => fileErr = v);
                    if (pdfUp != null) item.ConstitutionPdfFileId = pdfUp;

                    item.ModifyUser = userinfo.UserID;
                    item.ModifyDate = dtnow;

                    // 會費卡片
                    var existingPricing = _aboutService.Lookup<TbAboutPricing>(ref _message)?.ToList() ?? new List<TbAboutPricing>();
                    var pricingToUpdate = new List<TbAboutPricing>();
                    if (datapost.PricingList != null)
                    {
                        foreach (var posted in datapost.PricingList)
                        {
                            var target = existingPricing.FirstOrDefault(x => x.Id == posted.Id);
                            if (target == null) continue;
                            target.NameZh = posted.NameZh;
                            target.NameEn = posted.NameEn;
                            target.Price = posted.Price;
                            target.DescZh = posted.DescZh;
                            target.DescEn = posted.DescEn;
                            pricingToUpdate.Add(target);
                        }
                    }

                    using (var transaction = _aboutService.GetTransaction())
                    {
                        try
                        {
                            await _aboutService.Update(item, transaction);
                            if (pricingToUpdate.Count > 0)
                                await _aboutService.UpdateRange(pricingToUpdate, transaction);
                            transaction.Commit();
                            isSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            _message += ex.ToString();
                            TempData["TempMsgDetail"] = fileErr ?? "發生技術性錯誤，請聯絡技術人員或稍後再試一次";
                            unCaughtError = true;
                        }
                    }

                    if (isSuccess && !string.IsNullOrEmpty(fileErr))
                        TempData["TempMsgDetail"] = fileErr; // 內容已存，但個別檔案未通過驗證
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
            await _commonService.OperateLog(userinfo.UserID, Feature, Action, AboutContentService.AboutId, AboutContentService.AboutId, _message, resp, isSuccess);

            return RedirectToAction("Edit");
        }

        /// <summary>上傳單一檔案並回傳新 FileId；無檔案回傳 null；驗證失敗回傳 null 並透過 setError 回報。</summary>
        private async Task<string?> HandleUpload(IFormFile? file, string[] allowedExt, string desc, string? oldFileId, Action<string> setError)
        {
            if (file == null || file.Length == 0) return null;

            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext))
            {
                setError($"檔案「{file.FileName}」格式不允許（僅接受 {string.Join("、", allowedExt)}）");
                return null;
            }
            const long maxSize = 15 * 1024 * 1024;
            if (file.Length > maxSize)
            {
                setError($"檔案「{file.FileName}」超過 15MB 上限");
                return null;
            }

            var up = await _fileService.FileUploadAsync(file, "AboutFiles", desc, oldFileId);
            if (up.IsSuccess && !string.IsNullOrEmpty(up.FileID))
                return up.FileID;

            setError($"檔案「{file.FileName}」上傳失敗");
            return null;
        }
    }
}
