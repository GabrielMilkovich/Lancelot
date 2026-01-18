using Core;
using Resurrection.Domain.Enum;

namespace DarkPassenger.Domain.Entities
{
    public class LoginResult : BaseEntity
    {
        public Login Situacao { get; private set; }

        public LoginResult(Login situacao)
        {
            Situacao = situacao;

            RealizaValidacoes();
        }

        public override void RealizaValidacoes()
        {
            if (Situacao == Login.ContaBanida)
                AdicionaNotificacao(new ValidationError("Conta banida."));

            if (Situacao == Login.ErroCredenciais)
                AdicionaNotificacao(new ValidationError("Conta com email ou senha incorreta."));

            if (Situacao == Login.TooManyRequest)
                AdicionaNotificacao(new ValidationError("Sistema microsoft com lentidão"));
        }
    }
}
