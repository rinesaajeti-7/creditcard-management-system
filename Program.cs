using CreditCard.Data;
using CreditCard.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Lexo connection string (Render ose lokal)
var connectionString = GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CreditCardService>();

// Konfigurimi i JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "DefaultKeyAtLeast16Chars";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "CreditCardApp",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "CreditCardApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Apliko migrimet në mënyrë të sigurt
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
catch (Exception ex)
{
    // Në Render, log-ja e tregon gabimin, por ne nuk e ndalojmë aplikacionin
    Console.WriteLine($"Migration failed: {ex.Message}");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

// Porti dinamik për Render (dhe fallback 8080 për Docker)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

static string GetConnectionString(IConfiguration config)
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        try
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');
            if (userInfo.Length != 2)
                throw new FormatException("DATABASE_URL format invalid: missing user:password");

            var user = userInfo[0];
            var password = userInfo[1];
            return new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.Port > 0 ? uri.Port : 5432,
                Username = user,
                Password = password,
                Database = uri.AbsolutePath.TrimStart('/'),
                SslMode = SslMode.Require,
                 TrustServerCertificate = true 
            }.ToString();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to parse DATABASE_URL: {ex.Message}", ex);
        }
    }
    // Për Docker lokal ose zhvillim
    var localConn = config.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(localConn))
        throw new InvalidOperationException("No connection string found. Set DefaultConnection or DATABASE_URL.");
    return localConn;
}