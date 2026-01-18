using Resurrection.Domain.Enum;

namespace Resurrection.Domain.Entities
{
    public abstract class Checker
    {
        protected Checker(Login login, Plataforma plataforma)
        {
            Login = login;
            Plataforma = plataforma;
        }

        public Login Login { get; private set; }
        public Plataforma Plataforma { get; private set; }

    }
}
