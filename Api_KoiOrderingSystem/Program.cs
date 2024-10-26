using Api_KoiOrderingSystem.MiddleWares;
using Common.DTO.Flight;
using Common.DTO.KoiFish;
using Common.DTO.Order;
using DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Service;
using Service.Interface;
using Service.Interfaces;
using Service.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBreedService, BreedService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IKoiService, KoiService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();
builder.Services.AddScoped<IStorageProvinceService, StorageProvinceService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IKoiFarmService, KoiFarmService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IShippingFeeService, ShippingFeeService>();
builder.Services.AddScoped<IOrderStorageService, OrderStorageService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IAirportService , AirportService>();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var key = builder.Configuration.GetValue<string>("ApiSetting:Secret");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
    })
    .AddOData(options =>
    {
        ODataConventionModelBuilder odataBuilder = new ODataConventionModelBuilder();
        odataBuilder.EntitySet<GetAllKoiDTO>("all-koi");
        odataBuilder.EntitySet<GetAllKoiDTO>("all-koi-koifarm");
        odataBuilder.EntitySet<GetAllHistoryOrderDTO>("all-history-order");
        odataBuilder.EntitySet<GetAllHistoryOrderDTO>("all-customer-history-order");
        odataBuilder.EntitySet<GetAllFarmHistoryOrderDTO>("all-farm-history-order");
        odataBuilder.EntitySet<GetAllFarmHistoryOrderDTO>("all-storage-history-order");
        odataBuilder.EntitySet<GetAllFlightDTO>("flights");
        odataBuilder.EntitySet<ShipperDto>("shippers");
        odataBuilder.EntitySet<PolicyDTO>("Policy");
        odataBuilder.EntitySet<Airport>("all-airports");
        options.AddRouteComponents("odata", odataBuilder.GetEdmModel());
        options.Select().Expand().Filter().OrderBy().Count().SetMaxTop(100);
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOriginPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAnyOriginPolicy");

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
