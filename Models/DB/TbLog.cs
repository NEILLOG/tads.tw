namespace TADS_Web.Models.DB
{
    public partial class TbLog
    {
        public int Pid { get; set; }
        public string Platform { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? Message { get; set; }
        public DateTime Date { get; set; }
        public string? Ip { get; set; }
        public string? Url { get; set; }
        public string? UserAgent { get; set; }
    }
}
