using Dominio.Dominio;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
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
            ManyToOne(x => x.Conta, m => m.Column("Conta"));
            Property(x => x.JurosPorcentual);
            Property(x => x.JurosValor);
            Property(x => x.DescontoPorcentual); 
            Property(x => x.DescontoValor);
            Property(x => x.ValorReajustado);
            ManyToOne(x => x.FormaPagamento, m => m.Column("FormaPagamento"));
            

        }
    }
}
