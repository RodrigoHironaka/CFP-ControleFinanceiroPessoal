using CFP.Dominio.Dominio;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class GrupoGasto : Base
    {
        public virtual Situacao Situacao { get; set; }
    }
}
