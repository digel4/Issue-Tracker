using System.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using IssueTracker.Services;

using IssueTracker.Services.Factories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// default database connection
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
//                        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//custom database connection using dataUtility
// Because DataUtility is a static class we don't have to instantiate it. We immediately have access to its properties
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>

    options.UseNpgsql(connectionString,
        // adding option to allow query splitting.
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    ).EnableSensitiveDataLogging()
);


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ITUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    //Adding the ClaimsPrincipalFactory
    .AddClaimsPrincipalFactory<ITUserClaimsPrincipalFactory>()
    // AddDefaultUi and AddDefaultTokenProviders do come with AddIdentity so we have to add them
    .AddDefaultUI()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

///custom Services
builder.Services.AddScoped<IRolesService, ITRolesService>();
builder.Services.AddScoped<ICompanyInfoService, ITCompanyInfoService>();

builder.Services.AddScoped<IProjectService, ITProjectService>();
builder.Services.AddScoped<ITicketService, ITTicketService>();
builder.Services.AddScoped<ITicketHistoryService, ITTicketHistoryService>();
builder.Services.AddScoped<IEmailSender, ITEmailService>();
builder.Services.AddScoped<INotificationService, ITNotificationService>();
builder.Services.AddScoped<IInviteService, ITInviteService>();
builder.Services.AddScoped<IFileService, ITFileService>();
builder.Services.AddScoped<IITLookUpService, ITLookUpService>();

// Configure mail settings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


var app = builder.Build();

await DataUtility.ManageDataAsync(app);

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