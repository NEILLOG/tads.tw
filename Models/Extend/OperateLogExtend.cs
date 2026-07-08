using TADS_Web.Models.DB;

namespace TADS_Web.Models.Extend
{
    /// <summary>後台操作紀錄 + 使用者資訊（帳號、姓名）</summary>
    public class OperateLogExtend
    {
        public TbBackendOperateLog Log { get; set; } = null!;

        /// <summary>操作者帳號（由 TbUser.Account join；查無則為 null）</summary>
        public string? Account { get; set; }

        /// <summary>操作者姓名（由 TbUser.UserName join；查無則為 null）</summary>
        public string? UserName { get; set; }
    }
}
