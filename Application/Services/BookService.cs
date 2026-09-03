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
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Book book)
        {
            book.AvailableCopies = book.TotalCopies;
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateAsync(Book book)
        {
            await _bookRepository.UpdateAsync(book);
        }

        public async Task DeleteAsync(int id)
        {
            await _bookRepository.SoftDeleteAsync(id);
        }

        public async Task<List<Book>> SearchAsync(string? title, int? categoryId, int? authorId)
        {
            return await _bookRepository.SearchAsync(title, categoryId, authorId);
        }
        public async Task<List<Book>> GetDeletedAsync()
        {
            return await _bookRepository.GetDeletedAsync();
        }

        public async Task RestoreAsync(int id)
        {
            await _bookRepository.RestoreAsync(id);
        }
    }
}
