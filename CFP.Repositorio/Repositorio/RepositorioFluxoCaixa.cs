using CFP.Dominio.Dominio;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Repositorio.Repositorio
{
     public class RepositorioFluxoCaixa : RepositorioBase<FluxoCaixa>
    {
        public RepositorioFluxoCaixa(ISession session) : base(session) { }
    
    }
}
