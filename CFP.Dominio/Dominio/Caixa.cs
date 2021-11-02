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
        public virtual Int64 Id { get; set; }
        public virtual Int64 Codigo { get; set; }
        public virtual DateTime DataAbertura { get; set; }
        public virtual DateTime DataFechamento { get; set; }
        public virtual Decimal ValorInicial { get; set; }
        public virtual SituacaoCaixa Situacao { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Conta Conta { get; set; }
        public virtual Usuario UsuarioAbertura { get; set; }
        public virtual Usuario UsuarioFechamento { get; set; }
        public virtual IList<FluxoCaixa> FluxoCaixas { get; set; }

        public virtual Decimal TotalEntrada
        {
            get
            {
                return FluxoCaixas.Where(x => x.TipoFluxo == EntradaSaida.Entrada).Sum(x => x.Valor);
            }
        }

        public virtual Decimal TotalSaida
        {
            get
            {
                return FluxoCaixas.Where(x => x.TipoFluxo == EntradaSaida.Saída).Sum(x => x.Valor);
            }
        }
        public virtual Decimal BalancoFinal
        {
            get
            {
                return (ValorInicial + TotalEntrada) - TotalSaida;
            }
        }
    }
}
