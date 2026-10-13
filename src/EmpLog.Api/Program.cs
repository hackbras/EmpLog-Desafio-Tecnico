using EmpLog.Api.Extensions;
using EmpLog.Api.Middlewares;
using EmpLog.Application;
using EmpLog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "EmpLog Motoristas API", Version = "v1" });
});

var caminhoBanco = Path.Combine(builder.Environment.ContentRootPath, "emplog.db");
builder.Services.AddApplication();
builder.Services.AddInfrastructure($"Data Source={caminhoBanco}");
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.MigrarBancoAsync();
app.Run();
