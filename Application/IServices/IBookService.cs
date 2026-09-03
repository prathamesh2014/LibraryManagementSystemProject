using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IBookService
    {
        Task<List<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<List<Book>> SearchAsync(string? title, int? categoryId, int? authorId);
        Task<List<Book>> GetDeletedAsync();
        Task RestoreAsync(int id);
        
    }
}
