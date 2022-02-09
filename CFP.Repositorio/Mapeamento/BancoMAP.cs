﻿using CFP.Dominio.ObjetoValor;
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
    public class BancoMAP : ClassMapping<Banco>
    {
        public BancoMAP()
        {
            Table("Bancos");

            Id(x => x.Id, m =>
            {
                m.Generator(Generators.HighLow, g => g.Params(new { max_lo = 0}));
            });

            Property(x => x.Nome, m => m.Length(70));
            Property(x => x.DataGeracao);
            Property(x => x.DataAlteracao);
            Property(x => x.TipoContaBanco, m => m.Type<EnumType<TipoContaBanco>>());
            Property(x => x.Situacao, m => m.Type<EnumType<Situacao>>());
            Property(x => x.UsarValorParaCalculos, m => m.Type<EnumType<SimNao>>());

            ManyToOne(x => x.PessoaBanco, m => m.Column("Pessoa"));
            ManyToOne(x => x.UsuarioCriacao, m => m.Column("UsuarioCriacao"));
            ManyToOne(x => x.UsuarioAlteracao, m => m.Column("UsuarioAlteracao"));
        }
    }
}
