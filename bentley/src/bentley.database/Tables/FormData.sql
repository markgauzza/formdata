CREATE TABLE [dbo].[FormData](
	[FormDataId] [uniqueidentifier] NOT NULL,
	[Subject] [nvarchar](200) NOT NULL,
	[Description] [text] NULL,
	[DueDate] [datetime] NULL,
	[Priority] [int] NULL,
	[Critical] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
 CONSTRAINT [PK_FormData] PRIMARY KEY CLUSTERED 
(
	[FormDataId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[FormData] ADD  CONSTRAINT [DF_FormData_Critical]  DEFAULT ((0)) FOR [Critical]
GO

ALTER TABLE [dbo].[FormData] ADD  CONSTRAINT [DF_FormData_CreatedAt]  DEFAULT (getutcdate()) FOR [CreatedAt]
GO


