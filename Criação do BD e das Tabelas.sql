

create database CentralClientes

use CentralClientes
CREATE TABLE dbo.Clientes(
	Id int Identity PRIMARY KEY,
	Nome varchar(100) not null,
	CPF varchar(14) not null,
	DataCadastro datetime not null,
	DataAlteracao datetime NULL,

) 

CREATE TABLE dbo.Contas(
	 Id int Identity PRIMARY KEY ,
	 ClienteId int not null
	 FOREIGN KEY (ClienteId) references Clientes (Id),
	 SaldoPontos int null

)

CREATE TABLE dbo.Enderecos(
	 Id int Identity PRIMARY KEY ,
	 ClienteId int not null
	 FOREIGN KEY (ClienteId) references Clientes (Id),
	 Rua varchar(150) null,
	 Numero int null,
	 Cep char(8) null
)

CREATE TABLE dbo.Telefones(
	 Id int Identity PRIMARY KEY ,
	 ClienteId int not null
	 FOREIGN KEY (ClienteId) references Clientes (Id),
	 DDD int null,
	 Numero Int  null
)

