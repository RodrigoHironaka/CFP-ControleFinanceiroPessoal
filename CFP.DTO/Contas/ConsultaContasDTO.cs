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
        public String TipoConta { get; set; }
        public Int64 CodigoConta { get; set; }
        public String NomeConta { get; set; }
        public Int32 NumeroParcela { get; set; }
        public Decimal ValorParcela { get; set; }
        public Decimal ValorReajustado { get; set; }
        public Decimal ValorPago { get; set; }
        public DateTime? DataVencimento { get; set; }
        public String FormaPagamento { get; set; }
        public Int64 NumeroDocumento { get; set; }
        public String PessoaNome { get; set; }
        public String SituacaoParcelas { get; set; }

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
            var query = session.Query<ContaPagamento>();
            if (predicado != null)
                query = query.Where(predicado);

            var consulta = from x in query select new ConsultaContasDTO
            {
                NumeroParcela = x.Numero,
                ValorParcela = x.ValorParcela,
                ValorReajustado = x.ValorReajustado,
                ValorPago = x.ValorPago,
                DataVencimento = x.DataVencimento,
                SituacaoParcelas = Enum.GetName(typeof(SituacaoParcela), x.SituacaoParcelas),
                TipoConta = Enum.GetName(typeof(TipoConta), x.Conta.TipoConta), 
                CodigoConta = x.Conta.Codigo,
                NomeConta = x.Conta.Nome,
                FormaPagamento = x.FormaPagamento.Nome,
                NumeroDocumento = x.Conta.NumeroDocumento != null ? (long)x.Conta.NumeroDocumento : 0,
                PessoaNome = x.Conta.Pessoa != null ? x.Conta.Pessoa.Nome : null
            };
            return consulta;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", CodigoConta, NomeConta);
        }



    }
}
