namespace TADS_Web.Models.DB
{
    public partial class TbLoginRecord
    {
        public int Pid { get; set; }
        public string Platform { get; set; } = null!;
        public string? Account { get; set; }
        public string? MemberId { get; set; }
        public string? Inputaua8 { get; set; }
        public string? Ip { get; set; }
        public string? UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public string LoginMsg { get; set; } = null!;
        public string? UserAgent { get; set; }
        public bool Sso { get; set; }
        public string? Ssoresult { get; set; }
    }
}
