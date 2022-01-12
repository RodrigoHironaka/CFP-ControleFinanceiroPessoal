using CFP.Dominio.ObjetoValor;
using Dominio.Dominio;
using Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.DTO.Contas
{
    public class ConsultaContasDTO
    {
        public Int32 NumeroParcela { get; set; }
        public Decimal ValorParcela { get; set; }
        public virtual Decimal ValorReajustado { get; set; }
        public DateTime? DataVencimento { get; set; }
        public Conta Conta { get; set; }
        public SituacaoParcela SituacaoParcelas { get; set; }

        public IList<ConsultaContasDTO> ToList(ISession session)
        {
            var v = Query(null, session);
            return v.ToList();
        }

        public IList<ConsultaContasDTO> ToList(System.Linq.Expressions.Expression<Func<ContaPagamento, bool>> predicado, ISession session)
        {
            var v = Query(predicado, session);
            return v.ToList();
        }

        public ConsultaContasDTO Load(System.Linq.Expressions.Expression<Func<ContaPagamento, bool>> predicado, ISession session)
        {
            var v = Query(predicado, session);
            return v.SingleOrDefault();
        }

        private static IQueryable<ConsultaContasDTO> Query(System.Linq.Expressions.Expression<Func<ContaPagamento, bool>> predicado, ISession session)
        {
            var v = session.Query<ContaPagamento>();
            if (predicado != null)
                v = v.Where(predicado);

            return v.Select(x => new ConsultaContasDTO
            {
                NumeroParcela = x.Numero,
                ValorParcela = x.ValorParcela,
                ValorReajustado = x.ValorReajustado,
                DataVencimento = x.DataVencimento,
                SituacaoParcelas = x.SituacaoParcelas,

                Conta = new Conta
                {
                    Codigo = x.Conta.Codigo,
                    Nome = x.Conta.Nome,
                    TipoConta = x.Conta.TipoConta,
                    NumeroDocumento = x.Conta.NumeroDocumento,
                    Situacao = x.Conta.Situacao,
                    TipoPeriodo = x.Conta.TipoPeriodo,
                    FormaCompra = new FormaPagamento
                    {
                        Id = x.Conta.FormaCompra.Id,
                        Nome = x.Conta.FormaCompra.Nome
                    },
                    Pessoa = new Pessoa
                    {
                        Id = x.Conta.Pessoa.Id,
                        Nome = x.Conta.Pessoa.Nome
                    },
                    UsuarioCriacao = new Usuario
                    {
                        Id = x.Conta.UsuarioCriacao.Id,
                        Nome = x.Conta.UsuarioCriacao.Nome
                    }
                }
            });
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Conta.Codigo, Conta.Nome);
        }



    }
}
