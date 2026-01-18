using Resurrection.Domain.Entities;
using Resurrection.Domain.Enum;
using Resurrection.Domain.Interfaces;
using System.Net.Mail;

namespace Resurrection.Domain.Services
{
    public class AccountService : IAccountService
    {
        public List<Account> BuscaContasBlocoNotas()
        {
            string caminhoArquivo = Path.Combine(AppContext.BaseDirectory, "accounts.txt");

            var contas = new List<Account>();

            foreach (var linha in File.ReadAllLines(caminhoArquivo))
            {
                if (string.IsNullOrWhiteSpace(linha))
                    continue;

                var partes = linha.Split(':');
                if (partes.Length != 2)
                    continue;

                string email = partes[0].Trim();
                string senha = partes[1].Trim();

                if (!EmailValido(email))
                    continue;

                contas.Add(new Account(email, senha, Plataforma.Microsoft));
            }

            return contas;
        }

        #region Métodos auxiliares 
        private bool EmailValido(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        #endregion Métodos auxiliares 

    }
}
