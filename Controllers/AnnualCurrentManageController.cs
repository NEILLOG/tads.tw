using Microsoft.AspNetCore.Mvc;
using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Models.Enums;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    /// <summary>
    /// 本屆年會管理：單一頁面編輯三個區塊（最上方介紹、徵稿主題介紹、學位論文獎），
    /// 內容存於 TbPageContent（AnnualCurrent_Intro / _Topics / _Award），首頁與 /Annual/Current 共用；
    /// 學位論文獎下載檔為獨立上傳欄位（存於 Award 資料列的 FileId，上傳新檔即覆蓋）。
    /// </summary>
    public class AnnualCurrentManageController : BaseController
    {
        private const string IntroCode = "AnnualCurrent_Intro";
        private const string TopicsCode = "AnnualCurrent_Topics";
        private const string AwardCode = "AnnualCurrent_Award";

        private readonly AllCommonService _allCommonService;
        private readonly CommonService _commonService;
        private readonly PageContentService _pageContentService;
        private readonly FileService _fileService;

        public AnnualCurrentManageController(AllCommonService allCommonService,
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

        public async Task<IActionResult> Edit()
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "本屆年會管理", Action = "檢視";

            VM_AnnualCurrentContent data = _pageContentService.GetAnnualCurrentContent(ref _message);
            await _commonService.OperateLog(userinfo.UserID, Feature, Action);
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VM_AnnualCurrentContent datapost, IFormFile? AwardFile)
        {
            UserSessionModel userinfo = GetUserInfo();
            string Feature = "本屆年會管理", Action = "編輯";
            DateTime dtnow = DateTime.Now;
            bool isSuccess = false;
            bool unCaughtError = false;

            try
            {
                string[] codes = { IntroCode, TopicsCode, AwardCode };
                var items = _pageContentService.Lookup<TbPageContent>(ref _message, x => codes.Contains(x.PageCode))?.ToList()
                    ?? new List<TbPageContent>();

                TbPageContent? intro = items.FirstOrDefault(x => x.PageCode == IntroCode);
                TbPageContent? topics = items.FirstOrDefault(x => x.PageCode == TopicsCode);
                TbPageContent? award = items.FirstOrDefault(x => x.PageCode == AwardCode);

                if (intro == null || topics == null || award == null)
                {
                    TempData["TempMsgDetail"] = "查無本屆年會資料列，請重新啟動網站以重建預設資料！";
                }
                else
                {
                    intro.Contents = datapost.IntroHtml;
                    topics.Contents = datapost.TopicsHtml;
                    award.Contents = datapost.AwardHtml;
                    foreach (var item in new[] { intro, topics, award })
                    {
                        item.ModifyUser = userinfo.UserID;
                        item.ModifyDate = dtnow;
                    }

                    // 學位論文獎下載檔（上傳覆蓋：帶入舊 FileId，FileService 會自動軟刪舊檔）
                    string? fileErr = null;
                    if (AwardFile != null && AwardFile.Length > 0)
                    {
                        string ext = Path.GetExtension(AwardFile.FileName).ToLowerInvariant();
                        const long maxSize = 15 * 1024 * 1024;
                        if (ext != ".pdf")
                            fileErr = $"檔案「{AwardFile.FileName}」格式不允許（僅接受 .pdf）";
                        else if (AwardFile.Length > maxSize)
                            fileErr = $"檔案「{AwardFile.FileName}」超過 15MB 上限";
                        else
                        {
                            var up = await _fileService.FileUploadAsync(AwardFile, "AnnualCurrentFiles", "AnnualCurrent-AwardFile", award.FileId);
                            if (up.IsSuccess && !string.IsNullOrEmpty(up.FileID))
                                award.FileId = up.FileID;
                            else
                                fileErr = $"檔案「{AwardFile.FileName}」上傳失敗";
                        }
                    }

                    using (var transaction = _pageContentService.GetTransaction())
                    {
                        try
                        {
                            await _pageContentService.UpdateRange(new List<TbPageContent> { intro, topics, award }, transaction);
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
                        TempData["TempMsgDetail"] = fileErr; // 內容已存，但檔案未通過驗證
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
            await _commonService.OperateLog(userinfo.UserID, Feature, Action, null, datapost, _message, resp, isSuccess);

            return RedirectToAction("Edit");
        }
    }
}
