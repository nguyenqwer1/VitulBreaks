using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using VirtualBreaks.Models;

public class SettingsController : Controller
{
    private static SettingsViewModel model;

    static SettingsController()
    {
        model = new SettingsViewModel
        {
            CurrentTime = DateTime.Now,
            DefaultBreaks = new List<BreakViewModel>
            {
                new BreakViewModel { Name = "Coffee Break", Duration = 15, Icon = "coffee-cup", Enabled = true, Selected = false, Index = 0 },
                new BreakViewModel { Name = "Lunch Break", FixedEndTime =  DateTime.Now.TimeOfDay, Icon = "burger", Enabled = true, Selected = false, Index = 1 }
            },
            NewBreakName = string.Empty,
            NewBreakDuration = 0,
            AvailableTimeZones = GetTimeZones(),
            SelectedTimeZoneId = TimeZoneInfo.Local.Id,
            Duration = 0,
            FixedEndTime = TimeSpan.Zero
        };
    }

    private static List<SelectListItem> GetTimeZones()
    {
        return TimeZoneInfo.GetSystemTimeZones()
            .Select(tz => new SelectListItem { Value = tz.Id, Text = tz.DisplayName })
            .ToList();
    }

    public ActionResult Index()
    {
        model.CurrentTime = DateTime.Now;
        return View(model);
    }

    public ActionResult StartBreak(int index)
    {
        if (index >= 0 && index < model.DefaultBreaks.Count)
        {
            var selectedBreak = model.DefaultBreaks[index];
            DateTime endTime;

            if (selectedBreak.FixedEndTime.HasValue)
            {
                var today = DateTime.Today;
                endTime = today.Add(selectedBreak.FixedEndTime.Value);
                if (endTime < DateTime.Now)
                {
                    endTime = endTime.AddDays(1);
                }
            }
            else
            {
                endTime = DateTime.UtcNow.AddMinutes(selectedBreak.Duration);
            }

            var timestamp = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();
            return RedirectToAction("Index", "Summary", new { timestamp = timestamp, name = selectedBreak.Name });
        }
        return RedirectToAction("Index");
    }

    public ActionResult SelectBreak(int index)
    {
        if (index >= 0 && index < model.DefaultBreaks.Count)
        {
            model.DefaultBreaks.ForEach(b => b.Selected = false);
            model.DefaultBreaks[index].Selected = true;
            model.SelectedBreakIndex = index;
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateBreak([FromBody] BreakUpdateModel updateModel)
    {
        if (updateModel.Index >= 0 && updateModel.Index < model.DefaultBreaks.Count)
        {
            var breakToUpdate = model.DefaultBreaks[updateModel.Index];
            breakToUpdate.Name = updateModel.Name;
            breakToUpdate.Duration = updateModel.Duration;

            if (!string.IsNullOrEmpty(updateModel.FixedEndTime) && TimeSpan.TryParse(updateModel.FixedEndTime, out var fixedEndTime))
            {
                breakToUpdate.FixedEndTime = fixedEndTime;
            }
            else
            {
                breakToUpdate.FixedEndTime = null;
            }

            return Json(new { success = true });
        }

        return Json(new { success = false });
    }

    [HttpPost]
    public IActionResult ChangeTimeZone(string timeZoneId)
    {
        if (!string.IsNullOrEmpty(timeZoneId))
        {
            model.SelectedTimeZoneId = timeZoneId;
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }

    [HttpPost]
    public IActionResult SaveSettings(SettingsViewModel viewModel)
    {
        model.Duration = viewModel.Duration;
        model.FixedEndTime = viewModel.FixedEndTime;
        return RedirectToAction("Index");
    }
}
