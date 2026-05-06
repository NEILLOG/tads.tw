namespace TADS_Web.Models.DB
{
    public partial class TbFileInfo
    {
        public string FileId { get; set; } = null!;
        public string? FileName { get; set; }
        public string? FileRealName { get; set; }
        public string? FileDescription { get; set; }
        public string FilePath { get; set; } = null!;
        public string? FilePathM { get; set; }
        public string? FilePathS { get; set; }
        public byte? Order { get; set; }
        public bool IsDelete { get; set; }
        public string? CreateUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
