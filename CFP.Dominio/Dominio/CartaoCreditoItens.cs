using CFP.Dominio.ObjetoValor;
using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class CartaoCreditoItens : Base
    {
        public virtual Decimal Valor { get; set; }
        public virtual Int32 Qtd { get; set; }
        public virtual DateTime? DataCompra { get; set; }
        public virtual SubGrupoGasto SubGrupoGasto { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual CartaoCredito CartaoCredito { get; set; }
    }
}
