using System.ComponentModel.DataAnnotations;

namespace Readerpath.Entities
{
    public class YearChallenge
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Count { get; set; }
        public bool CongratsShowed { get; set; }
        public string User { get; set; }
    }
}
