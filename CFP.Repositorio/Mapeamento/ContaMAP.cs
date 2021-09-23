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
    public class ContaMAP : ClassMapping<Conta>
    {
        public ContaMAP()
        {
            Table("Contas");
            Id(x => x.ID, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.TipoConta, m => m.Type<EnumType<TipoConta>>());
            Property(x => x.TipoPeriodo, m => m.Type<EnumType<TipoPeriodo>>());
            Property(x => x.QtdParcelas);
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
          
            Property(x => x.Nome, m => m.Length(70));
            ManyToOne(x => x.GrupoGasto, m => m.Column("GrupoGasto"));
            Property(x => x.ValorTotal, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.ValorParcela, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.DataEmissao);
            Property(x => x.DataVencimento);
            Property(x => x.NumeroDocumento);
            Property(x => x.Observacao, m => m.Length(400));
            Property(x => x.Situacao, m => m.Type<EnumType<SituacaoConta>>());

        }
    }
}
