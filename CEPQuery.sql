create database CEP_UPDATE

use CEP_UPDATE

create table Users
(UserCode int not null identity,
 UserLogin varchar(20) not null,
 UserPassword varchar(20) not null,
 AdministratorStatus bit not null default 0,
 UserSurname varchar(20) null,
 UserName varchar(20) null, 
 constraint LoginUniq unique (UserLogin),
 constraint UserCode_PK primary key (UserCode))
 insert into Users (UserLogin, UserPassword, AdministratorStatus, UserName, UserSurname) values ('admin1', 'wrldftnks', 1, '', '')
 insert into Users (UserLogin, UserPassword, AdministratorStatus, UserName, UserSurname) values ('admin2', 'bttlfld', 1, '', '')
 insert into Users (UserLogin, UserPassword, AdministratorStatus, UserName, UserSurname) values ('admin3', 'strwrs_bttlfrnt', 1, '', '')
 insert into Users (UserLogin, UserPassword, AdministratorStatus, UserName, UserSurname) values ('admin', 'qwerty', 1, '', '')
 --drop table Users
 --delete from Users

 create table CurrencyBuy
 (CurrencyBuyCode int not null identity, 
  CurrencyBuyName varchar(40) not null,
  CurrencyBuyCourse float not null,
  constraint CurrencyName unique (CurrencyBuyName),
  constraint CurrencyCode_PK primary key (CurrencyBuyCode))
  insert into CurrencyBuy(CurrencyBuyName, CurrencyBuyCourse) values ('Белорусский рубль (BYR)', 1), ('Американский доллар (USD)', 2.549), ('Евро (EUR)', 2.769), ('Российский рубль (RUB)', 0.03353), ('Украинская гривна (UAH)', 0.078)
  --drop table Currency

 create table CurrencySell
 (CurrencySellCode int not null identity, 
  CurrencySellName varchar(40) not null,
  CurrencySellCourse float not null,
  constraint CurrencyNameSell unique (CurrencySellName),
  constraint CurrencyCodeSell_PK primary key (CurrencySellCode))
  insert into CurrencySell(CurrencySellName, CurrencySellCourse) values ('Белорусский рубль (BYR)', 1),('Американский доллар (USD)', 2.553), ('Евро (EUR)', 2.772), ('Российский рубль (RUB)', 0.03366), ('Украинская гривна (UAH)', 0.0894)
  --drop table CurrencySell

 create table BankCard
 (UserCode int not null default 0,
  CardNumber varchar(16) not null,
  UserSurname varchar(20) not null,
  UserName varchar(20) not null,
  ValidDate varchar(5) not null, 
  CVVCode int not null, 
  constraint CardNumber unique (CardNumber),
  constraint CardNumber_PK primary key (CardNumber), 
  UserMoney float not null,
  CurrencyBuyCode int not null default 1,
  CurrencySellCode int not null default 0,
  constraint UserCode_FK foreign key (UserCode) references Users (UserCode) on delete cascade,
  constraint CurrencyCode_FK foreign key (CurrencyBuyCode) references CurrencyBuy (CurrencyBuyCode) on delete set default on update cascade,
  constraint CurrencyCodeSell_FK foreign key (CurrencySellCode) references CurrencySell (CurrencySellCode) on delete set default on update cascade)
  --drop table BankCard

  create table Statistic
  (CardNumber varchar(16) not null default 0,
   OperationCode int not null identity,
   OperationSum float not null,
   Currency varchar(40) not null,   
   OperationType varchar(40) not null,
   constraint OperationTypeCheck check (OperationType = 'Покупка' or OperationType = 'Продажа' or OperationType = 'Обмен'),
   constraint CardNumber_FK foreign key (CardNumber) references BankCard (CardNumber) on delete cascade,
   constraint OperationCode_PK primary key (OperationCode))
   --drop table Statistic

   create table CurrencyInfo
  (RecordNumber int not null identity,
   CardNumber varchar(16) not null default 0,
   CurrencySum float not null,
   CurrencyName varchar(40),
   constraint CardNumberCI_FK foreign key (CardNumber) references BankCard (CardNumber) on delete cascade,
   constraint RecordNumber_PK primary key (RecordNumber))
   --drop table CurrencyInfo