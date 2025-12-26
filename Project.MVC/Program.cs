

using Project.MVC.Services;


var builder = WebApplication.CreateBuilder(args);


// Add services

builder.Services.AddControllersWithViews();

// Session
builder.Services.AddSession();

// Http
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// ApiService
builder.Services.AddScoped<ApiService>();

var app = builder.Build();

// Middleware pipeline

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
