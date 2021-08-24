using Dominio.Dominio;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Repositorios
{
    public class RepositorioTipoRenda : RepositorioBase<TipoRenda>
    {
        public RepositorioTipoRenda(ISession session) : base(session) { }
    }
}
