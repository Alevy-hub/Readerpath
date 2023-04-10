namespace Readerpath.Entities
{
	public class Publisher
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string AddedBy { get; set; }
		public DateTime DateAdded { get; set; }
		public bool isAccepted { get; set; }

		public Publisher()
		{
			DateAdded = DateTime.Now;
		}
	}
}
