using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniInventory.API.Data;
using MiniInventory.API.Middlewares;
using MiniInventory.API.Services; // <-- THÊM DÒNG NÀY ĐỂ GỌI ĐƯỢC THƯ MỤC SERVICES
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// --- BẮT ĐẦU LẮP CAMERA SERILOG ---
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .WriteTo.Console() // Hiển thị nhật ký ra cửa sổ Output/Terminal
        .WriteTo.File("Logs/nhat-ky-he-thong-.txt", rollingInterval: RollingInterval.Day) // Tự động lưu ra file .txt (mỗi ngày 1 file mới)
);
// --- KẾT THÚC LẮP CAMERA ---

// 1. Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =======================================================
// BƯỚC ĐĂNG KÝ DEPENDENCY INJECTION (BÁO CÁO GIÁM ĐỐC)
// =======================================================
// Mỗi khi có người cần IProductService, hãy cử Đầu bếp ProductService ra phục vụ
builder.Services.AddScoped<IProductService, ProductService>();
// =======================================================
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Đăng ký tác vụ chạy ngầm kiểm tra kho hàng
builder.Services.AddHostedService<InventoryCheckJob>();
// Đăng ký Cô Lao Công thành tác vụ chạy ngầm vô thời hạn
builder.Services.AddHostedService<LogCleanupService>();

// 2. Cấu hình Xác thực JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// 3. Cấu hình Controllers và XỬ LÝ VÒNG LẶP JSON (Quan trọng)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // 🚀 MA THUẬT XÓA BỎ LỚP MÀN ẢO VÀ SP KHÔNG XÁC ĐỊNH
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Thêm đoạn này để cấp phép cho Angular (Cổng 4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();

// 4. Cấu hình Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new global::Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = global::Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = global::Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Dán đoạn mã Token vào ô bên dưới."
    });

    options.AddSecurityRequirement(new global::Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new global::Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new global::Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = global::Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Bật tính năng CORS 
app.UseCors("AllowAngular");

// Thầy đã xóa đoạn code bị lặp ở đây đi cho gọn
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SystemLogMiddleware>();

app.MapControllers();

app.Run();