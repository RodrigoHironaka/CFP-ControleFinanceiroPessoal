using Dominio.Dominio;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Dominio.Dominio
{
    public class SubGrupoGasto : Base
    {
        public override string ToString()
        {
            return String.Format("{0}/{1}", GrupoGasto.Nome, Nome);
        }
        public virtual GrupoGasto GrupoGasto { get; set; }
        public virtual Situacao Situacao { get; set; }

        public virtual String DescricaoCompleta
        {
            get
            {
               return String.Format("{0}/{1}", GrupoGasto.Nome, Nome);
            }
        }
    }
}
