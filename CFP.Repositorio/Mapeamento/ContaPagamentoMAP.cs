using Dominio.Dominio;
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
    public class ContaPagamentoMAP : ClassMapping<ContaPagamento>
    {
        public ContaPagamentoMAP()
        {
            Table("ContasPagamento");
            Id(x => x.ID, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
                
            });
            Property(x => x.Numero, m => m.Length(10));
            Property(x => x.ValorParcela, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.DataVencimento);
            Property(x => x.JurosPorcentual);
            Property(x => x.JurosValor, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.DescontoPorcentual); 
            Property(x => x.DescontoValor, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.ValorReajustado, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.SituacaoParcelas, m => m.Type<EnumType<SituacaoConta>>());
            ManyToOne(x => x.FormaPagamento, m => m.Column("FormaPagamento"));
            ManyToOne(x => x.Conta, m => m.Column("Conta"));


        }
    }
}
