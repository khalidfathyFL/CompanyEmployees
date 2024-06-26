// the type of variable here is web application builder which is used in four main functions
// adding configuration like a environment variable or a json file
// registering a service like database or unit of work 
// to configure logging at console or debug
// it has two properties one for host which access the i host builder to configures the host to run as windows service or anything else
// the other property is the webhost  which access i web host builder  which configure the web host to run on kestral for example and run on specific ports

using CompanyEmployees;
using CompanyEmployees.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Utility;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using Service.DataShaping;
using Shared.DataTransferObjects;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// getting nlog configuration fromthe nlog.config file
LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "nlog.config"));

// Download Microsoft.AspNetCore.Mvc.Newtonsoftjson package to support request body conversion    to a PatchDocument once we send our request
//  we are creating a local function. This function configures support for JSON Patch using 
//  Newtonsoft.Json while leaving the other formatters unchanged. 
NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
  new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
    .Services.BuildServiceProvider()
    .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
    .OfType<NewtonsoftJsonPatchInputFormatter>().First();

// registering the action filter
builder.Services.AddScoped<ValidationFilterAttribute>();
// registering data shaping for employee
builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

// changing the controllers place 
builder.Services.AddControllers(config=>
{
  config.RespectBrowserAcceptHeader = true;
  // to tell that this media type is not accepted and not returning the default value which is json
  config.ReturnHttpNotAcceptable=true;
  // to use patch http request we place the json patch input formatter at the index zero in the input formatters
  config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
}).AddXmlDataContractSerializerFormatters().AddCustomCSVFormatter().AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);
// to enable sending empty body
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.SuppressModelStateInvalidFilter = true;
});
// adding the custom media type
builder.Services.AddCustomMediaTypes();
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureSerivceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
// handling error using iExceptionhandler in dot net  8
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// media type validationa ction filter
builder.Services.AddScoped<ValidateMediaTypeAttribute>();
// hateoas implementation
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();
// this build return one web application which implements i host that is used to start and stop the host 
// and it implements i application builder which is used to build the middleware pipeline
// and IEndPointRoute Builder to add endpoints in our app
var app = builder.Build();

// this is why we use  iexception handler
//var logger = app.Services.GetRequiredService<ILoggerManager>();
//app.ConfigureExceptionHandler(logger);
app.UseExceptionHandler(opt => { });
if (app.Environment.IsProduction())
  //adds the http strict transport security (hsts)
  //HSTS: HTTP Strict Transport Security is a web security policy mechanism that helps protect websites against man-in-the-middle attacks such as protocol downgrade attacks and cookie hijacking. It tells browsers to only interact with the site using HTTPS even if users attempt to access it using HTTP.

  app.UseHsts();


// it is used to add the middleware for the redirection from http to https
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
  ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors("CorsPolicy");

// adds the authorization middle ware using IApplicationBuilder
app.UseAuthorization();

// adds the endpoints through the IEndPointRouteBuilder
app.MapControllers();

app.Run();