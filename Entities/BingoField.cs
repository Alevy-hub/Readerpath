namespace Readerpath.Entities
{
    public class BingoField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Bingo Bingo { get; set; }
        public BookAction? BookAction { get; set; }
        public bool IsChecked { get; set; }
        public int Index { get; set; }
    }
}
