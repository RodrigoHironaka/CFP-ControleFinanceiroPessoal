using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
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
    public class PessoaMAP : ClassMapping<Pessoa>
    {
        public PessoaMAP()
        {
            Table("Pessoas");

            Id(x => x.Id, m => {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0}));
            });

            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.ValorTotalBruto, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.ValorTotalLiquido, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.Situacao, m => m.Type<EnumType<Situacao>>());

            Bag(x => x.PessoaTipoRendas, m =>
            {
                m.Cascade(Cascade.All);
                m.Key(k => k.Column("Pessoa"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(PessoaTipoRendas))));
        }
    }
}
