using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class Base
    {
        public virtual Int64 Id { get; set; }
        public virtual String Nome { get; set; }
        public virtual DateTime DataGeracao { get; set; }
        public virtual DateTime DataAlteracao { get; set; }
    }
}
