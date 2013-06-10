Create Table KVS

(Key Varchar2(128),
Value1 Varchar(4000),
Value2 Varchar(4000),
Value3 Varchar(4000),
Value4 Varchar(4000),
Value5 Varchar(4000),
Value6 Varchar(4000),
Value7 Varchar(4000),
Value8 Varchar(4000),
Value9 Varchar(4000),
Expires timestamp(6),
CAS Number(38,0)
);

alter table KVS add constraint PK_KVS_Key primary key (Key)