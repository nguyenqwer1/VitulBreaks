using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.IO;
using System.Threading.Tasks;
using VirtualBreaks.Models;
using VitualBreaks.Models;

namespace VirtualBreaks.Controllers
{
    public class SummaryController : Controller
    {
        private readonly VitualBreaksContext _context;

        public SummaryController(VitualBreaksContext context)
        {
            _context = context;
        }

        public IActionResult Index(long timestamp, string name)
        {
            var model = new SummaryViewModel
            {
                Timestamp = timestamp, // Keeping it in milliseconds
                Name = name
            };

            var backgroundImage = _context.BackgroundImages.OrderByDescending(b => b.Id).FirstOrDefault(); // Lấy ảnh mới nhất
            if (backgroundImage != null)
            {
                model.BackgroundImagePath = $"data:image/png;base64,{Convert.ToBase64String(backgroundImage.ImageData)}";
            }

            return View(model);
        }


        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UploadBackground(SummaryViewModel model)
        {
            if (model.BackgroundImage != null && model.BackgroundImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.BackgroundImage.CopyToAsync(memoryStream);
                    var backgroundImage = new BackgroundImage
                    {
                        FileName = model.BackgroundImage.FileName,
                        ImageData = memoryStream.ToArray()
                    };

                    _context.BackgroundImages.Add(backgroundImage);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Log hoặc hiển thị thông báo lỗi nếu ảnh không hợp lệ
            }

            return RedirectToAction("Index", new { timestamp = model.Timestamp, name = model.Name });
        }


        public IActionResult GenerateQRCode(long timestamp, string name)
        {
            string url = $"{Request.Scheme}://{Request.Host}/mobile/{timestamp}/{name}";
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                return File(qrCodeBytes, "image/png");
            }
        }

        public JsonResult Countdown(long timestamp)
        {
            var nowTicks = DateTime.UtcNow.Ticks;
            var targetTimeTicks = timestamp * TimeSpan.TicksPerMillisecond;

            var distance = targetTimeTicks - nowTicks;
            if (distance > 0)
            {
                var hours = (int)((distance % TimeSpan.TicksPerDay) / TimeSpan.TicksPerHour);
                var minutes = (int)((distance % TimeSpan.TicksPerHour) / TimeSpan.TicksPerMinute);
                var seconds = (int)((distance % TimeSpan.TicksPerMinute) / TimeSpan.TicksPerSecond);

                var countdown = $"{hours.ToString().PadLeft(2, '0')}:{minutes.ToString().PadLeft(2, '0')}:{seconds.ToString().PadLeft(2, '0')}";
                return Json(countdown);
            }
            else
            {
                return Json("00:00:00");
            }
        }
    }
}
