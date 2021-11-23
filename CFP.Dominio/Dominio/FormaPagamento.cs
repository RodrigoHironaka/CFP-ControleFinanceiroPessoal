using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using Dominio.ObjetoValor;
using System;

namespace Dominio.Dominio
{
    public class FormaPagamento : Base
    {
        public override string ToString()
        {
            return Nome;
        }
        public virtual Int32 QtdParcelas { get; set; }
        public virtual Int32 DiasParaVencimento { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual SimNao TransacoesBancarias { get; set; }
        public virtual SimNao UsadoParaCompras { get; set; }
    }
}
