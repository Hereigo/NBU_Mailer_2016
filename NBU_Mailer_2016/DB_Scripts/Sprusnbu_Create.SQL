USE Andrew2
CREATE TABLE [SPRUSNBU] (
	[IDHOST] nvarchar(255), 
	[IDHFIRST] nvarchar(255), 
	[FNHOST] nvarchar(255), 
	[WHATUS] nvarchar(255), 
	[TELE] nvarchar(255), 
	[MFOM] nvarchar(255), 
	[OKPO] nvarchar(255), 
	[GROUP] nvarchar(255), 
	[KTELE] nvarchar(255), 
	[KFASE] nvarchar(255), 
	[TYPESEND] nvarchar(255), 
	[Y_N_SEND] nvarchar(255), 
	[P_M_B] nvarchar(255), 
	[N_PATH] nvarchar(255), 
	[N_ADD_PATH] nvarchar(255), 
	[UID] uniqueidentifier NOT NULL DEFAULT (newid())
) ON [PRIMARY];
