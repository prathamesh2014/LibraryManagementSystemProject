using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class IssueBookRepository : IIssueBookRepository
    {
        private readonly LibraryDbContext _context;

        public IssueBookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<IssueBook>> GetAllAsync()
        {
            return await _context.IssueBooks
                .Include(i => i.Book)
                .Include(i => i.Member)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<IssueBook?> GetByIdAsync(int id)
        {
            return await _context.IssueBooks
                .Include(i => i.Book)
                .Include(i => i.Member)
                .FirstOrDefaultAsync(i => i.IssueId == id);
        }

        public async Task<List<IssueBook>> GetActiveIssuesByMemberAsync(int memberId)
        {
            return await _context.IssueBooks
                .Include(i => i.Book)
                .Where(i => i.MemberId == memberId && i.Status == "Issued")
                .ToListAsync();
        }

        public async Task AddAsync(IssueBook issueBook)
        {
            _context.IssueBooks.Add(issueBook);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IssueBook issueBook)
        {
            _context.IssueBooks.Update(issueBook);
            await _context.SaveChangesAsync();
        }
    }
}
