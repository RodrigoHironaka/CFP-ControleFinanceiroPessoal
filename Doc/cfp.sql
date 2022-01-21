
alter table CartaoCreditoItens  drop foreign key FK_888A592D
;

alter table CartaoCreditoItens  drop foreign key FK_29C723AB
;

alter table CartaoCreditoItens  drop foreign key FK_7EB380E7
;

alter table CartaoCreditoItens  drop foreign key FK_E4540CEC
;

alter table CartaoCreditoItens  drop foreign key FK_94424A0F
;

alter table CartoesCredito  drop foreign key FK_85E690E0
;

alter table CartoesCredito  drop foreign key FK_FB3847D0
;

alter table CartoesCredito  drop foreign key FK_7B72D78E
;

alter table Configuracoes  drop foreign key FK_39C0199
;

alter table Configuracoes  drop foreign key FK_CE2F688A
;

alter table Configuracoes  drop foreign key FK_679D977D
;

alter table ContaArquivos  drop foreign key FK_72DE5358
;

alter table ContaArquivos  drop foreign key FK_1A33A722
;

alter table ContaArquivos  drop foreign key FK_7CF12460
;

alter table FluxoCaixas  drop foreign key FK_D59D810C
;

alter table FluxoCaixas  drop foreign key FK_2056DEA5
;

alter table FluxoCaixas  drop foreign key FK_95CF74AB
;

alter table FluxoCaixas  drop foreign key FK_8F495043
;

alter table FluxoCaixas  drop foreign key FK_D3D389E7
;

alter table SubGrupoGastos  drop foreign key FK_ED2BD212
;

alter table SubGrupoGastos  drop foreign key FK_46645E96
;

alter table SubGrupoGastos  drop foreign key FK_94FE4FD2
;

alter table Bancos  drop foreign key FK_1E48502F
;

alter table Bancos  drop foreign key FK_8C4ECE36
;

alter table Bancos  drop foreign key FK_C1767FF5
;

alter table Caixas  drop foreign key FK_1DD50A06
;

alter table Caixas  drop foreign key FK_6402C960
;

alter table Caixas  drop foreign key FK_D1D5DDED
;

alter table Cofres  drop foreign key FK_FE16542B
;

alter table Cofres  drop foreign key FK_1B7FF394
;

alter table Cofres  drop foreign key FK_C9974506
;

alter table Cofres  drop foreign key FK_34B96F4A
;

alter table Cofres  drop foreign key FK_A1EFEF63
;

alter table Contas  drop foreign key FK_27A3E395
;

alter table Contas  drop foreign key FK_108F2CD5
;

alter table Contas  drop foreign key FK_5DDB1A8A
;

alter table Contas  drop foreign key FK_747B3355
;

alter table Contas  drop foreign key FK_B2083560
;

alter table ContasPagamento  drop foreign key FK_1109C842
;

alter table ContasPagamento  drop foreign key FK_77677524
;

alter table FormasPagamento  drop foreign key FK_72A8B73B
;

alter table FormasPagamento  drop foreign key FK_C3A6039E
;

alter table GrupoGastos  drop foreign key FK_B9398F83
;

alter table GrupoGastos  drop foreign key FK_F498C017
;

alter table HorasExtra  drop foreign key FK_9FA1A2B6
;

alter table HorasExtra  drop foreign key FK_72687FDB
;

alter table HorasExtra  drop foreign key FK_A6951426
;

alter table Pessoas  drop foreign key FK_42D8A7E1
;

alter table Pessoas  drop foreign key FK_70F48CA1
;

alter table PessoaTipoRendas  drop foreign key FK_2F54ECD9
;

alter table PessoaTipoRendas  drop foreign key FK_DC98FCA8
;

alter table TiposRenda  drop foreign key FK_DEE36B93
;

