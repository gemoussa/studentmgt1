using UsingDapper.Infrastucture.Repositories;
using UsingDapper.Infrastucture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DapperContext and repositories
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<SetupRepository>();
builder.Services.AddScoped<DataGeneratorRepository>(); 

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Create tables and insert data on startup
using (var scope = app.Services.CreateScope())
{
    var setupRepository = scope.ServiceProvider.GetRequiredService<SetupRepository>();
    await setupRepository.CreateTablesAsync();

    var dataGenerator = scope.ServiceProvider.GetRequiredService<DataGeneratorRepository>();
    await dataGenerator.InsertRandomDataAsync();
}

app.Run();
