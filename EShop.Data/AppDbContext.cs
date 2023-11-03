using EShop.Data.Configuration;
using EShop.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace EShop.Data;

public sealed class AppDbContext : DbContext
{
	public DbSet<User> Users { get; set; } = null!;
	public DbSet<GuestUser> GuestUsers { get; set; } = null!;
	public DbSet<AuthenticatedUser> AuthenticatedUsers { get; set; } = null!;
	public DbSet<Administrator> Administrators { get; set; } = null!;

	public AppDbContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new AuthenticatedUserConfiguration());
		modelBuilder.ApplyConfiguration(new AdministratorConfiguration());
	}
}