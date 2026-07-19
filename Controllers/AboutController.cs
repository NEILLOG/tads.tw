using Microsoft.AspNetCore.Mvc;
using TADS_Web.Models;
using TADS_Web.Service;

namespace TADS_Web.Controllers
{
    public class AboutController : BaseController
    {
        private readonly AboutContentService _aboutService;
        private readonly FileService _fileService;

        public AboutController(AboutContentService aboutService, FileService fileService)
        {
            _aboutService = aboutService;
            _fileService = fileService;
        }

        private string? FileUrl(string? fileId)
        {
            if (string.IsNullOrEmpty(fileId)) return null;
            var info = _fileService.GetFileInfo(fileId);
            return info == null ? null : "/" + info.FilePath;
        }

        public IActionResult Index()
        {
            VM_AboutContent data = new VM_AboutContent();
            var about = _aboutService.GetAbout(ref _message);
            if (about != null) data.AboutItem = about;
            data.PricingList = _aboutService.GetPricing(ref _message);

            // 檔案 URL：無上傳檔時退回原本的靜態預設資產
            data.IntroImageUrl = FileUrl(data.AboutItem.IntroImageFileId) ?? "/img/about_fig.jpg";
            data.OrgChartUrl = FileUrl(data.AboutItem.OrgChartFileId) ?? "/img/Organizational_Structure.png";
            data.ConstitutionPdfUrl = FileUrl(data.AboutItem.ConstitutionPdfFileId) ?? "/static/Constitution.1081025.pdf";
            data.MembershipFormUrl = FileUrl(data.AboutItem.MembershipFormFileId);

            return View(data);
        }
    }
}
