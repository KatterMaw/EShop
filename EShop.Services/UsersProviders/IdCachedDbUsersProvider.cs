using EShop.Application;
using EShop.Data;
using EShop.Domain.Model;
using EShop.Services.Extensions;
using FlakeId;
using Microsoft.EntityFrameworkCore;

namespace EShop.Services.UsersProviders;

/// <summary>
/// Uses FindAsync to get the user by id, uses First[OrDefault]Async to get the user by name
/// </summary>
public sealed class IdCachedDbUsersProvider : UsersProvider
{
	public IdCachedDbUsersProvider(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public Task<User> GetUser(Id id) => _dbContext.Users.FindAsync(id).AsTask().GuardIsNotNull();

	public Task<AuthenticatedUser> GetUser(string name) =>
		_dbContext.AuthenticatedUsers.FirstAsync(user => user.Name == name);

	public Task<User?> GetOptionalUser(Id id) => _dbContext.Users.FindAsync(id).AsTask();

	public Task<AuthenticatedUser?> GetOptionalUser(string name) =>
		_dbContext.AuthenticatedUsers.FirstOrDefaultAsync(user => user.Name == name);

	private readonly AppDbContext _dbContext;
}