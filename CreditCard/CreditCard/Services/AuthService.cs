using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using CreditCard.Models; 
namespace CreditCard.Services
{
    public class AuthService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

      // AuthService.cs - Shtoni log
public bool VerifyPassword(string passwordHash, string password)
{
    try
    {
        Console.WriteLine($"Verifying password. Hash length: {passwordHash?.Length}, Password length: {password?.Length}");
        
        // Kontrolloni nëse hash është valid BCrypt
        if (string.IsNullOrEmpty(passwordHash) || !passwordHash.StartsWith("$2"))
        {
            Console.WriteLine("Invalid hash format");
            return false;
        }
        
        var result = BCrypt.Net.BCrypt.Verify(password, passwordHash);
        Console.WriteLine($"BCrypt verification result: {result}");
        return result;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in VerifyPassword: {ex.Message}");
        return false;
    }
}
    }
}
