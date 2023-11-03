using CommunityToolkit.Diagnostics;
using EShop.Application;
using EShop.Data;
using EShop.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EShop.Services;

public sealed class DbUserLastVisitUpdater : UserLastVisitUpdater
{
	public DbUserLastVisitUpdater(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task UpdateLastVisit(User user)
	{
		var userEntryState = _dbContext.Entry(user).State;
		if (userEntryState is EntityState.Deleted)
			ThrowHelper.ThrowArgumentException(nameof(user), "User is marked for deletion");
		if (userEntryState is EntityState.Detached)
		{
			_logger.Warning("User {User} isn't tracked by database context", user);
			_dbContext.Attach(user);
		}
		var oldLastVisit = user.LastVisit;
		user.UpdateLastVisit();
		var entriesUpdated = await _dbContext.SaveChangesAsync();
		Guard.IsGreaterThan(entriesUpdated, 0);
		_logger.Information("User {User} last visit updated from {OldLastVisit} to {NewLastVisit}", user, oldLastVisit, user.LastVisit);
	}
	
	private readonly ILogger _logger = Log.ForContext<DbUserLastVisitUpdater>();
	private readonly AppDbContext _dbContext;
}