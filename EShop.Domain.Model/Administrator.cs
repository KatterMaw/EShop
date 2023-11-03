using FlakeId;

namespace EShop.Domain.Model;

public sealed class Administrator
{
	public Id Id { get; private set; }
	public AuthenticatedUser User { get; private set; }
	public bool HasPrivileges { get; set; }

	public Administrator(AuthenticatedUser user, bool hasPrivileges = true)
	{
		User = user;
		HasPrivileges = hasPrivileges;
	}

	private Administrator()
	{
		User = null!;
	}
}