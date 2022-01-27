using CFP.Dominio.Dominio;
using Dominio.Dominio;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Repositorios
{
    public class RepositorioConta : RepositorioBase<Conta>
    {
        public RepositorioConta(ISession session) : base(session) { }
    }
}
