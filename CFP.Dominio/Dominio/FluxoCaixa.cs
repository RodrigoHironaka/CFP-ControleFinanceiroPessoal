using CFP.Dominio.ObjetoValor;
using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class FluxoCaixa : Base
    {
        public override string ToString()
        {
            return Nome;
        }
        public virtual Decimal Valor { get; set; }
        public virtual EntradaSaida TipoFluxo { get; set; }
        public virtual Conta Conta { get; set; }
        public virtual Caixa Caixa { get; set; }
        public virtual Usuario UsuarioLogado { get; set; }
        public virtual FormaPagamento FormaPagamento { get; set; }

    }
}
