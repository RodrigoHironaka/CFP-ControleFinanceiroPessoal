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
    public class CartaoCreditoItensMAP : ClassMapping<CartaoCreditoItens>
    {
        public CartaoCreditoItensMAP()
        {
            Table("CartaoCreditoItens");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.Nome, m => m.Length(100));
            Property(x => x.Qtd);
            Property(x => x.DataCompra);
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);

            Property(x => x.Valor, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });

            ManyToOne(x => x.Cartao, m => m.Column("Cartao"));
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.SubGrupoGasto, m => m.Column("SubGrupoGasto"));
            ManyToOne(x => x.CartaoCredito, m => m.Column("CartaoCredito"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}
