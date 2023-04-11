using System.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using IssueTracker.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ITUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // AddDefaultUi and AddDefaultTokenProviders do come with AddIdentity so we have to add them
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

///custom Services
builder.Services.AddScoped<IITRolesService, ITRolesService>();
builder.Services.AddScoped<IITCompanyInfoService, ITCompanyInfoService>();
builder.Services.AddScoped<IITProjectService, ITProjectService>();
builder.Services.AddScoped<IITTicketService, ITTicketService>();
builder.Services.AddScoped<IITTicketHistoryService, ITTicketHistoryService>();
builder.Services.AddScoped<IEmailSender, ITEmailService>();
builder.Services.AddScoped<IITNotificationService, ITNotificationService>();
builder.Services.AddScoped<IITInviteService, ITInviteService>();
builder.Services.AddScoped<IITFileService, ITFileService>();

// Configure mail settings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();