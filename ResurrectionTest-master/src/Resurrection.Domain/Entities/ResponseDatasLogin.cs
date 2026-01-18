using Resurrection.Ressurrection.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Resurrection.Domain.Entities
{
    public class ResponseDatasLogin : BaseResponseDatas
    {
        public ResponseDatasLogin(string conteudo, Account account)
        {
            Ppft = conteudo.Split(@"<input type=\""hidden\"" name=\""PPFT\""")[1]
           .Split(@"value")[1]
           .Split(@"=\""-")[1]
           .Split(@"\""/>"",")[0];

            LogUrl = conteudo.Split(@"""urlPost"":""")[1]
                            .Split(@""",")[0];

            Uaid = conteudo.Split("uaid=")[1]
                           .Split(@"""")[0];

            AtribuiData($"ps=2&psRNGCDefaultType=&psRNGCEntropy=&psRNGCSLK=&canary=&ctx=&hpgrequestid=&PPFT=-{Ppft}=PassportRN&NewUser=1&FoundMSAs=&fspost=0&i21=0&CookieDisclosure=0&IsFidoSupported=1&isSignupPost=0&isRecoveryAttemptPost=0&i13=0&login={account.Email}&loginfmt={account.Email}&type=11&LoginOptions=3&lrt=&lrtPartition=&hisRegion=&hisScaleUnit=&cpr=0&passwd={account.Password}");
        }

        public string Ppft { get; private set; }
        public string LogUrl { get; private set; }
        public string Uaid { get; private set; }
        public string PPFT { get; private set; }
        public string UrlPost { get; private set; }
        public string ErrText { get; private set; }


        public string AtribuiPPFT(string conteudo)
            => PPFT = Regex.Match(conteudo, @"sFT\"":""([^""]+)""").Groups[1].Value;

        public string AtribuiUrlPost(string conteudo)
            => UrlPost = Regex.Match(conteudo, @"urlPost"":""([^""]+)""").Groups[1].Value;

        public string AtribuiErrText(string conteudo)
            => ErrText = Regex.Match(conteudo, @"sErrTxt"":""([^""]+)""").Groups[1].Value;

        public ValidationResult Validacao()
        {
            if (ErrText == "You've tried to sign in too many times with an incorrect account or password.")
                return new ValidationResult(ErrText);

            return new ValidationResult(string.Empty);
        }

    }
}
