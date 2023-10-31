using EShop.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace EShop.Data;

public sealed class AppDbContext : DbContext
{
	public DbSet<User> Users { get; set; } = null!;
	public DbSet<GuestUser> GuestUsers { get; set; } = null!;
	public DbSet<AuthenticatedUser> AuthenticatedUsers { get; set; } = null!;
	
	public AppDbContext(DbContextOptions options) : base(options)
	{
	}
}