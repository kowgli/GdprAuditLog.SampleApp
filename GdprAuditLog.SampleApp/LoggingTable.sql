/****** Object:  Table [dbo].[AuditEntry]    Script Date: 2019-05-09 10:20:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AuditEntry](
	[AuditEntryId] [uniqueidentifier] NOT NULL,
	[DataRetrievedOnUtc] [datetime] NOT NULL,
	[UserLogin] [nvarchar](100) NOT NULL,
	[UserFullName] [nvarchar](100) NOT NULL,
	[EntityName] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Query] [xml] NOT NULL,
	[RowCount] [int] NOT NULL,
	[Data] [xml] NOT NULL,
	[SystemId] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_AuditEntry] PRIMARY KEY CLUSTERED 
(
	[AuditEntryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AuditEntry] ADD  CONSTRAINT [DF_AuditEntry_AuditEntryId]  DEFAULT (newid()) FOR [AuditEntryId]
GO

