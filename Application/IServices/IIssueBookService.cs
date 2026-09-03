using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IIssueBookService
    {
        Task<List<IssueBook>> GetAllAsync();
        Task<IssueBook?> GetByIdAsync(int id);
        Task<string> IssueBookAsync(int bookId, int memberId);
        Task<decimal> ReturnBookAsync(int issueId);
    }
}
