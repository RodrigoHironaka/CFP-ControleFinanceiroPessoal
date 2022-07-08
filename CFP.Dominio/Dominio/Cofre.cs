using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
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
        public virtual Int64 Codigo { get; set; }
        public virtual Caixa Caixa { get; set; }
        public virtual Banco Banco { get; set; }
        public virtual Decimal Valor { get; set; }
        public virtual DateTime? DataRegistro { get; set; }
        public virtual FormaPagamento TransacoesBancarias { get; set; }
        public virtual EntradaSaida Situacao { get; set; }
    }
}
