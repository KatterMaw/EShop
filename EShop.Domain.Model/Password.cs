using CommunityToolkit.Diagnostics;
using Isopoh.Cryptography.Argon2;
using Serilog;

namespace EShop.Domain.Model;

public sealed class Password
{
	private static string CreateSalt(int size)
	{
		var buffer = new byte[size];
		Random.Shared.NextBytes(buffer);
		var saltString = Convert.ToBase64String(buffer);
		Log.ForContext<Password>().Debug("Generated salt: {Salt}", saltString);
		return saltString;
	}
	
	public Password(string password)
	{
		Salt = CreateSalt(16);
		Hash = Argon2.Hash(password + Salt);
		Guard.IsTrue(Argon2.Verify(password, Hash));
	}

	public string Hash { get; private set; }
	public string Salt { get; private set; }

	public bool Verify(string password) => Argon2.Verify(Hash, password + Salt);

	private Password(string hash, string salt)
	{
		Hash = hash;
		Salt = salt;
	}
}