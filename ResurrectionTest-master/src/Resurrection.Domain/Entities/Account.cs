using Resurrection.Domain.Enum;

namespace Resurrection.Domain.Entities
{
    public class Account
    {
        public string Email { get; private set; }
        public string Password { get; private set; }
        public Plataforma TypeAccount { get; private set; }

        public Account(string email, string password, Plataforma typeAccount)
        {
            Email = email;
            Password = password;
            TypeAccount = typeAccount;
        }
    }
}
