using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Banco : Base
    {
        public override string ToString()
        {
            return String.Format("{0} {1} - {2}", Nome, TipoContaBanco, PessoaBanco);
        }
        public virtual TipoContaBanco TipoContaBanco { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual SimNao UsarValorParaCalculos { get; set; }
        public virtual Pessoa PessoaBanco { get; set; }
    }
}
