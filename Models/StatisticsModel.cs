namespace Readerpath.Models
{
    public class StatisticsModel
    {
        public string year { get; set; }
        public List<bool> months { get; set; }
        public Dictionary<string, bool> MonthsDict { get; set; }
    }
}
