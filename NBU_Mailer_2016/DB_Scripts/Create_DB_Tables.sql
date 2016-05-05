﻿
USE ANDREW2

CREATE TABLE [SPRUSNBU_BANKS] (
[ID] int IDENTITY(0,1) PRIMARY KEY,
[IDHOST] nvarchar(6) NOT NULL,
[FNHOST] nvarchar(50) NOT NULL,
[MFOM] integer,
[OKPO] integer,
[KTELE] nvarchar(30),
[KFASE] nvarchar(50),
[PARTNER] bit
--[UID] uniqueidentifier NOT NULL DEFAULT (newid())
) ON [PRIMARY];

--FROM:, TO:, F:(1-8.1-4), C:(\d8), D:(\d:12), DATE:, DATE-DELIVERED:, FILE_PATH, ENV_PATH, ENV_NAME
CREATE TABLE [NBU_ENVELOPES] (
[ID] INT NOT NULL IDENTITY(1000,1) PRIMARY KEY,
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
[SPRUSNBU_BANK_ID] INT,
CONSTRAINT fk_bank_envelope FOREIGN KEY (SPRUSNBU_BANK_ID) REFERENCES SPRUSNBU_BANKS (ID)
) ON [PRIMARY];

-- ID = 0 !!!
INSERT INTO SPRUSNBU_BANKS (IDHOST, FNHOST, MFOM, OKPO, KTELE) 
                    VALUES ('U000', '!!! не определён !!!', 1, 1, '771')

-- T E S T !!!
--INSERT INTO NBU_ENVELOPES ([FROM], [TO], [FILE_NAME], FILE_SIZE, FILE_BODY, FILE_DATE, DATE_SENT, DATE_DELIV, ENV_NAME, ENV_PATH, SPRUSNBU_BANK_ID) 
--VALUES ('Sender1', 'Reciever1', 'FileName1', 333, '1/1/1', '1/1/1', '1/1/1', 'E_1_UYTX.ERT', 'Some-path', 1)