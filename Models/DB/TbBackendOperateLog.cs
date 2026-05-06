namespace TADS_Web.Models.DB
{
    public partial class TbBackendOperateLog
    {
        public long Pid { get; set; }
        public string UserId { get; set; } = null!;
        public string Feature { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? Ip { get; set; }
        public string? Url { get; set; }
        public string? UserAgent { get; set; }
        public bool IsSuccess { get; set; }
        public string? DataKey { get; set; }
        public string? Message { get; set; }
        public string? Request { get; set; }
        public string? Response { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
