using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class ContaArquivo : Base
    {
        public override string ToString()
        {
            return Nome;
        }

        public virtual Conta Conta { get; set; }
        public virtual String Caminho { get; set; }
    }
}
