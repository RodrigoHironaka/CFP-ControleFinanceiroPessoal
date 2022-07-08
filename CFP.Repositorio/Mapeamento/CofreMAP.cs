using CFP.Dominio.ObjetoValor;
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
    public class CofreMAP : ClassMapping<Cofre>
    {
        public CofreMAP()
        {
            Table("Cofres");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.Codigo);
            Property(x => x.Nome, m => m.Length(200));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Valor, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.DataRegistro);
            Property(x => x.Situacao, m => m.Type<EnumType<EntradaSaida>>());
            ManyToOne(x => x.Caixa, m => m.Column("Caixa"));
            ManyToOne(x => x.Banco, m => m.Column("Banco"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
            ManyToOne(x => x.TransacoesBancarias, m => m.Column("TransacoesBancarias"));

        }
    }
}
