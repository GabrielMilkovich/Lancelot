using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resurrection.Domain.Entities
{
    public class EnriqueceResult : BaseEntity
    {
        public Dictionary<Account, Checker> Result { get; private set; }


        public EnriqueceResult() { }

        public override void RealizaValidacoes()
        {
            throw new NotImplementedException();
        }

        public void AtribuiEnriqueceResult(Dictionary<Account, Checker> keys)
            => Result = keys;
    }
}
