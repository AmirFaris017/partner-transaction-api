using FluentValidation;        
using PartnerTransactionAPI.Validators; 
using System.Reflection;
using System.IO;
using log4net;
using log4net.Config;
using PartnerTransactionAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<PartnerTransactionAPI.Services.SignatureService>();
builder.Services.AddSingleton<PartnerTransactionAPI.Services.DiscountService>();

builder.Services.AddValidatorsFromAssemblyContaining<TransactionRequestValidator>();
builder.Services.AddScoped<IValidator<TransactionRequest>, TransactionRequestValidator>();

// Configure log4net
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

builder.WebHost.UseUrls("http://+:5000");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new {
    status  = "Partner Transaction API is running!",
    version = "1.0.0",
    endpoints = new[] {
        "POST /api/submittrxmessage"
    }
}));

app.Run();
