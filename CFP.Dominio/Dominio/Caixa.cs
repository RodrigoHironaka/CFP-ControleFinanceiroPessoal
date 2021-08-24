using Dominio.ObejtoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Caixa
    {
        public virtual Int64 ID { get; set; }
        public virtual DateTime DataAbertura { get; set; }
        public virtual DateTime DataFechamento { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Double TotalEntrada { get; set; }
        public virtual Double TotalSaida { get; set; }
        public virtual Double BalançoFinal { get; set; }
        public virtual SituacaoCaixa Situacao { get; set; }

    }
}
