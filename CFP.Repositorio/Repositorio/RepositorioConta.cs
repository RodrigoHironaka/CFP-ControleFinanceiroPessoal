using CFP.Dominio.Dominio;
using Dominio.Dominio;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Repositorios
{
    public class RepositorioConta : RepositorioBase<Conta>
    {
        public RepositorioConta(ISession session) : base(session) { }

        public void NovaContaRefCartaoCredito(Usuario usuario, string nome, SubGrupoGasto subGrupoGasto, FormaPagamento formaPagamento, CartaoCredito faturaCartao, ISession session)
        {
            Conta conta = new Conta();
            conta.Codigo = RetornaUltimoCodigo() + 1;
            conta.UsuarioCriacao = usuario;
            conta.Nome = nome;
            conta.TipoConta = Dominio.ObjetoValor.TipoConta.Pagar;
            conta.SubGrupoGasto = subGrupoGasto;
            conta.TipoPeriodo = Dominio.ObejtoValor.TipoPeriodo.Unica;
            conta.Situacao = Dominio.ObjetoValor.SituacaoConta.Aberto;
            conta.DataEmissao = DateTime.Now;
            conta.Pessoa = null;
            conta.NumeroDocumento = null;
            conta.FormaCompra = formaPagamento;
            conta.Observacao = string.Empty;
            conta.QtdParcelas = 1;
            conta.ValorTotal = 0;
            conta.FaturaCartaoCredito = faturaCartao;

            int dia = new RepositorioFormaPagamento(session).ObterPorParametros(x => x.Id == faturaCartao.Cartao.Id).Select(x => x.DiaVencimento).First();
            int mes = faturaCartao.MesReferencia == 12 ? 1 : faturaCartao.MesReferencia + 1;
            int ano = faturaCartao.MesReferencia == 12 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            string DataVencimento = String.Format("{0:00}/{1:00}/{2:0000}", dia, mes, ano); 
            List<ContaPagamento> contaPagamento = new List<ContaPagamento>();
            contaPagamento.Add(new ContaPagamento()
            {
                Numero = 1,
                ValorParcela = 0,
                ValorReajustado = 0,
                DataVencimento = DateTime.ParseExact(DataVencimento, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                SituacaoParcelas = CFP.Dominio.ObjetoValor.SituacaoParcela.Pendente,
                Conta = conta
            });
            conta.ContaPagamentos = contaPagamento;

            SalvarLote(conta);
        }
    }
}
