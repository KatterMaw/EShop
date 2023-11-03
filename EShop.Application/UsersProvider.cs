using EShop.Domain.Model;
using FlakeId;

namespace EShop.Application;

public interface UsersProvider
{
	Task<User> GetUser(Id id);
	Task<AuthenticatedUser> GetUser(string name);
	Task<User?> GetOptionalUser(Id id);
	Task<AuthenticatedUser?> GetOptionalUser(string name);
}