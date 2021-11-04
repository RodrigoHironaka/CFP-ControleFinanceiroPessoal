using CFP.Dominio.Dominio;
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
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.Codigo);
            Property(x => x.DataAbertura);
            Property(x => x.DataFechamento);
            Property(x => x.ValorInicial, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            //Property(x => x.TotalEntrada, m =>
            //{
            //    m.Precision(10);
            //    m.Scale(2);
            //});
            //Property(x => x.TotalSaida, m =>
            //{
            //    m.Precision(10);
            //    m.Scale(2);
            //});
            //Property(x => x.BalancoFinal, m =>
            //{
            //    m.Precision(10);
            //    m.Scale(2);
            //});
            Property(x => x.Situacao, m => m.Type<EnumType<SituacaoCaixa>>());
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.UsuarioAbertura, m => m.Column("UsuarioAbertura"));
            ManyToOne(x => x.UsuarioFechamento, m => m.Column("UsuarioFechamento"));

            Bag(x => x.FluxoCaixas, m =>
            {
                m.Cascade(Cascade.All);
                m.Key(k => k.Column("Caixa"));
                m.Inverse(true);
            }, map => map.OneToMany(a => a.Class(typeof(FluxoCaixa))));
        }
    }
}
