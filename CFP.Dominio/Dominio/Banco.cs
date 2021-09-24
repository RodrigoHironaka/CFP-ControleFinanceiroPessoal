using CFP.Dominio.Dominio;
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
            return Nome;
        }
        public virtual TipoContaBanco TipoContaBanco { get; set; }
        public virtual Situacao Situacao { get; set; }
    }
}
