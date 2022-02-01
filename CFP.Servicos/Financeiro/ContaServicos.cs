using CFP.Dominio.Dominio;
using CFP.Dominio.ObjetoValor;
using CFP.Repositorio.Repositorio;
using Dominio.Dominio;
using Dominio.ObejtoValor;
using Dominio.ObjetoValor;
using NHibernate;
using Repositorio.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFP.Servicos.Financeiro
{
    public class ContaServicos
    {
        public static void NovaContaRefCartaoCredito(Usuario usuario, CartaoCredito faturaCartao, ISession session)
        {
            Conta conta = new Conta();
            conta.Codigo = new RepositorioConta(session).RetornaUltimoCodigo() + 1;
            conta.UsuarioCriacao = usuario;
            conta.Nome = String.Format("Fatura {0}", faturaCartao.DescricaoCompleta);
            conta.TipoConta = TipoConta.Pagar;
            conta.SubGrupoGasto = new RepositorioConfiguracao(session).ObterTodos().Select(x => x.GrupoGastoFaturaPadrao).FirstOrDefault();
            conta.TipoPeriodo = TipoPeriodo.Unica;
            conta.Situacao = SituacaoConta.Aberto;
            conta.DataEmissao = DateTime.Now;
            conta.Pessoa = null;
            conta.NumeroDocumento = 0;
            conta.FormaCompra = faturaCartao.Cartao;
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
                SituacaoParcelas = SituacaoParcela.Pendente,
                Conta = conta
            });
            conta.ContaPagamentos = contaPagamento;

            new RepositorioConta(session).SalvarLote(conta);
        }
    }
}
