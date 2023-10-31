﻿namespace EShop.Domain.Model;

public sealed class AuthenticatedUser : User
{
	public string Name { get; set; }
	public Password Password { get; set; }

	public AuthenticatedUser(string name, string password)
	{
		Name = name;
		Password = new Password(password);
	}

	private AuthenticatedUser()
	{
		Name = null!;
		Password = null!;
	}
}