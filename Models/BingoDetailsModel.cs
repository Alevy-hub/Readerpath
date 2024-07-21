using Readerpath.Entities;

namespace Readerpath.Models
{
    public class BingoDetailsModel
    {
        public int BingoId { get; set; }
        public string Title { get; set; }
        public List<BingoField> bingoFields { get; set; }
    }
}
