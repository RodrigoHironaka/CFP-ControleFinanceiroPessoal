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
    public class RepositorioContaPagamento : RepositorioBase<ContaPagamento>
    {
        public RepositorioContaPagamento(ISession session) : base(session) 
        {
            
        }
    }
}
