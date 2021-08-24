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
    public class CaixaMAP : ClassMapping<Caixa> 
    {
        public CaixaMAP()
        {
            Table("Caixas");
            Id(x => x.ID, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.DataAbertura);
            Property(x => x.DataFechamento);
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            Property(x => x.TotalEntrada);
            Property(x => x.TotalSaida);
            Property(x => x.BalançoFinal);
            Property(x => x.Situacao, m => m.Type<EnumType<SituacaoCaixa>>());

        }
    }
}
