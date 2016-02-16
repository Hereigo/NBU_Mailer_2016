USE Andrew2
--FROM:, TO:, F:(1-8.1-4), C:(\d8), D:(\d:12), DATE:, DATE-DELIVERED:, FILE_PATH, ENV_PATH, ENV_NAME
CREATE TABLE [ENVELOPES] (
	[NUM] INT NOT NULL IDENTITY(100001,1) PRIMARY KEY,
	[FROM] nvarchar(15),
	[TO] nvarchar(15),
	[FILE_NAME] nvarchar(15),
	[FILE_SIZE] int,
	[FILE_BODY] IMAGE,
	[FILE_DATE] datetime,
	[DATE_SENT] datetime,
	[DATE_DELIV] datetime,
	[ENV_NAME] nvarchar(15),
	[ENV_PATH] nvarchar(255),
	[UID] uniqueidentifier NOT NULL DEFAULT (newid())
) ON [PRIMARY];

--public string envelopePath { get; }
--public string envelopeName { get; }
--public string sendFromAddress { get; }
--public string recieveAddress { get; }
--public string fileName { get; }
--public ulong fileSize { get; }
--public DateTime fileModified { get; }
--public DateTime fileSent { get; }
--public DateTime fileDelivered { get; }
--public string fileLocation { get; }
