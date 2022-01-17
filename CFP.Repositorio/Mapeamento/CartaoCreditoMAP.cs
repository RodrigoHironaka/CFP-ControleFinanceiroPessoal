using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Repositorio.Mapeamento
{
    public class CartaoCreditoMAP : ClassMapping<CartaoCredito>
    {
        public CartaoCreditoMAP()
        {
            Table("CartaoCreditos");
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.MesReferencia);
            Property(x => x.AnoReferencia);
            Property(x => x.SituacaoFatura, m => m.Type<EnumType<SituacaoFatura>>());
            Property(x => x.ValorFatura, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });

            Bag(x => x.CartaoCreditos, m =>
            {
                m.Cascade(Cascade.All);
                m.Key(k => k.Column("CartaoCredito"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(CartaoCreditoItens))));
        }
    }
}
