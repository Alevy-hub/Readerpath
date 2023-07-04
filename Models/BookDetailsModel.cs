using Readerpath.Entities;

namespace Readerpath.Models
{
    public class BookDetailsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public List<EditionModel> Editions { get; set; }
        public List<ActionModel> Actions { get; set; }
    }

    public class EditionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Readerpath.Entities.Type Type { get; set; }
        public int? Pages { get; set; }
        public int? Duration { get; set; }
        public string AddedBy { get; set; }
        public string User { get; set; }
    }

    public class ActionModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Publisher { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public float? Rating { get; set; }
    }
}
