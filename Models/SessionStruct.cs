namespace TADS_Web.Models
{
    public class SessionStruct
    {
        public struct Login
        {
            public static string UserInfo = "UserInfo";
            public static string Menu = "Menu";
        }
    }

    public class UserSessionModel
    {
        public string UserID { get; set; } = null!;
        public string? UserName { get; set; }
        public string? GroupID { get; set; }
    }
}
