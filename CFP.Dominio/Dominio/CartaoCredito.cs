using CFP.Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class CartaoCredito
    {
        public virtual Int64 Id { get; set; }
        public virtual Int32 MesReferencia { get; set; }
        public virtual Int32 AnoReferencia { get; set; }
        public virtual Decimal ValorFatura { get; set; }
        public virtual SituacaoFatura SituacaoFatura { get; set; }
        public virtual IList<CartaoCreditoItens> CartaoCreditos { get; set; }
    }
}
