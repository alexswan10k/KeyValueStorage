create table [KVS]
(
	[Key] Varchar(128) not null,
	[Value] Varchar(MAX),
	[Expires] datetime,
	[Cas] int
)

alter table [KVS]
	add constraint PK_KVS 
	primary key clustered ([Key] ASC)
	
create nonclustered index IX_KVS_Expires on [KVS]([Expires])