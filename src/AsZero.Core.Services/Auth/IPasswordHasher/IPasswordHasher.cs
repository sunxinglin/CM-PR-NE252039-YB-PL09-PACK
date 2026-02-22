using System.Collections.Generic;

namespace AsZero.Core.Services.Auth
{
    public interface IPasswordHasher
    {
        string ComputeHash(string plaintext, string salt);
    }

}
