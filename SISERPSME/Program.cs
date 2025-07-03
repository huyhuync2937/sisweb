using Blazored.LocalStorage;
using SisData.Data;
using SisData.Dataaccess;
using SisData.Service;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using DevExpress.XtraCharts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using DevExpress.Xpo.Logger.Transport;
using Microsoft.JSInterop;
using SisData.Model;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);
//Add services to the container.

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<ISqldataacceess, Sqldataacceess>();
builder.Services.AddSingleton<IDbManager, DbManager>();
builder.Services.AddScoped<Statemanagerment>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddSingleton<ISysVarManager, SysVarManager>();
builder.Services.AddSingleton<IOptionsManager, OptionsManager>();
builder.Services.AddSingleton<ISisTypeManager, SisTypeManager>();
builder.Services.AddSingleton<IDocumentProvider, DocumentProvider>();
builder.Services.AddScoped<IDxWindowService, DxWindowService>();
builder.Services.AddSingleton<ICommandManager, CommandManager>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddSingleton<SisGlobals>();
builder.Services.AddScoped<XmlStringLocalizer>();
builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressBlazorReporting();
builder.Services.AddDevExpressServerSideBlazorReportViewer();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddLocalization(options => options.ResourcesPath = "Lang");
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
builder.Services.Configure<DevExpress.Blazor.Configuration.GlobalOptions>(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
});


builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");    
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseDevExpressBlazorReporting();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("vi-VN");

app.Run();