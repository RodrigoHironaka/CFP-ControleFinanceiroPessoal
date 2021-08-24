using Dominio.Dominio;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Repositorios
{
    public class RepositorioCofre : RepositorioBase<Cofre>
    {
        public RepositorioCofre(ISession session) : base(session) { }
    }
}
