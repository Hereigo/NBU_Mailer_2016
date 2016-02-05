USE Andrew2
--FROM:, TO:, F:(1-8.1-4), C:(\d8), D:(\d:12), DATE:, DATE-DELIVERED:, FILE_PATH, ENV_PATH, ENV_NAME
CREATE TABLE [ENVELOPES] (
	[FROM] nvarchar(15),
	[TO] nvarchar(15),
	[FILE] nvarchar(15),
	[FILE_SIZE] int,
	[FILE_DATE] datetime,
	[DATE_SENT] datetime,
	[DATE_DELIV] datetime,
	[FILE_PATH] nvarchar(255),
	[ENV_PATH] nvarchar(255),
	[ENV_NAME] nvarchar(15),
	[ENV_DATE] datetime,
	[UID] uniqueidentifier NOT NULL DEFAULT (newid())
) ON [PRIMARY];
