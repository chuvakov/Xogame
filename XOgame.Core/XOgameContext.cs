using Microsoft.EntityFrameworkCore;
using XOgame.Core.Models;

namespace XOgame.Core;

public class XOgameContext : DbContext 
{
    public DbSet<Game> Games { get; set; }
    public DbSet<GameProgress> GameProgresses { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGame> UserGames { get; set; }
    
    public XOgameContext(DbContextOptions options) : base(options)
    {}
    
    //Тонкие настройки связок таблиц
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasOne(g => g.Room)
            .WithMany(r => r.Games)
            .OnDelete(DeleteBehavior.SetNull);
    }
}