using LowFareAirDotnet.Infrastructure;

namespace LowFareAirDotnet.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews();

        //TODO : Extension metody do DI do innego pliku dla .Web
        builder.Services.RegisterInfrastructure(builder.Configuration);

        var app = builder.Build();

        //TODO : DODAJ JWT NA AUTH

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
