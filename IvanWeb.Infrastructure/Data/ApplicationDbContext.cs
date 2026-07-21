using IvanWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IvanWeb.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Nossas tabelas (DbSets)
    public DbSet<PlayerProfile> PlayerProfiles => Set<PlayerProfile>();
    public DbSet<Trophy> Trophies => Set<Trophy>();
    public DbSet<UnlockedTrophy> UnlockedTrophies => Set<UnlockedTrophy>();
    public DbSet<StoreItem> StoreItems => Set<StoreItem>();
    public DbSet<PurchaseHistory> PurchaseHistories => Set<PurchaseHistory>();
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}