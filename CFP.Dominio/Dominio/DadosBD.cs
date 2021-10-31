using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class DadosBD
    {
        public virtual String ServidorBD { get; set; }
        public virtual String BaseBD { get; set; }
        public virtual String UsuarioBD { get; set; }
        public virtual String SenhaBD { get; set; }
        public virtual String PortaBD { get; set; }
        public virtual Boolean Servidor { get; set; }
    }
}
