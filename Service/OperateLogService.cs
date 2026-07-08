using Microsoft.EntityFrameworkCore;
using TADS_Web.Models.DB;
using TADS_Web.Service.Base;

namespace TADS_Web.Service
{
    public class OperateLogService : ServiceBase
    {
        public OperateLogService(DBContext context) : base(context) { }

        /// <summary>後台操作紀錄查詢來源</summary>
        public IQueryable<TbBackendOperateLog>? GetLogList(ref string errMsg)
        {
            return Lookup<TbBackendOperateLog>(ref errMsg);
        }

        /// <summary>單筆完整紀錄</summary>
        public async Task<TbBackendOperateLog?> GetLog(long pid)
        {
            return await Find<TbBackendOperateLog>(pid);
        }

        /// <summary>UserId(=TbUser.Pid字串) 對應使用者，供 join 顯示帳號/姓名</summary>
        public Dictionary<string, TbUser> GetUserMap()
        {
            return _context.Set<TbUser>()
                .AsNoTracking()
                .ToDictionary(u => u.Pid.ToString(), u => u);
        }

        /// <summary>依帳號/姓名關鍵字取得對應的 UserId 清單（供 log 過濾）</summary>
        public List<string> GetUserIdsByKeyword(string keyword)
        {
            return _context.Set<TbUser>()
                .AsNoTracking()
                .Where(u => u.Account.Contains(keyword)
                            || (u.UserName != null && u.UserName.Contains(keyword)))
                .Select(u => u.Pid.ToString())
                .ToList();
        }

        /// <summary>DB 內既有的 Feature 清單（去重、排序），供搜尋下拉</summary>
        public List<string> GetDistinctFeatures()
        {
            return _context.Set<TbBackendOperateLog>()
                .AsNoTracking()
                .Select(x => x.Feature)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }
}
