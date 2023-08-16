using System.Configuration;

namespace Readerpath.Entities
{
    public class TBRBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TBR TBR { get; set; }
        public Edition? LinkedEdition { get; set; }
        public bool IsRead { get; set; }
    }
}
