using EShop.Domain.Model;

namespace EShop.Application;

public interface UserLastVisitUpdater
{
	Task UpdateLastVisit(User user);
}