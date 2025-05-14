using FinancialProducts.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// �K�[�A�Ȩ�e��
// �K�[ MVC
builder.Services.AddControllersWithViews();

// �K�[����XSS�������\��
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

// ���U�ƾڳX�ݪA��
builder.Services.AddSingleton<DataAccess>();

// �t�m��x
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// �c�����ε{��
var app = builder.Build();

// �t�m HTTP �ШD�޹D
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// �ϥ�HTTPS���w�V
app.UseHttpsRedirection();

// �R�A���
app.UseStaticFiles();

// ����
app.UseRouting();

// ���v
app.UseAuthorization();

// �]�w���I
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// �B�����ε{��
app.Run();