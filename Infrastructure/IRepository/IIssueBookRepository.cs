using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IRepository
{
    public interface IIssueBookRepository
    {
        Task<List<IssueBook>> GetAllAsync();
        Task<IssueBook?> GetByIdAsync(int id);
        Task<List<IssueBook>> GetActiveIssuesByMemberAsync(int memberId);
        Task AddAsync(IssueBook issueBook);
        Task UpdateAsync(IssueBook issueBook);
    }
}
