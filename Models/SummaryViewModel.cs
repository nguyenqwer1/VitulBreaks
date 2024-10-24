namespace VirtualBreaks.Models
{
    public class SummaryViewModel
    {
        public long Timestamp { get; set; }
        public string Name { get; set; }
        public IFormFile BackgroundImage { get; set; }
        public string BackgroundImagePath { get; set; }
        public string SelectedTimeZoneId { get; set; }
        public string SelectedTimeZoneName { get; set; } // Thêm thuộc tính này
    }
}