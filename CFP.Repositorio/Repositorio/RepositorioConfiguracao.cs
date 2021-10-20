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
    public class RepositorioConfiguracao : RepositorioBase<Configuracao>
    {
        public RepositorioConfiguracao(ISession session) : base(session) { }
    }
}
