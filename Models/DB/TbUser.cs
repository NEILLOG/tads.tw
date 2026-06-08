namespace TADS_Web.Models.DB
{
    public partial class TbUser
    {
        public int Pid { get; set; }
        public string Account { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? UserName { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastLoginTime { get; set; }
    }
}
