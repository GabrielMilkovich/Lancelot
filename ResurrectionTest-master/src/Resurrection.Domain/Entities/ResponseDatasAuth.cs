using Resurrection.Ressurrection.Domain.Base;
using System.Text.RegularExpressions;

namespace Resurrection.Domain.Entities
{
    public class ResponseDatasAuth : BaseResponseDatas
    {
        public ResponseDatasAuth(string conteudo)
        {
            FmHF = Regex.Match(conteudo, @"name=""fmHF"" id=""fmHF"" action=""([^""]+)""").Groups[1].Value;
            Pprid = Regex.Match(conteudo, @"type=""hidden"" name=""pprid"" id=""pprid"" value=""([^""]+)""").Groups[1].Value;
            Nap = Regex.Match(conteudo, @"type=""hidden"" name=""NAP"" id=""NAP"" value=""([^""]+)""").Groups[1].Value;
            ANon = Regex.Match(conteudo, @"type=""hidden"" name=""ANON"" id=""ANON"" value=""([^""]+)""").Groups[1].Value;
            T = Regex.Match(conteudo, @"type=""hidden"" name=""t"" id=""t"" value=""([^""]+)""").Groups[1].Value;

            AtribuiData($@"pprid={Pprid}&NAP={Nap}&ANON={ANon}&t={T}");
        }

        public string FmHF { get; private set; }
        public string Pprid { get; private set; }
        public string Nap { get; private set; }
        public string ANon { get; private set; }
        public string T { get; private set; }
    }
}
