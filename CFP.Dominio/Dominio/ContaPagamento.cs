using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class ContaPagamento
    {
        public virtual Int64 ID { get; set; }
        public virtual Conta Conta { get; set; }
        public virtual Double JurosPorcentual { get; set; }
        public virtual Double JurosValor { get; set; } 
        public virtual Double DescontoPorcentual { get; set; }
        public virtual Double DescontoValor { get; set; }
        public virtual Double ValorReajustado { get; set; }

        //Na forma de pagamento pode haver situações onde uma pessoa transfere o dinheiro para outra pessoa pagar;
        //Neste caso, a forma de pagamento não pode subtrair da pessoa que esta cadastrada a conta, teria que verificar quem é a pessoa que recebeu a transferencia e subtrair dela, sem alterar o dono da conta;
        public virtual FormaPagamento FormaPagamento { get; set; }
    }
}
