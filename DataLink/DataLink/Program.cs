using DataLink.Services;
using DataLink.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure web host
builder.WebHost.UseUrls("http://localhost:5000");

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Configure endpoints
app.MapRazorPages();

// Add endpoint to trigger email
app.MapPost("/api/send-digest", async (IEmailService emailService, [FromBody] ScrapedData newsData) =>
{
    try
    {
        await emailService.SendNewsDigestAsync(newsData);
        return Results.Ok(new { message = "News digest email sent successfully" });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
