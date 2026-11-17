using ShareholderRegInfo.Portal.Services;
using ShareholderRegInfo.Portal.XML;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddOptions<RegisterOptions>()
    .BindConfiguration(RegisterOptions.SectionName)
    .Validate(o => !string.IsNullOrWhiteSpace(o.FilePath), $"{RegisterOptions.SectionName}:FilePath must be set.")
    .ValidateOnStart();

builder.Services.AddSingleton<IRegisterFormParser, RegisterFormParser>();
builder.Services.AddScoped<IRegisterService, RegisterService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
