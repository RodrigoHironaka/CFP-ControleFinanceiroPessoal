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
     public class RepositorioSubGrupoGasto : RepositorioBase<SubGrupoGasto>
    {
        public RepositorioSubGrupoGasto(ISession session) : base(session) { }
    }
}