alter table TiposRenda  drop foreign key FK_858D8654
;
drop table if exists CartaoCreditoItens;
drop table if exists CartoesCredito;
drop table if exists Configuracoes;
drop table if exists ContaArquivos;
drop table if exists FluxoCaixas;
drop table if exists SubGrupoGastos;
drop table if exists Bancos;
drop table if exists Caixas;
drop table if exists Cofres;
drop table if exists Contas;
drop table if exists ContasPagamento;
drop table if exists FormasPagamento;
drop table if exists GrupoGastos;
drop table if exists HorasExtra;
drop table if exists Pessoas;
drop table if exists PessoaTipoRendas;
drop table if exists TiposRenda;
drop table if exists Usuarios;
drop table if exists hibernate_unique_key;
create table CartaoCreditoItens (Id BIGINT not null, Nome VARCHAR(100), Valor DECIMAL(10, 2), NumeroParcelas VARCHAR(30), DataCompra DATETIME, SubGrupoGasto BIGINT, Pessoa BIGINT, CartaoCredito BIGINT, DataGeracao DATETIME, DataAlteracao DATETIME, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table CartoesCredito (Id BIGINT not null, Nome TEXT, DataGeracao DATETIME, DataAlteracao DATETIME, MesReferencia INTEGER, AnoReferencia INTEGER, ValorFatura DECIMAL(10, 2), SituacaoFatura INTEGER, Cartao BIGINT, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table Configuracoes (Id BIGINT not null, DataGeracao DATETIME, DataAlteracao DATETIME, UsuarioCriacao BIGINT, CaminhoArquivos VARCHAR(250), CaminhoBackup VARCHAR(250), FormaPagamentoPadraoConta BIGINT, TransacaoBancariaPadrao BIGINT, DiasAlertaVencimento INTEGER, primary key (Id)) ENGINE=InnoDB;
create table ContaArquivos (Id BIGINT not null, Conta BIGINT, Caminho VARCHAR(250), Nome VARCHAR(250), DataGeracao DATETIME, DataAlteracao DATETIME, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table FluxoCaixas (Id BIGINT not null, Nome VARCHAR(150), DataGeracao DATETIME, DataAlteracao DATETIME, Valor DECIMAL(10, 2), TipoFluxo INTEGER, Conta BIGINT, Caixa BIGINT, FormaPagamento BIGINT, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table SubGrupoGastos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, GrupoGasto BIGINT, Situacao INTEGER, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table Bancos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, TipoContaBanco INTEGER, Situacao INTEGER, Pessoa BIGINT, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table Caixas (Id BIGINT not null, Codigo BIGINT, DataAbertura DATETIME, DataFechamento DATETIME, ValorInicial DECIMAL(10, 2), Situacao INTEGER, Pessoa BIGINT, UsuarioAbertura BIGINT, UsuarioFechamento BIGINT, TotalEntrada DECIMAL(10, 2), TotalSaida DECIMAL(10, 2), BalancoFinal DECIMAL(10, 2), primary key (Id)) ENGINE=InnoDB;
create table Cofres (Id BIGINT not null, Codigo BIGINT, Caixa BIGINT, Banco BIGINT, Valor DECIMAL(10, 2), TransacoesBancarias BIGINT, Situacao INTEGER, Nome VARCHAR(200), DataGeracao DATETIME, DataAlteracao DATETIME, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table Contas (Id BIGINT not null, Codigo BIGINT, TipoConta INTEGER, TipoPeriodo INTEGER, Situacao INTEGER, DataEmissao DATETIME, ValorTotal DECIMAL(10, 2), QtdParcelas BIGINT, NumeroDocumento BIGINT, SubGrupoGasto BIGINT, FormaCompra BIGINT, FaturaCartaoCredito BIGINT, Observacao TEXT, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table ContasPagamento (ID BIGINT not null, Numero INTEGER, ValorParcela DECIMAL(10, 2), DataVencimento DATETIME, DataPagamento DATETIME, JurosPorcentual DECIMAL(19,5), JurosValor DECIMAL(10, 2), DescontoPorcentual DECIMAL(19,5), DescontoValor DECIMAL(10, 2), ValorReajustado DECIMAL(10, 2), ValorPago DECIMAL(10, 2), ValorRestante DECIMAL(10, 2), SituacaoParcelas INTEGER, FormaPagamento BIGINT, Conta BIGINT, primary key (ID)) ENGINE=InnoDB;
create table FormasPagamento (Id BIGINT not null, Nome VARCHAR(70), QtdParcelas INTEGER, DiasParaVencimento INTEGER, Situacao INTEGER, TransacoesBancarias INTEGER, UsadoParaCompras INTEGER, RemoveCofre INTEGER, DiaVencimento INTEGER, DataGeracao DATETIME, DataAlteracao DATETIME, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table GrupoGastos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, Situacao INTEGER, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table HorasExtra (Id BIGINT not null, Nome VARCHAR(255), DataGeracao DATETIME, DataAlteracao DATETIME, Pessoa BIGINT, DataHoraExtra DATETIME, HoraInicioManha BIGINT, HoraFinalManha BIGINT, HoraInicioTarde BIGINT, HoraFinalTarde BIGINT, HoraInicioNoite BIGINT, HoraFinalNoite BIGINT, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table Pessoas (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, ValorTotalBruto DECIMAL(10, 2), ValorTotalLiquido DECIMAL(10, 2), UsarRendaParaCalculos INTEGER, Situacao INTEGER, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table PessoaTipoRendas (ID BIGINT not null, RendaBruta DECIMAL(10, 2), RendaLiquida DECIMAL(10, 2), TipoRenda BIGINT, Pessoa BIGINT, primary key (ID)) ENGINE=InnoDB;
create table TiposRenda (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, Situacao INTEGER, UsuarioCriacao BIGINT, UsuarioAlteracao BIGINT, primary key (Id)) ENGINE=InnoDB;
create table Usuarios (Id BIGINT not null, Nome VARCHAR(70), NomeAcesso VARCHAR(70), Senha VARCHAR(255), TipoUsuario INTEGER, Situacao INTEGER, DataGeracao DATETIME, DataAlteracao DATETIME, primary key (Id)) ENGINE=InnoDB;
alter table CartaoCreditoItens add index (SubGrupoGasto), add constraint FK_888A592D foreign key (SubGrupoGasto) references SubGrupoGastos (Id);
alter table CartaoCreditoItens add index (Pessoa), add constraint FK_29C723AB foreign key (Pessoa) references Pessoas (Id);
alter table CartaoCreditoItens add index (CartaoCredito), add constraint FK_7EB380E7 foreign key (CartaoCredito) references CartoesCredito (Id);
alter table CartaoCreditoItens add index (UsuarioCriacao), add constraint FK_E4540CEC foreign key (UsuarioCriacao) references Usuarios (Id);
alter table CartaoCreditoItens add index (UsuarioAlteracao), add constraint FK_94424A0F foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table CartoesCredito add index (Cartao), add constraint FK_85E690E0 foreign key (Cartao) references FormasPagamento (Id);
alter table CartoesCredito add index (UsuarioCriacao), add constraint FK_FB3847D0 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table CartoesCredito add index (UsuarioAlteracao), add constraint FK_7B72D78E foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table Configuracoes add index (UsuarioCriacao), add constraint FK_39C0199 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table Configuracoes add index (FormaPagamentoPadraoConta), add constraint FK_CE2F688A foreign key (FormaPagamentoPadraoConta) references FormasPagamento (Id);
alter table Configuracoes add index (TransacaoBancariaPadrao), add constraint FK_679D977D foreign key (TransacaoBancariaPadrao) references FormasPagamento (Id);
alter table ContaArquivos add index (Conta), add constraint FK_72DE5358 foreign key (Conta) references Contas (Id);
alter table ContaArquivos add index (UsuarioCriacao), add constraint FK_1A33A722 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table ContaArquivos add index (UsuarioAlteracao), add constraint FK_7CF12460 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table FluxoCaixas add index (Conta), add constraint FK_D59D810C foreign key (Conta) references Contas (Id);
alter table FluxoCaixas add index (Caixa), add constraint FK_2056DEA5 foreign key (Caixa) references Caixas (Id);
alter table FluxoCaixas add index (FormaPagamento), add constraint FK_95CF74AB foreign key (FormaPagamento) references FormasPagamento (Id);
alter table FluxoCaixas add index (UsuarioCriacao), add constraint FK_8F495043 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table FluxoCaixas add index (UsuarioAlteracao), add constraint FK_D3D389E7 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table SubGrupoGastos add index (GrupoGasto), add constraint FK_ED2BD212 foreign key (GrupoGasto) references GrupoGastos (Id);
alter table SubGrupoGastos add index (UsuarioCriacao), add constraint FK_46645E96 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table SubGrupoGastos add index (UsuarioAlteracao), add constraint FK_94FE4FD2 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table Bancos add index (Pessoa), add constraint FK_1E48502F foreign key (Pessoa) references Pessoas (Id);
alter table Bancos add index (UsuarioCriacao), add constraint FK_8C4ECE36 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table Bancos add index (UsuarioAlteracao), add constraint FK_C1767FF5 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table Caixas add index (Pessoa), add constraint FK_1DD50A06 foreign key (Pessoa) references Pessoas (Id);
alter table Caixas add index (UsuarioAbertura), add constraint FK_6402C960 foreign key (UsuarioAbertura) references Usuarios (Id);
alter table Caixas add index (UsuarioFechamento), add constraint FK_D1D5DDED foreign key (UsuarioFechamento) references Usuarios (Id);
alter table Cofres add index (Caixa), add constraint FK_FE16542B foreign key (Caixa) references Caixas (Id);
alter table Cofres add index (Banco), add constraint FK_1B7FF394 foreign key (Banco) references Bancos (Id);
alter table Cofres add index (TransacoesBancarias), add constraint FK_C9974506 foreign key (TransacoesBancarias) references FormasPagamento (Id);
alter table Cofres add index (UsuarioCriacao), add constraint FK_34B96F4A foreign key (UsuarioCriacao) references Usuarios (Id);
alter table Cofres add index (UsuarioAlteracao), add constraint FK_A1EFEF63 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table Contas add index (SubGrupoGasto), add constraint FK_27A3E395 foreign key (SubGrupoGasto) references SubGrupoGastos (Id);
alter table Contas add index (FormaCompra), add constraint FK_108F2CD5 foreign key (FormaCompra) references FormasPagamento (Id);
alter table Contas add index (FaturaCartaoCredito), add constraint FK_5DDB1A8A foreign key (FaturaCartaoCredito) references Pessoas (Id);
alter table Contas add index (UsuarioCriacao), add constraint FK_747B3355 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table Contas add index (UsuarioAlteracao), add constraint FK_B2083560 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table ContasPagamento add index (FormaPagamento), add constraint FK_1109C842 foreign key (FormaPagamento) references FormasPagamento (Id);
alter table ContasPagamento add index (Conta), add constraint FK_77677524 foreign key (Conta) references Contas (Id);
alter table FormasPagamento add index (UsuarioCriacao), add constraint FK_72A8B73B foreign key (UsuarioCriacao) references Usuarios (Id);
alter table FormasPagamento add index (UsuarioAlteracao), add constraint FK_C3A6039E foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table GrupoGastos add index (UsuarioCriacao), add constraint FK_B9398F83 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table GrupoGastos add index (UsuarioAlteracao), add constraint FK_F498C017 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table HorasExtra add index (Pessoa), add constraint FK_9FA1A2B6 foreign key (Pessoa) references Pessoas (Id);
alter table HorasExtra add index (UsuarioCriacao), add constraint FK_72687FDB foreign key (UsuarioCriacao) references Usuarios (Id);
alter table HorasExtra add index (UsuarioAlteracao), add constraint FK_A6951426 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table Pessoas add index (UsuarioCriacao), add constraint FK_42D8A7E1 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table Pessoas add index (UsuarioAlteracao), add constraint FK_70F48CA1 foreign key (UsuarioAlteracao) references Usuarios (Id);
alter table PessoaTipoRendas add index (TipoRenda), add constraint FK_2F54ECD9 foreign key (TipoRenda) references TiposRenda (Id);
alter table PessoaTipoRendas add index (Pessoa), add constraint FK_DC98FCA8 foreign key (Pessoa) references Pessoas (Id);
alter table TiposRenda add index (UsuarioCriacao), add constraint FK_DEE36B93 foreign key (UsuarioCriacao) references Usuarios (Id);
alter table TiposRenda add index (UsuarioAlteracao), add constraint FK_858D8654 foreign key (UsuarioAlteracao) references Usuarios (Id);
create table hibernate_unique_key ( next_hi BIGINT );
insert into hibernate_unique_key values ( 1 );
