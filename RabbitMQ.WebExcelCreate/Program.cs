using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.WebExcelCreate.Hubs;
using RabbitMQ.WebExcelCreate.Models;
using RabbitMQ.WebExcelCreate.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSingleton(sp => new ConnectionFactory()
{ Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true }
);
builder.Services.AddDbContext<AppDbContext>(opt =>
{
	opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
{
	opt.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
	var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	var userManager =  scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

	appDbContext.Database.Migrate();

	if (!appDbContext.Users.Any())
	{
		userManager.CreateAsync(new IdentityUser()
		{
			UserName = "deneme",
			Email = "deneme@outlook.com"
		}, "Password12*").Wait();

		userManager.CreateAsync(new IdentityUser()
		{
			UserName = "deneme2",
			Email = "deneme2@outlook.com"
		}, "Password12*").Wait();
	}
}
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
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<MyHub>("/myhub");
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
