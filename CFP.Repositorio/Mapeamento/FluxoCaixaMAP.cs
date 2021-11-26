﻿using CFP.Dominio.Dominio;
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
    public class FluxoCaixaMAP : ClassMapping<FluxoCaixa>
    {
        public FluxoCaixaMAP()
        {
            Table("FluxoCaixas");

            Id(x => x.Id, m => {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0 }));
            });
            Property(x => x.Nome, m => m.Length(150));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.Valor, m =>
            {
                m.Precision(10);
                m.Scale(2);
            });
            Property(x => x.TipoFluxo, m => m.Type<EnumType<EntradaSaida>>());
            ManyToOne(x => x.Caixa, m => m.Column("Caixa"));
            ManyToOne(x => x.Conta, m => m.Column("Conta"));
            ManyToOne(x => x.FormaPagamento, m => m.Column("FormaPagamento"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}
