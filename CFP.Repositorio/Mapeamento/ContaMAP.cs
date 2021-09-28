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
            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });

            Property(x => x.QtdParcelas, m => m.Length(3));
            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.Observacao, m => m.Length(400));
            Property(x => x.DataEmissao);
            Property(x => x.DataPrimeiroVencimento);
            Property(x => x.NumeroDocumento, m => m.Length(20));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Situacao, m => m.Type<EnumType<SituacaoConta>>());
            Property(x => x.TipoConta, m => m.Type<EnumType<TipoConta>>());
            Property(x => x.TipoPeriodo, m => m.Type<EnumType<TipoPeriodo>>());

            Property(x => x.ValorTotal, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
         
            ManyToOne(x => x.FormaCompra, m => m.Column("FormaCompra"));
            ManyToOne(x => x.Pessoa, m => m.Column("Pessoa"));
            ManyToOne(x => x.GrupoGasto, m => m.Column("GrupoGasto"));
        }
    }
}
