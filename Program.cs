using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VitualBreaks.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<VitualBreaksContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("VitualBreaks"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
void ConfigureServices(IServiceCollection services)
{
    services.AddSession(options =>
    {
        // Thiết lập các tùy chọn cho Session
        options.Cookie.Name = ".VirtualBreaks.Session";
        options.IdleTimeout = TimeSpan.FromMinutes(60); // Thời gian timeout cho Session
        options.Cookie.HttpOnly = true; // Chỉ cho phép truy cập HTTP
        options.Cookie.IsEssential = true; // Đánh dấu Session là bắt buộc
    });
    app.UseSession();

    app.UseRouting();
    services.AddControllersWithViews();
    // Các dịch vụ khác...
}