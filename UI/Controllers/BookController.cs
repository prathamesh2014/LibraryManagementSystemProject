using Application.IServices;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;
        private readonly IWebHostEnvironment _env;

        public BookController(IBookService bookService, ICategoryService categoryService,
            IAuthorService authorService, IWebHostEnvironment env)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _authorService = authorService;
            _env = env;
        }

        public async Task<IActionResult> Index(string? searchTitle, int? categoryId, int? authorId)
        {
            var books = await _bookService.SearchAsync(searchTitle, categoryId, authorId);

            ViewBag.Categories = await _categoryService.GetAllAsync();
            ViewBag.Authors = await _authorService.GetAllAsync();
            ViewBag.SearchTitle = searchTitle;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedAuthorId = authorId;

            return View(books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new BookViewModel
            {
                Categories = await _categoryService.GetAllAsync(),
                Authors = await _authorService.GetAllAsync()
            };
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(BookViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await _categoryService.GetAllAsync();
                vm.Authors = await _authorService.GetAllAsync();
                return View(vm);
            }

            var book = new Book
            {
                Title = vm.Title,
                ISBN = vm.ISBN,
                Publisher = vm.Publisher,
                PublishYear = vm.PublishYear,
                TotalCopies = vm.TotalCopies,
                CategoryId = vm.CategoryId,
                AuthorId = vm.AuthorId
            };

            if (vm.ImageFile != null)
            {
                book.ImagePath = await SaveImageAsync(vm.ImageFile);
            }

            await _bookService.AddAsync(book);
            TempData["Success"] = "Book added successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
                return NotFound();

            var vm = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                ISBN = book.ISBN,
                Publisher = book.Publisher,
                PublishYear = book.PublishYear,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                IsDeleted = book.IsDeleted,
                ImagePath = book.ImagePath,
                CategoryId = book.CategoryId,
                AuthorId = book.AuthorId,
                Categories = await _categoryService.GetAllAsync(),
                Authors = await _authorService.GetAllAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await _categoryService.GetAllAsync();
                vm.Authors = await _authorService.GetAllAsync();
                return View(vm);
            }

            var book = await _bookService.GetByIdAsync(vm.BookId);
            if (book == null)
                return NotFound();

            book.Title = vm.Title;
            book.ISBN = vm.ISBN;
            book.Publisher = vm.Publisher;
            book.PublishYear = vm.PublishYear;
            book.TotalCopies = vm.TotalCopies;
            book.CategoryId = vm.CategoryId;
            book.AuthorId = vm.AuthorId;

            if (vm.ImageFile != null)
            {
                book.ImagePath = await SaveImageAsync(vm.ImageFile);
            }

            await _bookService.UpdateAsync(book);
            TempData["Success"] = "Book updated successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
                return NotFound();

            return View(book);   // <-- single Book, not a List<Book>
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookService.DeleteAsync(id);
            TempData["Success"] = "Book deleted successfully";
            return RedirectToAction("Index");
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "books");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/images/books/" + uniqueFileName;
        }
        public async Task<IActionResult> Deleted()
        {
            var books = await _bookService.GetDeletedAsync();
            return View(books);
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            await _bookService.RestoreAsync(id);
            TempData["Success"] = "Book restored successfully";
            return RedirectToAction("Deleted");
        }
    }
}
