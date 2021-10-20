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
    public class ConfiguracaoMAP : ClassMapping<Configuracao>
    {
        public ConfiguracaoMAP()
        {
            Table("Configuracoes");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.ServidorBD, m => m.Length(10));
            Property(x => x.BaseBD, m => m.Length(50));
            Property(x => x.UsuarioBD, m => m.Length(50));
            Property(x => x.SenhaBD, m => m.Length(50));
            Property(x => x.PortaBD, m => m.Length(10));
            Property(x => x.CaminhoArquivos, m => m.Length(250));
            Property(x => x.CaminhoBackup, m => m.Length(250));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
        }
    }
}
