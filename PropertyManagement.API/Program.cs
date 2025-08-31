using PropertyManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

var app = builder.ConfigureServices();

app.UsePipeline();

app.Run();