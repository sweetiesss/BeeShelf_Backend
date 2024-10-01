using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Kros.Extensions;
using Microsoft.Extensions.Configuration;
using Ocelot.Values;
using ApiGateway.Config;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();


var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL").Value!.ToString();

builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultURL),
    new EnvironmentCredential()
);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration,
  (o) =>
  {
      o.GenerateDocsForGatewayItSelf = true;
  });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureDownstreamHostAndPortsPlaceholders(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwaggerForOcelotUI();
app.UseSwagger();
app.UseSwaggerUI();

// Other middleware
var client = new SecretClient(new Uri(keyVaultURL), new EnvironmentCredential());

var apikey = client.GetSecret("BeeStore-Apikey").Value.Value;
if (!string.IsNullOrEmpty(apikey))
{
    app.Use(async (context, next) =>
    {
        context.Request.Headers.Add("X-Api-Key", apikey);
        await next.Invoke();
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.UseOcelot();

app.Run();