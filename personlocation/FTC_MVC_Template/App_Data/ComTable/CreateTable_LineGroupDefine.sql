CREATE TABLE [LineGroupDefine](
	[SysID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyID] [nvarchar](50) NOT NULL,
	[FactoryID] [nvarchar](50) NOT NULL,
	[LineGroupName] [nvarchar](100) NOT NULL,
	[LineGroupToken] [nvarchar](200) NOT NULL,
	[MidifyUser] [nvarchar](50) NULL,
	[MidifyTime] [datetime] NULL,
 CONSTRAINT [PK_LineGroupDefine] PRIMARY KEY CLUSTERED 
(
	[SysID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公司編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LineGroupDefine', @level2type=N'COLUMN',@level2name=N'CompanyID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'工廠編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LineGroupDefine', @level2type=N'COLUMN',@level2name=N'FactoryID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Line群組ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LineGroupDefine', @level2type=N'COLUMN',@level2name=N'LineGroupName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Line群組權杖' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LineGroupDefine', @level2type=N'COLUMN',@level2name=N'LineGroupToken'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改人員' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LineGroupDefine', @level2type=N'COLUMN',@level2name=N'MidifyUser'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LineGroupDefine', @level2type=N'COLUMN',@level2name=N'MidifyTime'
GO


