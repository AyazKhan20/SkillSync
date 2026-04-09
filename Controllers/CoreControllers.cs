using Microsoft.AspNetCore.Mvc;
using SmartJobRecommender.Models;
using SmartJobRecommender.Services;

namespace SmartJobRecommender.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Explore() => View();
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    public class DashboardController : Controller
    {
        private readonly IResumeService _resumeService;

        public DashboardController(IResumeService resumeService)
        {
            _resumeService = resumeService;
        }

        public IActionResult Index() => View();
        
        [HttpGet]
        public IActionResult Analyze() => View();

        [HttpPost]
        public async Task<IActionResult> Analyze(IFormFile resume)
        {
            if (resume == null || resume.Length == 0)
            {
                ModelState.AddModelError("", "Please select a valid PDF file.");
                return View();
            }

            using var stream = resume.OpenReadStream();
            var skills = await _resumeService.ExtractSkillsAsync(stream);
            
            ViewBag.DetectedSkills = skills;
            return View("AnalysisResult");
        }
    }
}
