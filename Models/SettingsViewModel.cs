using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace VirtualBreaks.Models
{
    public class SettingsViewModel
    {
        public DateTime CurrentTime { get; set; }
        public List<BreakViewModel> DefaultBreaks { get; set; }
        public string NewBreakName { get; set; }
        public int NewBreakDuration { get; set; }
        public List<SelectListItem> AvailableTimeZones { get; set; }
        public string SelectedTimeZoneId { get; set; }
        public int SelectedBreakIndex { get; set; }

        // New properties for separate Duration and FixedEndTime
        public int Duration { get; set; }
        public TimeSpan FixedEndTime { get; set; }
    }

    public class BreakViewModel
    {
        public string Name { get; set; }
        public int Duration { get; set; }
        public string Icon { get; set; }
        public bool Enabled { get; set; }
        public bool Selected { get; set; }
        public int Index { get; set; }
        public TimeSpan? FixedEndTime { get; set; }

        // Thêm thuộc tính để xác định loại break
        public bool IsDurationBreak { get; set; }
    }


    public class BreakUpdateModel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public string FixedEndTime { get; set; } // Cần là string để có thể xử lý null hoặc không hợp lệ
    }


}
