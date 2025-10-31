using LineInsight.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LineInsight.Infrastructure;
public sealed class LineDbContext : DbContext
{
    public LineDbContext(DbContextOptions<LineDbContext> options) : base(options) { }
    public DbSet<PlcEndpoint> Plcs => Set<PlcEndpoint>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<TagDef> Tags => Set<TagDef>();
    public DbSet<TagReading> Readings => Set<TagReading>();
    protected override void OnModelCreating(ModelBuilder b)
        => b.Entity<TagReading>().HasIndex(x => new { x.TagDefId, x.UtcTs });
}
