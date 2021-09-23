using Dominio.Dominio;
using Dominio.ObejtoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Mapeamentos
{
    public class PessoaTipoRendasMAP : ClassMapping<PessoaTipoRendas>
    {
        public PessoaTipoRendasMAP()
        {
            Table("PessoaTipoRendas");

            Id(x => x.ID, m => {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0}));
            });
            
            Property(x => x.RendaBruta, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.RendaLiquida, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            ManyToOne(x => x.TipoRenda, m=>m.Column("TipoRenda"));
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
        }
    }
}
