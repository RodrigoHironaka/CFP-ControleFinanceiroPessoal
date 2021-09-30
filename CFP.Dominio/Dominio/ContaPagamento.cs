using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class ContaPagamento
    {
        public override string ToString()
        {
            return " ";
        }
        public virtual Int64 ID { get; set; }
        public virtual Int32 Numero { get; set; }
        public virtual Decimal ValorParcela { get; set; }
        public virtual DateTime? DataVencimento { get; set; }
        public virtual DateTime? DataPagamento { get; set; }
        public virtual Decimal JurosPorcentual { get; set; }
        public virtual Decimal JurosValor { get; set; } 
        public virtual Decimal DescontoPorcentual { get; set; }
        public virtual Decimal DescontoValor { get; set; }
        public virtual Decimal ValorReajustado { get; set; }
        public virtual Decimal ValorPago { get; set; }
        public virtual SituacaoConta SituacaoParcelas { get; set; }
        public virtual FormaPagamento FormaPagamento { get; set; }
        public virtual Conta Conta { get; set; }

        //Na forma de pagamento pode haver situações onde uma pessoa transfere o dinheiro para outra pessoa pagar;
        //Neste caso, a forma de pagamento não pode subtrair da pessoa que esta cadastrada a conta,
        //teria que verificar quem é a pessoa que recebeu a transferencia
        //e subtrair dela, sem alterar o dono da conta;
    }
}
