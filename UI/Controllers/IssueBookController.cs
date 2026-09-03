using Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class IssueBookController : Controller
    {
        private readonly IIssueBookService _issueBookService;
        private readonly IBookService _bookService;
        private readonly IMemberService _memberService;

        public IssueBookController(IIssueBookService issueBookService, IBookService bookService, IMemberService memberService)
        {
            _issueBookService = issueBookService;
            _bookService = bookService;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
        {
            var issues = await _issueBookService.GetAllAsync();
            return View(issues);
        }

        public async Task<IActionResult> Details(int id)
        {
            var issue = await _issueBookService.GetByIdAsync(id);
            if (issue == null)
                return NotFound();

            return View(issue);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new IssueBookViewModel
            {
                Books = (await _bookService.GetAllAsync()).Where(b => b.AvailableCopies > 0).ToList(),
                Members = (await _memberService.GetAllAsync()).Where(m => m.IsActive).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IssueBookViewModel vm)
        {
            if (vm.BookId == 0 || vm.MemberId == 0)
            {
                ModelState.AddModelError("", "Please select both a book and a member");
                vm.Books = (await _bookService.GetAllAsync()).Where(b => b.AvailableCopies > 0).ToList();
                vm.Members = (await _memberService.GetAllAsync()).Where(m => m.IsActive).ToList();
                return View(vm);
            }

            var result = await _issueBookService.IssueBookAsync(vm.BookId, vm.MemberId);

            if (result != "Success")
            {
                ModelState.AddModelError("", result);
                vm.Books = (await _bookService.GetAllAsync()).Where(b => b.AvailableCopies > 0).ToList();
                vm.Members = (await _memberService.GetAllAsync()).Where(m => m.IsActive).ToList();
                return View(vm);
            }

            TempData["Success"] = "Book issued successfully";
            return RedirectToAction("Index");
        }
    }
}
