using TADS_Web.Models.DB;

namespace TADS_Web.Service.Seed
{
    /// <summary>
    /// 內頁內容植入器：確保每個固定內頁在 TbPageContent 都有一筆資料。
    /// 首次啟動（或新增頁面）時，若資料列不存在，則以 SeedData/PageContent/{Code}.html 的預設 HTML 建立，
    /// 讓部署後前台外觀不變、且後台編輯者一開始就能看到並修改現有內容。冪等：已存在的資料列不會被覆寫。
    /// </summary>
    public static class PageContentSeeder
    {
        // 頁面代碼 / 後台顯示名稱（新增可編輯內頁時，於此加一列即可）
        private static readonly (string Code, string Name)[] Pages = new[]
        {
            ("Contact", "聯絡我們 Contact Us"),
            ("DevelopmentDynamics", "發展學領域動態 Development Dynamics"),
            ("RelevantLinks", "相關連結 Relevant Links"),
            ("Newsletters", "學會通訊 Newsletters"),
            // 本屆年會區塊：首頁與 /Annual/Current 共用同一份內容
            ("AnnualCurrent_Intro", "本屆年會1－最上方介紹"),
            ("AnnualCurrent_Topics", "本屆年會2－徵稿主題介紹"),
            ("AnnualCurrent_Award", "本屆年會3－學位論文獎"),
        };

        public static void Seed(DBContext db, string contentRootPath)
        {
            var existingCodes = db.TbPageContent.Select(x => x.PageCode).ToHashSet();
            bool added = false;

            foreach (var (code, name) in Pages)
            {
                if (existingCodes.Contains(code))
                    continue;

                db.TbPageContent.Add(new TbPageContent
                {
                    PageCode = code,
                    PageName = name,
                    Contents = ReadDefaultHtml(contentRootPath, code),
                    CreateUser = "system",
                    CreateDate = DateTime.Now,
                });
                added = true;
            }

            if (added)
                db.SaveChanges();
        }

        private static string? ReadDefaultHtml(string contentRootPath, string code)
        {
            try
            {
                string path = Path.Combine(contentRootPath, "SeedData", "PageContent", code + ".html");
                if (File.Exists(path))
                    return File.ReadAllText(path);
            }
            catch
            {
                // 讀檔失敗時以 null 建立（前台會退回顯示 View 內的預設靜態內容）
            }
            return null;
        }
    }
}
