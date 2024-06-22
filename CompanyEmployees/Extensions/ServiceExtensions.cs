using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using Service.Contracts;

namespace CompanyEmployees.Extensions
{
  public static class ServiceExtensions
  {
    /// <summary>
    /// adding cors configuration
    /// </summary>
    /// <param name="services"></param>
    /// allow any origin , with origin is to know from whom you can accept requests
    /// allow any method with methods is to allow for example get or post only
    /// allow any header , with headers is to allow specific headers like accept and content type
    public static void ConfigureCors(this IServiceCollection services) => services.AddCors(
      opt =>
      {
        opt.AddPolicy("CorsPolicy", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().
          WithExposedHeaders("X-Pagination"));
          
      });

    /// <summary>
    /// Configuring internet information services(IIS) integration
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureIISIntegration(this IServiceCollection services) =>
      services.Configure<IISOptions>(
        options =>
        {
          // AuthenticationDisplayName:
          // Sets the display name shown to users on login pages. The default is null.
          // Use this to provide a user-friendly name for the authentication scheme.
          // options.AuthenticationDisplayName = null; // or set to a specific display name

          // AutomaticAuthentication:
          // If true, the middleware will automatically set HttpContext.User.
          // If false, the middleware will only provide an identity when explicitly requested by the AuthenticationScheme.
          // Note: Windows Authentication must be enabled in IIS for this to work.
          // options.AutomaticAuthentication = true; // or set to false for explicit authentication requests

          // ForwardClientCertificate:
          // Populates the ITLSConnectionFeature if the MS-ASPNETCORE-CLIENTCERT request header is present.
          // Use this to forward client certificate information to the application.
          // options.ForwardClientCertificate = true; // or set to false if not using client certificates

        });

    /// <summary>
    /// Configuring logger
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureLoggerService(this IServiceCollection services) =>
      services.AddSingleton<ILoggerManager, LoggerManager>();

    /// <summary>
    /// Configure Repository Manager
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
      services.AddScoped<IRepositoryManager, RepositoryManager>();

    /// <summary>
    /// Configuring Service Manager
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureSerivceManager(this IServiceCollection services) =>
      services.AddScoped<IServiceManager, ServiceManager>();

    /// <summary>
    /// Configuring Database
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void ConfigureSqlContext(this IServiceCollection services,
      IConfiguration configuration) =>
      services.AddDbContext<RepositoryContext>(opts =>
        opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

    // adding a created csv formatter to the output formatters
    public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
      builder.AddMvcOptions(config => config.OutputFormatters.Add(new
        CsvOutputFormatter()));

    public static void AddCustomMediaTypes(this IServiceCollection services)
    {
      services.Configure<MvcOptions>(config =>
      {
        var systemTextJsonOutputFormatter = config.OutputFormatters
          .OfType<SystemTextJsonOutputFormatter>()?
          .FirstOrDefault();
        if (systemTextJsonOutputFormatter != null)
        {
          systemTextJsonOutputFormatter.SupportedMediaTypes
            .Add("application/vnd.codemaze.hateoas+json");
        }
        var xmlOutputFormatter = config.OutputFormatters
          .OfType<XmlDataContractSerializerOutputFormatter>()?
          .FirstOrDefault();
        if (xmlOutputFormatter != null)
        {
          xmlOutputFormatter.SupportedMediaTypes
            .Add("application/vnd.codemaze.hateoas+xml");
        }
      });
    }
  }


}