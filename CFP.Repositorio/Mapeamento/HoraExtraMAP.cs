using Dominio.Dominio;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Mapeamentos
{
    public class HoraExtraMAP : ClassMapping<HoraExtra>
    {
        public HoraExtraMAP()
        {
            Table("HorasExtra");

            Id(x => x.ID, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            Property(x => x.Data);
            Property(x => x.HoraInicioManha);
            Property(x => x.HoraFinalManha);
            Property(x => x.TotalManha);
            Property(x => x.HoraInicioTarde);
            Property(x => x.HoraFinalTarde);
            Property(x => x.TotalTarde);
            Property(x => x.HoraFinalDia);
            

        }
    }
}
