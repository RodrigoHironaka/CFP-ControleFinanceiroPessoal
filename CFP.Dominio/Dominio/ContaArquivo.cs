using Dominio.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class ContaArquivo
    {
        public virtual Int64 Id { get; set; }
        public virtual Conta Conta { get; set; }
        public virtual String Caminho { get; set; }
    }
}
