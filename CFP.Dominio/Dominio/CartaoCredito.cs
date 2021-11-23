using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class CartaoCredito : Base
    {
        public virtual Int64 Codigo { get; set; }
        public virtual Int64? NumeroDocumento { get; set; }
        public virtual Int64? QtdParcelas { get; set; }
        public virtual DateTime? DataEmissao { get; set; }
        public virtual Decimal? Valor { get; set; }
        public virtual TipoConta TipoConta { get; set; }
        public virtual TipoPeriodo TipoPeriodo { get; set; }
        public virtual SituacaoConta SituacaoFatura { get; set; }
        public virtual SubGrupoGasto SubGrupoGasto { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }
        public virtual String Observacao { get; set; }
    }
}
