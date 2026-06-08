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
        private readonly UserService _userService;

        public SdatbackController(AllCommonService allCommonService,
                                  UserService userService)
        {
            _allCommonService = allCommonService;
            _userService = userService;
        }

        private UserSessionModel? GetUserInfo()
        {
            return HttpContext.Session.Get<UserSessionModel>(SessionStruct.Login.UserInfo);
        }

        public IActionResult Sample()
        {
            if (GetUserInfo() == null)
                return RedirectToAction("Login");

            ViewData["Title"] = "Sample Page";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            // 已登入則直接導向後台
            if (GetUserInfo() != null)
                return RedirectToAction("Sample");

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

            return RedirectToAction("Sample");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionStruct.Login.UserInfo);
            return RedirectToAction("Login");
        }
    }
}
