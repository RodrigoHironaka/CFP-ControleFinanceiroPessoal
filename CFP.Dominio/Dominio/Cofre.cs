using CFP.Dominio.Dominio;
using Dominio.ObejtoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Cofre : Base
    {
        //O que sobrar do balanço pode ser inserido, transferido, depositado, etc., aqui no cofre(banco);
        public virtual Caixa Caixa { get; set; }
        public virtual Banco Banco { get; set; }

        //esse valor deve ser somado ou subtraido ao que ja tem na conta definida pelo usuario;
        public virtual Double Valor { get; set; }
        public virtual SituacaoCofre Situacao { get; set; }
    }
}
