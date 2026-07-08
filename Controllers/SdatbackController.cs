using Microsoft.AspNetCore.Mvc;
using TADS_Web.Extensions;
using TADS_Web.Models;
using TADS_Web.Models.DB;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class SdatbackController : Controller
    {
        private readonly AllCommonService _allCommonService;
        private readonly CommonService _commonService;
        private readonly UserService _userService;

        public SdatbackController(AllCommonService allCommonService,
                                  CommonService commonService,
                                  UserService userService)
        {
            _allCommonService = allCommonService;
            _commonService = commonService;
            _userService = userService;
        }

        private UserSessionModel? GetUserInfo()
        {
            return HttpContext.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo);
        }

        public IActionResult Welcome()
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Login");

            ViewData["Title"] = "後台首頁";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            // 已登入則直接導向後台
            if (GetUserInfo() != null)
                return RedirectToAction("Welcome");

            ViewData["Title"] = "登入";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            ViewData["Title"] = "登入";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["LoginError"] = "請輸入帳號與密碼";
                return View();
            }

            TbUser? user = _userService.ValidateLogin(email, password);

            if (user == null)
            {
                await _allCommonService.LoginRecord("Backend", "登入失敗", email);
                await _commonService.OperateLog("anonymous", "後台登入", "登入", email, email, "帳號或密碼錯誤", IsSuccess: false);
                TempData["LoginError"] = "帳號或密碼錯誤";
                return View();
            }

            HttpContext.Session.Set(SessionStruct.Login.UserInfo, new UserSessionModel
            {
                UserID = user.Pid.ToString(),
                UserName = user.UserName,
            });

            await _userService.UpdateLastLoginTime(user);
            await _allCommonService.LoginRecord("Backend", "登入成功", email, user.Pid.ToString());
            await _commonService.OperateLog(user.Pid.ToString(), "後台登入", "登入", email, email, IsSuccess: true);

            return RedirectToAction("Welcome");
        }

        public async Task<IActionResult> Logout()
        {
            UserSessionModel? userinfo = GetUserInfo();
            if (userinfo != null)
                await _commonService.OperateLog(userinfo.UserID, "後台登入", "登出", userinfo.UserID, userinfo.UserName, IsSuccess: true);

            HttpContext.Session.Remove(SessionStruct.Login.UserInfo);
            return RedirectToAction("Login");
        }
    }
}
