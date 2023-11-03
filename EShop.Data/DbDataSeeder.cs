using EShop.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EShop.Data;

public static class DbDataSeeder
{
	public static Task SeedData(this AppDbContext context)
	{
		return context.EnsureAnyAdministratorExists();
	}

	private static async Task EnsureAnyAdministratorExists(this AppDbContext context)
	{
		var hasAnyAdministratorAccount = await context.Administrators.AnyAsync(admin => admin.HasPrivileges);
		if (hasAnyAdministratorAccount)
			return;
		var occupiedAdministratorNames = await context.Administrators.Select(admin => admin.User.Name).ToListAsync();
		var administratorName = "admin";
		var index = 0;
		while (occupiedAdministratorNames.Contains(administratorName))
		{
			administratorName = $"admin{index}";
			index++;
		}
		Administrator administrator = new(new AuthenticatedUser(administratorName, "admin"));
		context.Administrators.Add(administrator);
		await context.SaveChangesAsync();
		Log.Information("Administrator account created: {AdministratorName}", administratorName);
	}
}