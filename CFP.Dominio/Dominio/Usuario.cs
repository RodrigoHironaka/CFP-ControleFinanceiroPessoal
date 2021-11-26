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
    public class Usuario : Base
    {
        public virtual String NomeAcesso { get; set; }
        public virtual String Senha { get; set; }
        public virtual TipoUsuario TipoUsuario { get; set; }
        public virtual Situacao Situacao { get; set; }
    }
}
