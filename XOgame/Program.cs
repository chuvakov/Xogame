using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using XOgame.Authentication;
using XOgame.Core;
using XOgame.Services;
using XOgame.Services.Account;
using XOgame.Services.Emoji;
using XOgame.Services.Game;
using XOgame.Services.Player;
using XOgame.Services.Room;
using XOgame.Services.Setting;
using XOgame.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "XOgame Api",
        Version = "v1"
    });
    
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите ваш JWT в поле ввода",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    { 
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<XOgameContext>(x => x.UseNpgsql(connectionString));

#region Authentication
//сформирован объект настроек на основе файла json appsettings.json => AuthSettings
var authSettings = builder.Configuration.GetSection("AuthSettings")
    .Get<AuthSettings>(opt => opt.BindNonPublicProperties = true);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //добавляем сервис аут JWT
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false; //не обязат. передавать данные HTTPS запроса
        opt.TokenValidationParameters = new TokenValidationParameters
            //Формируем настройки валидации (проверка ранее выданого токена)
        {
            ValidateIssuer = true,
            ValidIssuer = authSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = authSettings.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authSettings.SymmetricSecurityKey
        };
    });

#endregion

builder.Logging.AddLog4Net("log4net.config");
builder.Services.AddCors(options =>
{
    options.AddPolicy("localhost", builder =>
    {
        builder
            .SetIsOriginAllowed(x => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});
builder.Services.AddSignalR();

#region DI

//Invers of contral(паттерн)
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IRoomService, RoomService>();
builder.Services.AddTransient<IPlayerService, PlayerService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<ISettingService, SettingService>();
builder.Services.AddTransient<IEmojiService, EmojiService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("localhost");

app.UseAuthentication();//идентификация добавили в том числе для JWT токена
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    
    #region Хабы
    endpoints.MapHub<RoomHub>("/hubs/room");
    endpoints.MapHub<GameHub>("/hubs/game");
    endpoints.MapHub<ChatHub>("/hubs/chat");
    #endregion

});

// Directory.CreateDirectory("Avatars");
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Avatars")),
    RequestPath = "/Avatars"
});
app.Run();