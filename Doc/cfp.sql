
alter table SubGrupoGastos  drop foreign key FK_ED2BD212
;

alter table Caixas  drop foreign key FK_1DD50A06
;

alter table Cofres  drop foreign key FK_FE16542B
;

alter table Cofres  drop foreign key FK_1B7FF394
;

alter table Contas  drop foreign key FK_27A3E395
;

alter table Contas  drop foreign key FK_63270F71
;

alter table Contas  drop foreign key FK_108F2CD5
;

alter table Contas  drop foreign key FK_4BC4A722
;

alter table ContasPagamento  drop foreign key FK_1109C842
;

alter table ContasPagamento  drop foreign key FK_77677524
;

alter table HorasExtra  drop foreign key FK_9FA1A2B6
;

alter table PessoaTipoRendas  drop foreign key FK_2F54ECD9
;

alter table PessoaTipoRendas  drop foreign key FK_DC98FCA8
;
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
create table SubGrupoGastos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, GrupoGasto BIGINT, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table Bancos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, TipoContaBanco INTEGER, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table Caixas (ID BIGINT not null, DataAbertura DATETIME, DataFechamento DATETIME, Pessoa BIGINT, TotalEntrada DOUBLE, TotalSaida DOUBLE, BalançoFinal DOUBLE, Situacao INTEGER, primary key (ID)) ENGINE=InnoDB;
create table Cofres (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, Caixa BIGINT, Banco BIGINT, Valor DOUBLE, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table Contas (Id BIGINT not null, TipoConta INTEGER, TipoPeriodo INTEGER, Situacao INTEGER, DataEmissao DATETIME, DataPrimeiroVencimento DATETIME, ValorTotal DECIMAL(10, 2), QtdParcelas BIGINT, NumeroDocumento BIGINT, SubGrupoGasto BIGINT, GrupoGasto BIGINT, FormaCompra BIGINT, Pessoa BIGINT, Observacao TEXT, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, primary key (Id)) ENGINE=InnoDB;
create table ContasPagamento (ID BIGINT not null, Numero INTEGER, ValorParcela DECIMAL(10, 2), DataVencimento DATETIME, DataPagamento DATETIME, JurosPorcentual DECIMAL(19,5), JurosValor DECIMAL(10, 2), DescontoPorcentual DECIMAL(19,5), DescontoValor DECIMAL(10, 2), ValorReajustado DECIMAL(10, 2), ValorPago DECIMAL(10, 2), ValorRestante DECIMAL(10, 2), SituacaoParcelas INTEGER, FormaPagamento BIGINT, Conta BIGINT, primary key (ID)) ENGINE=InnoDB;
create table FormasPagamento (Id BIGINT not null, Nome VARCHAR(70), QtdParcelas INTEGER, DiasParaVencimento INTEGER, Situacao INTEGER, DataGeracao DATETIME, DataAlteracao DATETIME, primary key (Id)) ENGINE=InnoDB;
create table GrupoGastos (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table HorasExtra (ID BIGINT not null, Pessoa BIGINT, Data DATETIME, HoraInicioManha BIGINT, HoraFinalManha BIGINT, TotalManha BIGINT, HoraInicioTarde BIGINT, HoraFinalTarde BIGINT, TotalTarde BIGINT, HoraFinalDia BIGINT, primary key (ID)) ENGINE=InnoDB;
create table Pessoas (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, ValorTotalBruto DECIMAL(10, 2), ValorTotalLiquido DECIMAL(10, 2), Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table PessoaTipoRendas (ID BIGINT not null, RendaBruta DECIMAL(10, 2), RendaLiquida DECIMAL(10, 2), TipoRenda BIGINT, Pessoa BIGINT, primary key (ID)) ENGINE=InnoDB;
create table TiposRenda (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
create table Usuarios (Id BIGINT not null, Nome VARCHAR(70), DataGeracao DATETIME, DataAlteracao DATETIME, Senha VARCHAR(100), ConfirmaSenha VARCHAR(100), TipoUsuario INTEGER, Situacao INTEGER, primary key (Id)) ENGINE=InnoDB;
alter table SubGrupoGastos add index (GrupoGasto), add constraint FK_ED2BD212 foreign key (GrupoGasto) references GrupoGastos (Id);
alter table Caixas add index (Pessoa), add constraint FK_1DD50A06 foreign key (Pessoa) references Pessoas (Id);
alter table Cofres add index (Caixa), add constraint FK_FE16542B foreign key (Caixa) references Caixas (ID);
alter table Cofres add index (Banco), add constraint FK_1B7FF394 foreign key (Banco) references Bancos (Id);
alter table Contas add index (SubGrupoGasto), add constraint FK_27A3E395 foreign key (SubGrupoGasto) references SubGrupoGastos (Id);
alter table Contas add index (GrupoGasto), add constraint FK_63270F71 foreign key (GrupoGasto) references GrupoGastos (Id);
alter table Contas add index (FormaCompra), add constraint FK_108F2CD5 foreign key (FormaCompra) references FormasPagamento (Id);
alter table Contas add index (Pessoa), add constraint FK_4BC4A722 foreign key (Pessoa) references Pessoas (Id);
alter table ContasPagamento add index (FormaPagamento), add constraint FK_1109C842 foreign key (FormaPagamento) references FormasPagamento (Id);
alter table ContasPagamento add index (Conta), add constraint FK_77677524 foreign key (Conta) references Contas (Id);
alter table HorasExtra add index (Pessoa), add constraint FK_9FA1A2B6 foreign key (Pessoa) references Pessoas (Id);
alter table PessoaTipoRendas add index (TipoRenda), add constraint FK_2F54ECD9 foreign key (TipoRenda) references TiposRenda (Id);
alter table PessoaTipoRendas add index (Pessoa), add constraint FK_DC98FCA8 foreign key (Pessoa) references Pessoas (Id);
create table hibernate_unique_key ( next_hi BIGINT );
insert into hibernate_unique_key values ( 1 );
