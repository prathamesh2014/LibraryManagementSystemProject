using Domain.Entities;

namespace UI.Models
{
    public class IssueBookViewModel
    {
        public int BookId { get; set; }
        public int MemberId { get; set; }

        public List<Book> Books { get; set; } = new List<Book>();
        public List<Member> Members { get; set; } = new List<Member>();
    }
}
