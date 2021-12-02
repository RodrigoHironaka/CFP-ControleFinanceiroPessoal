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

            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            
            Property(x => x.Nome);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.DataHoraExtra);
            Property(x => x.HoraInicioManha);
            Property(x => x.HoraFinalManha);
            Property(x => x.HoraInicioTarde);
            Property(x => x.HoraFinalTarde);
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));


        }
    }
}
