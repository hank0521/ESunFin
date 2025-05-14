using FinancialProducts.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 添加服務到容器
// 添加 MVC
builder.Services.AddControllersWithViews();

// 添加防止XSS攻擊的功能
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

// 註冊數據訪問服務
builder.Services.AddSingleton<DataAccess>();

// 配置日誌
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 構建應用程序
var app = builder.Build();

// 配置 HTTP 請求管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 使用HTTPS重定向
app.UseHttpsRedirection();

// 靜態文件
app.UseStaticFiles();

// 路由
app.UseRouting();

// 授權
app.UseAuthorization();

// 設定端點
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 運行應用程序
app.Run();