using CFP.Dominio.Dominio;
using Dominio.Interfaces;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Conta : Base
    {
        public virtual Int64 Codigo { get; set; }
        public virtual TipoConta TipoConta { get; set; }
        public virtual TipoPeriodo TipoPeriodo { get; set; }
        public virtual SituacaoConta Situacao { get; set; }
        public virtual DateTime? DataEmissao { get; set; }
        public virtual Decimal? ValorTotal { get; set; }
        public virtual Int64? QtdParcelas { get; set; }
        public virtual Int64? NumeroDocumento { get; set; }
        public virtual SubGrupoGasto SubGrupoGasto { get; set; }
        public virtual FormaPagamento FormaCompra { get; set; }
        public virtual Pessoa Pessoa { get; set; }
        public virtual Usuario UsuarioCriacao { get; set; }
        public virtual string Observacao { get; set; }
        public virtual IList<ContaPagamento> ContaPagamentos { get; set; }
        public virtual IList<ContaArquivo> ContaArquivos { get; set; }

    }
}
