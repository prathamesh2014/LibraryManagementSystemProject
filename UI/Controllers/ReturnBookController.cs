using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReturnBookController : Controller
    {
        private readonly IIssueBookService _issueBookService;

        public ReturnBookController(IIssueBookService issueBookService)
        {
            _issueBookService = issueBookService;
        }

        [HttpGet]
        public async Task<IActionResult> Return(int id)
        {
            var issue = await _issueBookService.GetByIdAsync(id);
            if (issue == null)
                return NotFound();

            if (issue.Status == "Returned")
            {
                TempData["Error"] = "This book has already been returned";
                return RedirectToAction("Index", "IssueBook");
            }

            return View(issue);
        }

        [HttpPost]
        public async Task<IActionResult> Return(int issueId, bool confirm)
        {
            var fine = await _issueBookService.ReturnBookAsync(issueId);

            TempData["Fine"] = fine.ToString();
            return RedirectToAction("Confirmation", new { id = issueId });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var issue = await _issueBookService.GetByIdAsync(id);
            if (issue == null)
                return NotFound();

            return View(issue);
        }
    }
}
