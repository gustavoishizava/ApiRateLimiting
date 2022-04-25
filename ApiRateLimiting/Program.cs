using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Rate limiting configuration
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.Services.Configure<IpRateLimitOptions>(c =>
{
    c.EnableEndpointRateLimiting = true;
    c.StackBlockedRequests = false;
    c.HttpStatusCode = StatusCodes.Status429TooManyRequests;
    c.RealIpHeader = "X-Real-IP";
    c.GeneralRules = new(){
       new(){
           Endpoint = "*",
           Period = "5s",
           Limit = 2
       }
   };
});
#endregion

var app = builder.Build();

#region Rate limiting configuration
app.UseIpRateLimiting();
#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
