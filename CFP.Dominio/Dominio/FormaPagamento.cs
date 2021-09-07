using CFP.Dominio.Dominio;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class FormaPagamento : Base
    {
        public virtual Int32 QtdParcelas { get; set; }
        public virtual Int32 DiasParaVencimento { get; set; }
        public virtual Situacao Situacao { get; set; }
    }
}
