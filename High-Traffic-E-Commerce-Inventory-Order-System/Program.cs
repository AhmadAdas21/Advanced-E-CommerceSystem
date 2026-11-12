using High_Traffic_E_Commerce_Inventory_Order_System.DataBase;
using High_Traffic_E_Commerce_Inventory_Order_System.interfaces;
using High_Traffic_E_Commerce_Inventory_Order_System.Services;
using High_Traffic_E_Commerce_Inventory_Order_System.Exceptions;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IOrderServices, OrderServices>();
builder.Services.AddScoped<ICustomerServices, CustomerServices>();

builder.Services.AddScoped<IProductServices, ProductServices>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<OrderConflictExceptionHandler>();

builder.Services.AddProblemDetails();
var app = builder.Build();
app.UseExceptionHandler();
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
