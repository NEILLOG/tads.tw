using TADS_Web.Models.DB;
using TADS_Web.Service;

namespace TADS_Web.Service.Seed
{
    /// <summary>
    /// 「關於我們」內容植入器（冪等）：首次啟動時建立 About 主記錄與 6 張會費卡片，
    /// 帶入頁面現有內容，讓部署後外觀不變、後台一開始就能看到並修改現有內容。
    /// 已存在的資料列不會被覆寫。長文（詳細內文、章程）從 SeedData/AboutContent/*.txt 讀取。
    /// </summary>
    public static class AboutContentSeeder
    {
        public static void Seed(DBContext db, string contentRootPath)
        {
            bool changed = false;

            if (!db.TbAboutContent.Any(x => x.Id == AboutContentService.AboutId))
            {
                db.TbAboutContent.Add(new TbAboutContent
                {
                    Id = AboutContentService.AboutId,
                    IntroNameZh = "台灣發展研究學會",
                    IntroNameEn = "Taiwanese Association for Development Studies, TADS",
                    IntroSummary = "發展研究（development studies），起初以國家為主的經濟發展考量，近年來逐漸轉移到個人、社區、參與以及平等，強調發展是要符合人民需要、與環境和平共處。為實現發展的永續性，除借鑑前人的經驗更需結合現今資源網絡，我們期許能夠找到一條可以實現永續的發展道路。",
                    IntroDetail = ReadSeedText(contentRootPath, "IntroDetail.txt"),
                    IntroImageFileId = null,

                    OrgChartFileId = null,
                    BoardTermNo = 7,
                    BoardPresident = "張國暉",
                    BoardVicePresident = "游尚儒",
                    BoardExecDirectors = "何浩慈、周嘉辰、張國暉、游尚儒",
                    BoardDirectors = "何浩慈、周嘉辰、林木興、林義鈞、林政逸、邱啟新、張國暉、郭益昌、游尚儒、曾聖文、黃兆年、劉秋婉、鄭安廷、蘇昱璇",
                    BoardExecSupervisor = "魏玫娟",
                    BoardSupervisors = "周桂田、陳良治、湯京平、簡旭伸、魏玫娟",
                    BoardSecretaryGeneral = "林竣達",
                    BoardExecSecretary = "王曉珩",
                    PastBoardListUrl = "https://drive.google.com/file/d/1iRwgpYdpH6AoOImzK4EcRxpdevo7lnWJ/view",

                    MembershipInviteZh = "誠摯邀請您加入「台灣發展研究學會（TADS）」！本會致力於推動國內外發展議題的學術對話與政策實踐。成為會員後，您將能優先參與年度研討會、掌握最新研究動態，並與跨領域的頂尖學者及實務工作者建立緊密的專業網絡。無論您是學者、學生或關注發展議題的專業人士，我們都歡迎您攜手共進，為全球永續發展貢獻心力。立即申請入會，擴大您的學術與社會影響力！",
                    MembershipInviteEn = "We cordially invite you to join the Taiwanese Association for Development Studies (TADS)! As Taiwan's first national academic society dedicated to development studies, our vision has expanded from macroeconomic perspectives to the sustainable coexistence of individuals, communities, and the environment. TADS serves as a premier cross-disciplinary platform, connecting leading scholars and practitioners through our benchmark annual conferences. Whether you are focused on pioneering theory or local practice, we welcome you to join us in inspiring innovative solutions, building international networks, and contributing to the sustainable development of contemporary society!",
                    MembershipFormFileId = null,

                    ConstitutionText = ReadSeedText(contentRootPath, "Constitution.txt"),
                    ConstitutionPdfFileId = null,

                    MemberResearchDesc = "「以文會友」是台灣發展研究學會分享會員研究成果的專欄。我們匯集學會會員在發展研究及相關領域的著作、論文與研究成果，期盼以文字串連跨領域的學術同好，促進對話與交流。歡迎您閱覽會員們的最新研究，一同關注台灣與全球發展議題。",

                    CreateUser = "system",
                    CreateDate = DateTime.Now,
                });
                changed = true;
            }

            if (!db.TbAboutPricing.Any())
            {
                var cards = new[]
                {
                    ("入會費", "Initiation Fee", "100", "若在入會首年繳交常年會費者免繳入會費。", "The initiation fee is waived if the annual membership fee is paid in the first year of joining."),
                    ("個人會員", "Individual Member", "1,000", "以發展研究及相關學科之教學或研究為主要職業，或各大學相關系所博士候選人。", "Professionals primarily engaged in teaching or research in development studies and related disciplines, or Ph.D. candidates in relevant university departments or institutes."),
                    ("學生會員", "Student Member", "500", "就讀於大學或研究所發展研究及相關系所學生。", "Students enrolled in undergraduate or graduate programs in development studies or related departments / institutes."),
                    ("團體會員", "Institutional Member", "5,000", "全國大專院校之發展研究相關系所或相關學術團體。", "Development studies-related departments or institutes at universities and colleges nationwide, or relevant academic organizations."),
                    ("退休會員", "Retired Member", "500", "個人會員退休或已離開研究及教學工作。", "Individual members who have retired or left research and teaching positions."),
                    ("終身會員", "Lifetime Member", "20,000", "符合個人會員條件，繳交一定數額會費者。", "Individuals who meet the qualifications for Individual Membership and have paid the designated lifetime membership fee."),
                };

                for (int i = 0; i < cards.Length; i++)
                {
                    var (nameZh, nameEn, price, descZh, descEn) = cards[i];
                    db.TbAboutPricing.Add(new TbAboutPricing
                    {
                        Id = "P" + (i + 1),
                        SortOrder = i + 1,
                        NameZh = nameZh,
                        NameEn = nameEn,
                        Price = price,
                        DescZh = descZh,
                        DescEn = descEn,
                    });
                }
                changed = true;
            }

            if (changed)
                db.SaveChanges();
        }

        private static string? ReadSeedText(string contentRootPath, string fileName)
        {
            try
            {
                string path = Path.Combine(contentRootPath, "SeedData", "AboutContent", fileName);
                if (File.Exists(path))
                    return File.ReadAllText(path);
            }
            catch
            {
                // 讀檔失敗時以 null 建立（前台會退回顯示樣板預設）
            }
            return null;
        }
    }
}
