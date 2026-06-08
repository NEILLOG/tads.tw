using System.Security.Cryptography;
using System.Text;
using TADS_Web.Models.DB;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class UserService : ServiceBase
    {
        public UserService(DBContext context) : base(context) { }

        public static string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        public TbUser? ValidateLogin(string account, string password)
        {
            string hash = HashPassword(password);
            return _context.TbUser.FirstOrDefault(x =>
                x.IsDelete == false && x.Account == account && x.Password == hash);
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
