using Resurrection.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resurrection.Domain.Constantes
{
    public class ArquivosConstantes
    {
        public static readonly List<string> ArquivosNecessarios = new List<string>()
        {
            {"Accounts" },
            {"Ouro" },
            {Login.Logado.ToString() },
            {Login.ErroCredenciais.ToString() },
            {Login.TooManyRequest.ToString() },
            {Login.ContaBanida.ToString() },
            {Login.ErroAutentificacao.ToString() },
            {Login.ErroBuscaToken.ToString() },
            {Login.ErroDesconhecido.ToString() },
            {Login.SessaoBloqueada.ToString() },
            {Login.Nenhum.ToString() },
        };
    }
}
