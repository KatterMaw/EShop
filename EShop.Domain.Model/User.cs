using FlakeId;

namespace EShop.Domain.Model;

public abstract class User
{
	public Id Id { get; private set; }
	public DateTime LastVisit { get; private set; }

	protected User()
	{
		Id = Id.Create();
		UpdateLastVisit();
	}
	
	public void UpdateLastVisit()
	{
		LastVisit = DateTime.UtcNow;
	}

	public override string ToString() => $"#{Id}";
}