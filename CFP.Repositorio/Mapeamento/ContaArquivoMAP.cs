using CFP.Dominio.Dominio;
using Dominio.Dominio;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Repositorio.Mapeamento
{
    public class ContaArquivoMAP : ClassMapping<ContaArquivo>
    {
        public ContaArquivoMAP()
        {
            Table("ContaArquivos");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));

            });
            ManyToOne(x => x.Conta, m => m.Column("Conta"));
            Property(x => x.Nome, m => m.Length(250));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Caminho, m => m.Length(250));
            


        }
    }
}
