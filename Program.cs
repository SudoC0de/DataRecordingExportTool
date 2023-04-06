using ElectronNET.API;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseElectron(args);
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.MapControllerRoute(
    name:"Record",
    pattern: "{controller=Record}/{action=Index}/{id?}");

app.MapControllerRoute(
    name:"Export",
    pattern: "{controller=Export}/{action=Index}/{id?}");

//app.Run();

// Open the Electron-Window here
await Electron.WindowManager.CreateWindowAsync();
await app.StartAsync();

app.WaitForShutdown();
