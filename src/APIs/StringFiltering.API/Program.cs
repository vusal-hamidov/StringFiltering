using StringFiltering.API.CompositionRoot;
using StringFiltering.Application.CompositionRoot;
using StringFiltering.Infrastructure.CompositionRoot;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services
    .AddControllers();

services
    .AddSwagger()
    .AddValidation()
    .AddApplication(configuration)
    .AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(configuration);
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();