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
    public class UsuarioMAP : ClassMapping<Usuario>
    {
        public UsuarioMAP()
        {
            Table("Usuarios");

            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo=0}));
            });

            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Senha, m => m.Length(100));
            Property(x => x.ConfirmaSenha, m => m.Length(100));
            Property(x => x.TipoUsuario, m => m.Type<EnumType<TipoUsuario>>());
            Property(x => x.Situacao, m => m.Type<EnumType<Situacao>>());
        }
    }
}
