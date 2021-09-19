using CFP.Dominio.Dominio;
using Dominio.Interfaces;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Dominio
{
    public class Pessoa : Base
    {
        public override string ToString()
        {
            return Nome;
        }

        public Pessoa()
        {
            PessoaTipoRendas = new List<PessoaTipoRendas>();
        }

        public virtual Decimal ValorTotalBruto { get; set; }
        public virtual Decimal ValorTotalLiquido { get; set; }
        public virtual Situacao Situacao { get; set; }
        public virtual IList<PessoaTipoRendas> PessoaTipoRendas { get; set; }

    }
}
