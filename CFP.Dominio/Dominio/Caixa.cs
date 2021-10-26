using CFP.Dominio.Dominio;
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
        public Caixa()
        {
            FluxoCaixas = new List<FluxoCaixa>();
        }
        public override string ToString()
        {
            return Codigo.ToString();
        }
        public virtual Int64 ID { get; set; }
        public virtual Int64 Codigo { get; set; }
        public virtual DateTime DataAbertura { get; set; }
        public virtual DateTime DataFechamento { get; set; }
        public virtual Decimal TotalEntrada { get; set; }
        public virtual Decimal TotalSaida { get; set; }
        public virtual Decimal BalançoFinal { get; set; }
        public virtual SituacaoCaixa Situacao { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Conta Conta { get; set; }
        public virtual IList<FluxoCaixa> FluxoCaixas { get; set; }
    }
}
