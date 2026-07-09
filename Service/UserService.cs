using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TADS_Web.Models.DB;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class UserService : ServiceBase
    {
        public const int PasswordMinLength = 8;

        public UserService(DBContext context) : base(context) { }

        public static string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        /// <summary>基本密碼原則：至少 8 碼，且需同時包含英文字母與數字。</summary>
        public static bool IsValidPasswordPolicy(string password, out string errorMessage)
        {
            if (string.IsNullOrEmpty(password) || password.Length < PasswordMinLength)
            {
                errorMessage = $"密碼長度至少需 {PasswordMinLength} 碼";
                return false;
            }
            if (!Regex.IsMatch(password, "[A-Za-z]") || !Regex.IsMatch(password, "[0-9]"))
            {
                errorMessage = "密碼需同時包含英文字母與數字";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        public TbUser? ValidateLogin(string account, string password)
        {
            string hash = HashPassword(password);
            return _context.TbUser.FirstOrDefault(x =>
                x.IsDelete == false && x.Account == account && x.Password == hash);
        }

        public async Task<TbUser?> GetUserById(int pid)
        {
            return await Find<TbUser>(pid);
        }

        public bool VerifyPassword(TbUser user, string password)
        {
            return user.Password == HashPassword(password);
        }

        public async Task<bool> ChangePassword(TbUser user, string newPassword)
        {
            user.Password = HashPassword(newPassword);
            var result = await Update(user);
            return result.IsSuccess;
        }

        public async Task UpdateLastLoginTime(TbUser user)
        {
            user.LastLoginTime = DateTime.Now;
            await Update(user);
        }

        public async Task EnsureDefaultUser(string account, string password, string userName)
        {
            if (!_context.TbUser.Any(x => x.Account == account))
            {
                await Insert(new TbUser
                {
                    Account = account,
                    Password = HashPassword(password),
                    UserName = userName,
                    IsDelete = false,
                    CreateDate = DateTime.Now
                });
            }
        }
    }
}
