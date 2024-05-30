namespace Readerpath.Models
{
    public class StatisticsModel
    {
        public string year { get; set; }
        public bool IsNextYearAvailable { get; set; }
        public bool IsPrevYearAvailable { get; set; }
        public List<bool> months { get; set; }
        public Dictionary<string, bool> MonthsDict { get; set; }
    }
}
