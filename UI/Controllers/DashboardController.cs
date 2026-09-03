using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;
        private readonly IIssueBookService _issueBookService;

        public DashboardController(IBookService bookService, IMemberService memberService, IIssueBookService issueBookService)
        {
            _bookService = bookService;
            _memberService = memberService;
            _issueBookService = issueBookService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllAsync();
            var members = await _memberService.GetAllAsync();
            var issues = await _issueBookService.GetAllAsync();

            var vm = new DashboardViewModel
            {
                TotalBooks = books.Count,
                TotalMembers = members.Count,
                IssuedBooksCount = issues.Count,
                ReturnedBooksCount = issues.Count(i => i.Status == "Returned"),
                PendingBooksCount = issues.Count(i => i.Status == "Issued"),
                RecentIssues = issues.Take(5).ToList()
            };

            return View(vm);
        }
    }
}
