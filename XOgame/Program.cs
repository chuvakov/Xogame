using Microsoft.EntityFrameworkCore;
using XOgame.Core;
using XOgame.Services;
using XOgame.Services.Account;
using XOgame.Services.Game;
using XOgame.Services.Player;
using XOgame.Services.Room;
using XOgame.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<XOgameContext>(x => x.UseNpgsql(connectionString));

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

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("localhost");

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<RoomHub>("/hubs/room");
});
app.Run();