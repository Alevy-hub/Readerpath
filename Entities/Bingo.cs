namespace Readerpath.Entities
{
    public class Bingo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsFinished { get; set; }

		public Bingo()
		{
			DateAdded = DateTime.Now;
		}
	}
}
