using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using OfficeOpenXml;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // for excel download
        var licenseContext = builder.Configuration["EPPlus:Excel:LicenseContext"];
        // Set the EPPlus license context
        if (!string.IsNullOrEmpty(licenseContext) && Enum.TryParse(licenseContext, out LicenseContext context))
        {
            ExcelPackage.LicenseContext = context;
        }

        builder.Services.AddControllersWithViews();

        
        builder.Services.AddScoped<ICountriesService, CountriesService>();
        builder.Services.AddScoped<IPersonsService, PersonsService>();

        builder.Services.AddDbContext<PersonsDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });



        var app = builder.Build();

        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // look at rotativia.io for more formal documentation
        Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();

        app.Run();
    }
}