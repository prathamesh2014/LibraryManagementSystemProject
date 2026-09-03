using Domain.Entities;

namespace UI.Models
{
    public class DashboardViewModel
    {
        public int TotalBooks { get; set; }
        public int TotalMembers { get; set; }
        public int IssuedBooksCount { get; set; }
        public int ReturnedBooksCount { get; set; }
        public int PendingBooksCount { get; set; }

        public List<IssueBook> RecentIssues { get; set; } = new List<IssueBook>();
    }
}
