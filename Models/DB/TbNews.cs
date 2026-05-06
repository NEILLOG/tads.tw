namespace TADS_Web.Models.DB
{
    public partial class TbNews
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public DateTime DisplayDate { get; set; }
        public string Contents { get; set; } = null!;
        public bool IsDelete { get; set; }
        public bool IsPublish { get; set; }
        public bool IsKeepTop { get; set; }
        public string? FileId { get; set; }
        public string CreateUser { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
