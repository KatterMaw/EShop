using EShop.Application;
using EShop.Data;
using EShop.Domain.Model;
using FlakeId;
using Microsoft.EntityFrameworkCore;

namespace EShop.Services.UsersProviders;

// Don't really like this implementation...
public sealed class CachedDbUsersProvider : UsersProvider
{
	public CachedDbUsersProvider(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<User> GetUser(Id id)
	{
		if (_usersByIds.TryGetValue(id, out var cachedUser))
			return cachedUser;
		var user = await _dbContext.Users.FirstAsync(user => user.Id == id);
		AddToCache(user);
		return user;
	}

	public async Task<AuthenticatedUser> GetUser(string name)
	{
		if (_usersByNames.TryGetValue(name, out var cachedUser))
			return cachedUser;
		var user = await _dbContext.AuthenticatedUsers.FirstAsync(user => user.Name == name);
		AddToCache(user);
		return user;
	}

	public async Task<User?> GetOptionalUser(Id id)
	{
		if (_usersByIds.TryGetValue(id, out var cachedUser))
			return cachedUser;
		var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
		if (user != null)
			AddToCache(user);
		return user;
	}

	public async Task<AuthenticatedUser?> GetOptionalUser(string name)
	{
		if (_usersByNames.TryGetValue(name, out var cachedUser))
			return cachedUser;
		var user = await _dbContext.AuthenticatedUsers.FirstOrDefaultAsync(user => user.Name == name);
		if (user != null)
			AddToCache(user);
		return user;
	}

	private readonly AppDbContext _dbContext;
	private readonly Dictionary<Id, User> _usersByIds = new();
	private readonly Dictionary<string, AuthenticatedUser> _usersByNames = new();

	private void AddToCache(User user)
	{
		_usersByIds.Add(user.Id, user);
	}

	private void AddToCache(AuthenticatedUser user)
	{
		_usersByIds.Add(user.Id, user);
		_usersByNames.Add(user.Name, user);
	}
}