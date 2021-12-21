using Application.Authentication;
using Application.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add the controllers
builder.Services.AddControllers();

// Adding Identity  
builder.Services.AddApplicationIdentity(builder.Configuration);

// Adding Authentication  
builder.Services.AddApplicationAuthentication(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect the user to an HTTPS connection
app.UseHttpsRedirection();

// Auth the user and fetch his user identity and claims
app.UseAuthentication();

// Authrize the uer to valid routes and endpoints
app.UseAuthorization();

// Map the request to the right controller
app.MapControllers();

// Run the app
app.Run();
