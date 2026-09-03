using Application.IServices;
using Domain.Entities;
using Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class IssueBookService : IIssueBookService
    {
        private readonly IIssueBookRepository _issueBookRepository;
        private readonly IBookRepository _bookRepository;

        public IssueBookService(IIssueBookRepository issueBookRepository, IBookRepository bookRepository)
        {
            _issueBookRepository = issueBookRepository;
            _bookRepository = bookRepository;
        }

        public async Task<List<IssueBook>> GetAllAsync()
        {
            return await _issueBookRepository.GetAllAsync();
        }

        public async Task<IssueBook?> GetByIdAsync(int id)
        {
            return await _issueBookRepository.GetByIdAsync(id);
        }

        public async Task<string> IssueBookAsync(int bookId, int memberId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
                return "Book not found";

            if (book.AvailableCopies <= 0)
                return "No copies available for this book";

            var issueBook = new IssueBook
            {
                BookId = bookId,
                MemberId = memberId,
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Status = "Issued"
            };

            await _issueBookRepository.AddAsync(issueBook);

            book.AvailableCopies -= 1;
            await _bookRepository.UpdateAsync(book);

            return "Success";
        }

        public async Task<decimal> ReturnBookAsync(int issueId)
        {
            var issueBook = await _issueBookRepository.GetByIdAsync(issueId);

            if (issueBook == null || issueBook.Status == "Returned")
                return 0;

            issueBook.ReturnDate = DateTime.Now;

            decimal fine = 0;
            if (issueBook.ReturnDate > issueBook.DueDate)
            {
                int lateDays = (issueBook.ReturnDate.Value - issueBook.DueDate).Days;
                fine = lateDays * 10;
            }

            issueBook.FineAmount = fine;
            issueBook.Status = "Returned";

            await _issueBookRepository.UpdateAsync(issueBook);

            var book = await _bookRepository.GetByIdAsync(issueBook.BookId);
            if (book != null)
            {
                book.AvailableCopies += 1;
                await _bookRepository.UpdateAsync(book);
            }

            return fine;
        }
    }
}
