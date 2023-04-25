using ChaTai.Blazor;
using ChaTai.Core;

GlobalVariable.Platform = PlatformType.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor()
    .AddHubOptions(op =>
        op.MaximumReceiveMessageSize = 100 * 1024 * 1024 //10M
    );

builder.Services.AddChaTaiSetup(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
