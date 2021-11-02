using CFP.Dominio.Dominio;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Repositorio.Mapeamento
{
    public class FluxoCaixaMAP : ClassMapping<FluxoCaixa>
    {
        public FluxoCaixaMAP()
        {
            Table("FluxoCaixas");

            Id(x => x.Id, m => {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.Nome, m => m.Length(150));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            ManyToOne(x => x.Caixa, m => m.Column("Caixa"));
            ManyToOne(x => x.UsuarioLogado, m => m.Column("UsuarioLogado"));
        }
    }
}
