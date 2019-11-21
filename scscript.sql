USE [BIGTRONS_PRD]
GO
/****** Object:  Table [dbo].[CApprovalPath]    Script Date: 11/21/2019 4:08:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CApprovalPath](
	[ApprovalPathID] [varchar](10) NOT NULL,
	[RoleID] [varchar](20) NOT NULL,
	[RoleParentID] [varchar](10) NULL,
	[RoleChildID] [varchar](10) NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[TaskTypeID] [varchar](10) NULL,
	[CreatedBy] [varchar](10) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__CApprova__D233CDD4BCD23201] PRIMARY KEY CLUSTERED 
(
	[ApprovalPathID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CMenuAction]    Script Date: 11/21/2019 4:08:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CMenuAction](
	[MenuID] [varchar](50) NOT NULL,
	[ActionID] [varchar](30) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_CMenuAction] PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC,
	[ActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CMenuObject]    Script Date: 11/21/2019 4:08:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CMenuObject](
	[MenuID] [varchar](50) NOT NULL,
	[ObjectID] [varchar](30) NOT NULL,
	[ObjectDesc] [varchar](30) NOT NULL,
	[ObjectLongDesc] [varchar](255) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_CMenuObject] PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC,
	[ObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CNegotiationConfigurations]    Script Date: 11/21/2019 4:08:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CNegotiationConfigurations](
	[NegotiationConfigID] [varchar](20) NOT NULL,
	[NegotiationConfigTypeID] [varchar](20) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[TaskID] [varchar](20) NULL,
	[ParameterValue] [varchar](20) NULL,
	[ParameterValue2] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__CNegotia__2E87C4737E77B618] PRIMARY KEY CLUSTERED 
(
	[NegotiationConfigID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DApprovalDelegationUser]    Script Date: 11/21/2019 4:08:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DApprovalDelegationUser](
	[ApprovalDelegationUserID] [varchar](32) NOT NULL,
	[ApprovalDelegateID] [varchar](32) NOT NULL,
	[DelegateUserID] [nvarchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DApprovalDelegationUser] PRIMARY KEY CLUSTERED 
(
	[ApprovalDelegationUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanBidOpening]    Script Date: 11/21/2019 4:08:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanBidOpening](
	[BPBidOpeningID] [varchar](32) NOT NULL,
	[BudgetPlanID] [varchar](20) NOT NULL,
	[BudgetPlanVersion] [int] NOT NULL,
	[StatusID] [int] NOT NULL,
	[PeriodStart] [datetime2](7) NOT NULL,
	[PeriodEnd] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DBudgetP__32A13893FA1658A8] PRIMARY KEY CLUSTERED 
(
	[BPBidOpeningID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanTCBidOpening]    Script Date: 11/21/2019 4:08:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanTCBidOpening](
	[BPTCBidOpeningID] [varchar](32) NOT NULL,
	[TCMemberID] [varchar](20) NOT NULL,
	[BPBidOpeningID] [varchar](32) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DBudgetP__478D269704702B90] PRIMARY KEY CLUSTERED 
(
	[BPTCBidOpeningID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanTCViewBidOpening]    Script Date: 11/21/2019 4:08:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanTCViewBidOpening](
	[VendorID] [varchar](10) NOT NULL,
	[BPTCBidOpeningID] [varchar](32) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK_DBudgetPlanTCViewBidOpening] PRIMARY KEY CLUSTERED 
(
	[VendorID] ASC,
	[BPTCBidOpeningID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanTemplateStructure]    Script Date: 11/21/2019 4:08:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanTemplateStructure](
	[BudgetPlanTemplateID] [varchar](3) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[Version] [int] NOT NULL,
	[Sequence] [int] NOT NULL,
	[ParentItemID] [varchar](20) NULL,
	[ParentVersion] [int] NULL,
	[ParentSequence] [int] NULL,
	[IsDefault] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DBudgetPlanTemplateStructure] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanTemplateID] ASC,
	[ItemID] ASC,
	[Version] ASC,
	[Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_DBudgetPlanTemplateStructure] UNIQUE NONCLUSTERED 
(
	[BudgetPlanTemplateID] ASC,
	[Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanVersion]    Script Date: 11/21/2019 4:08:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanVersion](
	[BudgetPlanID] [varchar](20) NOT NULL,
	[BudgetPlanVersion] [int] NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[Area] [decimal](10, 4) NOT NULL,
	[Unit] [decimal](10, 4) NOT NULL,
	[FeePercentage] [decimal](6, 2) NOT NULL,
	[StatusID] [int] NOT NULL,
	[IsBidOpen] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
	[BlockNo] [varchar](300) NULL,
 CONSTRAINT [PK_DBudgetPlanVersion] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanID] ASC,
	[BudgetPlanVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanVersionAdditional]    Script Date: 11/21/2019 4:08:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanVersionAdditional](
	[BudgetPlanVersionAdditionalID] [varchar](32) NOT NULL,
	[BudgetPlanVersionVendorID] [varchar](32) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[Version] [int] NOT NULL,
	[Sequence] [int] NOT NULL,
	[ParentItemID] [varchar](20) NULL,
	[ParentVersion] [int] NULL,
	[ParentSequence] [int] NULL,
	[Info] [varchar](1000) NOT NULL,
	[Volume] [decimal](8, 4) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DBudgetPlanVersionAdditional] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanVersionAdditionalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_DBudgetPlanVersionAdditional] UNIQUE NONCLUSTERED 
(
	[BudgetPlanVersionVendorID] ASC,
	[ItemID] ASC,
	[Version] ASC,
	[Sequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanVersionAssignment]    Script Date: 11/21/2019 4:08:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanVersionAssignment](
	[BudgetPlanVersionStructureID] [varchar](32) NOT NULL,
	[VendorID] [varchar](32) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DBudgetPlanVersionAssignment_1] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanVersionStructureID] ASC,
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanVersionEntry]    Script Date: 11/21/2019 4:08:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanVersionEntry](
	[BudgetPlanVersionVendorID] [varchar](32) NOT NULL,
	[BudgetPlanVersionStructureID] [varchar](32) NOT NULL,
	[Info] [varchar](1000) NOT NULL,
	[Volume] [decimal](12, 4) NULL,
	[MaterialAmount] [decimal](14, 4) NULL,
	[WageAmount] [decimal](14, 4) NULL,
	[MiscAmount] [decimal](14, 4) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DBudgetPlanVersionEntry_1] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanVersionVendorID] ASC,
	[BudgetPlanVersionStructureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanVersionMutual]    Script Date: 11/21/2019 4:08:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanVersionMutual](
	[BudgetPlanVersionStructureID] [varchar](32) NOT NULL,
	[Info] [varchar](1000) NOT NULL,
	[Volume] [decimal](14, 4) NULL,
	[MaterialAmount] [decimal](14, 4) NULL,
	[WageAmount] [decimal](14, 4) NULL,
	[MiscAmount] [decimal](14, 4) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DBudgetPlanVersionMutual] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanVersionStructureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanVersionPeriod]    Script Date: 11/21/2019 4:08:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanVersionPeriod](
	[BudgetPlanVersionPeriodID] [varchar](32) NOT NULL,
	[BudgetPlanID] [varchar](20) NOT NULL,
	[BudgetPlanVersion] [int] NOT NULL,
	[PeriodVersion] [int] NOT NULL,
	[BudgetPlanPeriodID] [varchar](2) NOT NULL,
	[StatusID] [int] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DBudgetPlanVersionPeriod] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanVersionPeriodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_DBudgetPlanVersionPeriod] UNIQUE NONCLUSTERED 
(
	[BudgetPlanID] ASC,
	[BudgetPlanVersion] ASC,
	[PeriodVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanVersionStructure]    Script Date: 11/21/2019 4:08:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanVersionStructure](
	[BudgetPlanVersionStructureID] [varchar](32) NOT NULL,
	[BudgetPlanID] [varchar](20) NOT NULL,
	[BudgetPlanVersion] [int] NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[Version] [int] NOT NULL,
	[Sequence] [int] NOT NULL,
	[ParentItemID] [varchar](20) NULL,
	[ParentVersion] [int] NULL,
	[ParentSequence] [int] NULL,
	[ItemVersionChildID] [varchar](8) NULL,
	[Specification] [varchar](1000) NOT NULL,
	[Volume] [decimal](12, 4) NULL,
	[MaterialAmount] [decimal](14, 4) NULL,
	[WageAmount] [decimal](14, 4) NULL,
	[MiscAmount] [decimal](14, 4) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DBudgetPlanVersionStructure] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanVersionStructureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_DBudgetPlanVersionStructure] UNIQUE NONCLUSTERED 
(
	[BudgetPlanID] ASC,
	[BudgetPlanVersion] ASC,
	[ItemID] ASC,
	[Version] ASC,
	[Sequence] ASC,
	[ParentItemID] ASC,
	[ParentVersion] ASC,
	[ParentSequence] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DBudgetPlanVersionVendor]    Script Date: 11/21/2019 4:08:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBudgetPlanVersionVendor](
	[BudgetPlanVersionVendorID] [varchar](32) NOT NULL,
	[BudgetPlanVersionPeriodID] [varchar](32) NOT NULL,
	[VendorID] [varchar](32) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[FeePercentage] [decimal](6, 4) NOT NULL,
	[StatusID] [int] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
	[ScheduleID] [varchar](32) NULL,
 CONSTRAINT [PK_DBudgetPlanVersionVendor_1] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanVersionVendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_DBudgetPlanVersionVendor] UNIQUE NONCLUSTERED 
(
	[BudgetPlanVersionPeriodID] ASC,
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DCatalogCartItems]    Script Date: 11/21/2019 4:08:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DCatalogCartItems](
	[CatalogCartItemID] [varchar](32) NOT NULL,
	[CatalogCartID] [varchar](32) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[Qty] [decimal](12, 4) NOT NULL,
	[Amount] [decimal](15, 4) NOT NULL,
	[ItemPriceID] [varchar](32) NULL,
	[VendorID] [nvarchar](10) NULL,
	[ValidFrom] [datetime] NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [nvarchar](20) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [nvarchar](20) NULL,
 CONSTRAINT [PK__DCatalog__AC960BA0625193B5] PRIMARY KEY CLUSTERED 
(
	[CatalogCartItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DEmpCommunication]    Script Date: 11/21/2019 4:08:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DEmpCommunication](
	[EmployeeID] [varchar](20) NOT NULL,
	[CommunicationTypeID] [varchar](4) NOT NULL,
	[CommunicationDesc] [varchar](40) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_DEmpCommunication] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC,
	[CommunicationTypeID] ASC,
	[CommunicationDesc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DEmpOrgAss]    Script Date: 11/21/2019 4:08:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DEmpOrgAss](
	[EmployeeID] [varchar](20) NOT NULL,
	[PersonnelAreaID] [varchar](4) NULL,
	[PersonnelSubareaID] [varchar](4) NULL,
	[EmployeeGroupID] [char](1) NULL,
	[EmployeeSubgroupID] [varchar](2) NULL,
	[WorkContractID] [varchar](2) NULL,
	[PositionID] [varchar](12) NULL,
	[Grade] [varchar](8) NULL,
	[Level] [tinyint] NULL,
	[SupervisorID] [varchar](20) NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_DEmpOrgAss] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DEventPIC]    Script Date: 11/21/2019 4:08:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DEventPIC](
	[EventPICID] [varchar](20) NOT NULL,
	[PICID] [varchar](20) NOT NULL,
	[IsAttend] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[FPTID] [varchar](50) NOT NULL,
	[PICTypeID] [varchar](20) NOT NULL,
	[FunctionID] [varchar](20) NOT NULL,
 CONSTRAINT [PK__DEventPI__293CD050733E33F4] PRIMARY KEY CLUSTERED 
(
	[EventPICID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTAdditionalInfo]    Script Date: 11/21/2019 4:08:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTAdditionalInfo](
	[FPTAdditionalInfoID] [varchar](32) NOT NULL,
	[FPTID] [varchar](50) NULL,
	[FPTAdditionalInfoItemID] [varchar](32) NULL,
	[Value] [varchar](255) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK_DFPTAdditionalInfo] PRIMARY KEY CLUSTERED 
(
	[FPTAdditionalInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTDeviations]    Script Date: 11/21/2019 4:08:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTDeviations](
	[FPTDeviationID] [varchar](20) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[DeviationTypeID] [varchar](20) NOT NULL,
	[RefNumber] [varchar](50) NULL,
	[RefTitle] [varchar](255) NULL,
	[RefDate] [date] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DFPTDevi__805A954421FF8670] PRIMARY KEY CLUSTERED 
(
	[FPTDeviationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTHistories]    Script Date: 11/21/2019 4:08:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTHistories](
	[FPTHistoryID] [varchar](20) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[Descriptions] [varchar](50) NULL,
	[Remarks] [varchar](max) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DFPTHist__69D2B12AADBECDEA] PRIMARY KEY CLUSTERED 
(
	[FPTHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTNegotiationRounds]    Script Date: 11/21/2019 4:08:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTNegotiationRounds](
	[RoundID] [varchar](32) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[StartDateTimeStamp] [datetime] NOT NULL,
	[EndDateTimeStamp] [datetime] NOT NULL,
	[Remarks] [varchar](50) NULL,
	[TopValue] [decimal](18, 2) NOT NULL,
	[BottomValue] [decimal](18, 2) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DFPTNego__94D84E1A284D2E58] PRIMARY KEY CLUSTERED 
(
	[RoundID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTProjects]    Script Date: 11/21/2019 4:08:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTProjects](
	[FPTProjectID] [varchar](20) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[ProjectID] [varchar](20) NOT NULL,
	[BudgetPlanID] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DFPTProj__AD7B7226B0D039EA] PRIMARY KEY CLUSTERED 
(
	[FPTProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTStatus]    Script Date: 11/21/2019 4:08:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTStatus](
	[FPTStatusID] [varchar](20) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[StatusDateTimeStamp] [datetime] NOT NULL,
	[StatusID] [int] NOT NULL,
	[Remarks] [varchar](max) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedHost] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK__DFPTStat__245FBDACBFC1D180] PRIMARY KEY CLUSTERED 
(
	[FPTStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTTCParticipants]    Script Date: 11/21/2019 4:08:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTTCParticipants](
	[FPTTCParticipantID] [varchar](32) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[TCMemberID] [varchar](20) NOT NULL,
	[StatusID] [bit] NOT NULL,
	[IsDelegation] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK_DFPTTCParticipants] PRIMARY KEY CLUSTERED 
(
	[FPTTCParticipantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTVendorParticipants]    Script Date: 11/21/2019 4:08:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTVendorParticipants](
	[FPTVendorParticipantID] [varchar](32) NOT NULL,
	[NegotiationConfigID] [varchar](20) NOT NULL,
	[VendorID] [varchar](32) NOT NULL,
	[StatusID] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DFPTVend__DE256024538D5813] PRIMARY KEY CLUSTERED 
(
	[FPTVendorParticipantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTVendorRecommendations]    Script Date: 11/21/2019 4:08:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTVendorRecommendations](
	[VendorRecommendationID] [varchar](32) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[TaskID] [varchar](20) NULL,
	[TCMemberID] [varchar](20) NOT NULL,
	[FPTVendorParticipantID] [varchar](32) NOT NULL,
	[IsProposed] [bit] NULL,
	[IsWinner] [bit] NULL,
	[Remarks] [varchar](max) NULL,
	[LetterNumber] [varchar](4) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DFPTVend__B6F5267504155DC7] PRIMARY KEY CLUSTERED 
(
	[VendorRecommendationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DFPTVendorWinner]    Script Date: 11/21/2019 4:08:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DFPTVendorWinner](
	[VendorWinnerID] [varchar](32) NOT NULL,
	[IsWinner] [bit] NULL,
	[FPTID] [varchar](50) NOT NULL,
	[FPTVendorParticipantID] [varchar](32) NOT NULL,
	[NegotiationEntryID] [varchar](32) NULL,
	[BidFee] [decimal](6, 2) NULL,
	[TaskID] [varchar](20) NULL,
	[LetterNumber] [varchar](4) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DFPTVend__F77A175C64CB7BC9] PRIMARY KEY CLUSTERED 
(
	[VendorWinnerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemComparisonDetails]    Script Date: 11/21/2019 4:08:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemComparisonDetails](
	[ItemComparisonDetailID] [varchar](32) NOT NULL,
	[ItemComparisonID] [varchar](32) NULL,
	[ItemPriceID] [varchar](32) NULL,
	[VendorID] [varchar](10) NULL,
	[ValidFrom] [datetime] NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [nvarchar](20) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [nvarchar](20) NULL,
 CONSTRAINT [PK__DItemCom__28C8CAC6C1EF6545] PRIMARY KEY CLUSTERED 
(
	[ItemComparisonDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemComparisonSession]    Script Date: 11/21/2019 4:08:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemComparisonSession](
	[ItemID] [varchar](20) NULL,
	[ItemName] [varchar](100) NULL,
	[ItemSecondaryName] [varchar](max) NULL,
	[SessionID] [varchar](100) NULL,
	[GroupGUID] [varchar](100) NULL,
	[ComparisonDetilID] [nvarchar](32) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemDetails]    Script Date: 11/21/2019 4:08:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemDetails](
	[ItemDetailID] [varchar](32) NOT NULL,
	[ItemDetailDesc] [varchar](1000) NOT NULL,
	[ItemDetailTypeID] [varchar](50) NOT NULL,
	[VendorID] [varchar](10) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [nvarchar](20) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [nvarchar](20) NULL,
 CONSTRAINT [PK__DItemDet__C0317991CE4952A4] PRIMARY KEY CLUSTERED 
(
	[ItemDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemDetailUpload]    Script Date: 11/21/2019 4:08:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemDetailUpload](
	[ItemDetailUploadID] [varchar](32) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[ItemDetailID] [varchar](32) NOT NULL,
	[ItemDetailDesc] [varchar](1000) NOT NULL,
	[ItemDetailTypeID] [varchar](10) NOT NULL,
	[ItemDetailTypeDesc] [varchar](250) NOT NULL,
	[VendorID] [varchar](10) NOT NULL,
	[TaskID] [varchar](20) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [nvarchar](20) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [nvarchar](20) NULL,
 CONSTRAINT [PK_DItemDetailUpload] PRIMARY KEY CLUSTERED 
(
	[ItemDetailUploadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemGroupParameter]    Script Date: 11/21/2019 4:08:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemGroupParameter](
	[ItemGroupID] [varchar](3) NOT NULL,
	[ParameterID] [varchar](3) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DItemGroupParameter] PRIMARY KEY CLUSTERED 
(
	[ItemGroupID] ASC,
	[ParameterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemParameter]    Script Date: 11/21/2019 4:08:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemParameter](
	[ItemID] [varchar](20) NOT NULL,
	[ItemGroupID] [varchar](3) NOT NULL,
	[ParameterID] [varchar](3) NOT NULL,
	[Value] [varchar](100) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DItemParameter] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[ItemGroupID] ASC,
	[ParameterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemPrice]    Script Date: 11/21/2019 4:08:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemPrice](
	[ItemPriceID] [varchar](32) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[RegionID] [varchar](3) NOT NULL,
	[ProjectID] [varchar](4) NOT NULL,
	[ClusterID] [varchar](3) NOT NULL,
	[UnitTypeID] [varchar](5) NOT NULL,
	[PriceTypeID] [varchar](3) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DItemPrice] PRIMARY KEY CLUSTERED 
(
	[ItemPriceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_DItemPrice] UNIQUE NONCLUSTERED 
(
	[ItemID] ASC,
	[RegionID] ASC,
	[ProjectID] ASC,
	[ClusterID] ASC,
	[UnitTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemPriceUpload]    Script Date: 11/21/2019 4:08:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemPriceUpload](
	[ItemPriceUploadID] [varchar](32) NOT NULL,
	[ItemPriceID] [varchar](32) NULL,
	[ItemID] [varchar](20) NOT NULL,
	[RegionID] [varchar](3) NOT NULL,
	[ProjectID] [varchar](4) NOT NULL,
	[ClusterID] [varchar](3) NOT NULL,
	[UnitTypeID] [varchar](5) NOT NULL,
	[PriceTypeID] [varchar](3) NOT NULL,
	[VendorID] [varchar](10) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [datetime] NOT NULL,
	[CurrencyID] [varchar](3) NOT NULL,
	[Amount] [decimal](15, 4) NOT NULL,
	[TaskID] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
	[ItemDesc] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DItemPriceUpload] PRIMARY KEY CLUSTERED 
(
	[ItemPriceUploadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemPriceVendor]    Script Date: 11/21/2019 4:08:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemPriceVendor](
	[ItemPriceID] [varchar](32) NOT NULL,
	[VendorID] [varchar](10) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DItemPriceVendor] PRIMARY KEY CLUSTERED 
(
	[ItemPriceID] ASC,
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemPriceVendor_]    Script Date: 11/21/2019 4:08:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemPriceVendor_](
	[ItemPriceID] [varchar](32) NOT NULL,
	[VendorID] [varchar](10) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemPriceVendorPeriod]    Script Date: 11/21/2019 4:08:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemPriceVendorPeriod](
	[ItemPriceID] [varchar](32) NOT NULL,
	[VendorID] [varchar](10) NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[CurrencyID] [varchar](3) NOT NULL,
	[Amount] [decimal](15, 4) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
	[TaskID] [varchar](20) NULL,
	[StatusID] [varchar](20) NULL,
 CONSTRAINT [PK_DItemPriceVendorPeriod] PRIMARY KEY CLUSTERED 
(
	[ItemPriceID] ASC,
	[VendorID] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemPriceVendorPeriod_]    Script Date: 11/21/2019 4:08:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemPriceVendorPeriod_](
	[ItemPriceID] [varchar](32) NOT NULL,
	[VendorID] [varchar](10) NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[CurrencyID] [varchar](3) NOT NULL,
	[Amount] [decimal](15, 4) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemVersion]    Script Date: 11/21/2019 4:08:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemVersion](
	[ItemID] [varchar](20) NOT NULL,
	[Version] [int] NOT NULL,
	[VersionDesc] [varchar](100) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DItemVersion] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemVersionChild]    Script Date: 11/21/2019 4:08:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemVersionChild](
	[ItemVersionChildID] [varchar](8) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[Version] [int] NOT NULL,
	[ChildItemID] [varchar](20) NOT NULL,
	[ChildVersion] [int] NOT NULL,
	[Sequence] [int] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DItemVersionChild] PRIMARY KEY CLUSTERED 
(
	[ItemVersionChildID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UX_DItemVersionChild] UNIQUE NONCLUSTERED 
(
	[ItemID] ASC,
	[Version] ASC,
	[ChildItemID] ASC,
	[ChildVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemVersionChildAlt]    Script Date: 11/21/2019 4:08:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemVersionChildAlt](
	[ItemVersionChildID] [varchar](8) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[Version] [int] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DItemVersionChildAlt] PRIMARY KEY CLUSTERED 
(
	[ItemVersionChildID] ASC,
	[ItemID] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DItemVersionChildFormula]    Script Date: 11/21/2019 4:08:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DItemVersionChildFormula](
	[ItemVersionChildID] [varchar](8) NOT NULL,
	[Coefficient] [decimal](7, 4) NOT NULL,
	[Formula] [varchar](1000) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DItemVersionChildFormula] PRIMARY KEY CLUSTERED 
(
	[ItemVersionChildID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DNegotiationBidEntry]    Script Date: 11/21/2019 4:08:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DNegotiationBidEntry](
	[NegotiationEntryID] [varchar](32) NOT NULL,
	[BidTypeID] [varchar](20) NOT NULL,
	[NegotiationBidID] [varchar](32) NOT NULL,
	[RoundID] [varchar](32) NULL,
	[FPTVendorParticipantID] [varchar](32) NOT NULL,
	[BidValue] [decimal](18, 2) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DNegotia__30B6174D03275C9C] PRIMARY KEY CLUSTERED 
(
	[NegotiationEntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DNotificationMap]    Script Date: 11/21/2019 4:08:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DNotificationMap](
	[NotifMapID] [varchar](32) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[FunctionID] [varchar](20) NOT NULL,
	[NotificationTemplateID] [varchar](20) NOT NULL,
 CONSTRAINT [PK__DNotific__22F6A9A9BE776E51] PRIMARY KEY CLUSTERED 
(
	[NotifMapID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DPackageList]    Script Date: 11/21/2019 4:08:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DPackageList](
	[PackageID] [varchar](20) NOT NULL,
	[BudgetPlanID] [varchar](20) NOT NULL,
	[BudgetPlanVersion] [int] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DPackageList] PRIMARY KEY CLUSTERED 
(
	[PackageID] ASC,
	[BudgetPlanID] ASC,
	[BudgetPlanVersion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DPreBuildRecipients]    Script Date: 11/21/2019 4:08:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DPreBuildRecipients](
	[PreBuildRecID] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[PreBuildRecTemplateID] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[RecipientTypeID] [varchar](20) NOT NULL,
 CONSTRAINT [PK__DPreBuil__4BC5A45EBE3E1074] PRIMARY KEY CLUSTERED 
(
	[PreBuildRecID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DRecipients]    Script Date: 11/21/2019 4:08:14 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DRecipients](
	[RecipientID] [varchar](32) NOT NULL,
	[RecipientDesc] [varchar](50) NULL,
	[MailAddress] [varchar](100) NULL,
	[OwnerID] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[RecipientTypeID] [varchar](20) NOT NULL,
	[MailNotificationID] [varchar](32) NOT NULL,
 CONSTRAINT [PK__DRecipie__F0A601ADA3F1F525] PRIMARY KEY CLUSTERED 
(
	[RecipientID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DRoleFunction]    Script Date: 11/21/2019 4:08:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DRoleFunction](
	[RoleID] [varchar](8) NOT NULL,
	[FunctionID] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DRoleFunction] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[FunctionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DRoleMenuAction]    Script Date: 11/21/2019 4:08:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DRoleMenuAction](
	[RoleID] [varchar](8) NOT NULL,
	[MenuID] [varchar](50) NOT NULL,
	[ActionID] [varchar](30) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DRoleMenuAction] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[MenuID] ASC,
	[ActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DRoleMenuAction_temp]    Script Date: 11/21/2019 4:08:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DRoleMenuAction_temp](
	[RoleID] [varchar](8) NOT NULL,
	[MenuID] [varchar](50) NOT NULL,
	[ActionID] [varchar](30) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DRoleMenuAction_tempQA]    Script Date: 11/21/2019 4:08:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DRoleMenuAction_tempQA](
	[RoleID] [varchar](8) NOT NULL,
	[MenuID] [varchar](50) NOT NULL,
	[ActionID] [varchar](30) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DRoleMenuActionbackup]    Script Date: 11/21/2019 4:08:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DRoleMenuActionbackup](
	[RoleID] [varchar](8) NOT NULL,
	[MenuID] [varchar](50) NOT NULL,
	[ActionID] [varchar](30) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DRoleMenuObject]    Script Date: 11/21/2019 4:08:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DRoleMenuObject](
	[RoleID] [varchar](8) NOT NULL,
	[MenuID] [varchar](50) NOT NULL,
	[ObjectID] [varchar](30) NOT NULL,
	[Value] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DRoleMenuObject] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[MenuID] ASC,
	[ObjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DTaskDetails]    Script Date: 11/21/2019 4:08:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DTaskDetails](
	[TaskDetailID] [varchar](32) NOT NULL,
	[TaskID] [varchar](20) NOT NULL,
	[StatusID] [int] NOT NULL,
	[Remarks] [varchar](max) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DTaskDet__989EE279401FD81D] PRIMARY KEY CLUSTERED 
(
	[TaskDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DTCFunctions]    Script Date: 11/21/2019 4:08:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DTCFunctions](
	[TCFunctionID] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[TCMemberID] [varchar](20) NOT NULL,
	[FunctionID] [varchar](20) NOT NULL,
 CONSTRAINT [PK__DTCFunct__1E0A06505C1CC302] PRIMARY KEY CLUSTERED 
(
	[TCFunctionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DTemplateTags]    Script Date: 11/21/2019 4:08:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DTemplateTags](
	[TemplateTagID] [varchar](32) NOT NULL,
	[TemplateID] [varchar](20) NOT NULL,
	[FieldTagID] [varchar](20) NOT NULL,
	[TemplateType] [varchar](5) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK_DNotificationTemplateTags] PRIMARY KEY CLUSTERED 
(
	[TemplateTagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DUserBudgetPlanAccess]    Script Date: 11/21/2019 4:08:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DUserBudgetPlanAccess](
	[UserID] [nvarchar](40) NOT NULL,
	[BudgetPlanTemplateID] [varchar](3) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DUserBudgetPlanAccess] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[BudgetPlanTemplateID] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DUserRole]    Script Date: 11/21/2019 4:08:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DUserRole](
	[UserID] [nvarchar](20) NOT NULL,
	[RoleID] [varchar](8) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_DUserRole] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC,
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DVendorCommunications]    Script Date: 11/21/2019 4:08:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DVendorCommunications](
	[VendorCommID] [varchar](32) NOT NULL,
	[CommunicationTypeID] [varchar](32) NOT NULL,
	[VendorPICID] [varchar](32) NOT NULL,
	[IsDefault] [bit] NULL,
	[CommDesc] [varchar](50) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DVendorC__C9CB3F2D839CE961] PRIMARY KEY CLUSTERED 
(
	[VendorCommID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DVendorPICs]    Script Date: 11/21/2019 4:08:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DVendorPICs](
	[VendorPICID] [varchar](32) NOT NULL,
	[VendorID] [varchar](32) NOT NULL,
	[PICName] [varchar](100) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__DVendorP__8CFF83C9F46FE840] PRIMARY KEY CLUSTERED 
(
	[VendorPICID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAdditionalInfoItems]    Script Date: 11/21/2019 4:08:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAdditionalInfoItems](
	[FPTAdditionalInfoItemID] [varchar](32) NOT NULL,
	[Descriptions] [varchar](255) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK_MAdditionalInfoItems] PRIMARY KEY CLUSTERED 
(
	[FPTAdditionalInfoItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MApprovalDelegation]    Script Date: 11/21/2019 4:08:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MApprovalDelegation](
	[ApprovalDelegateID] [varchar](32) NOT NULL,
	[UserID] [nvarchar](20) NOT NULL,
	[TaskTypeID] [varchar](10) NOT NULL,
	[PeriodStart] [datetime] NOT NULL,
	[PeriodEnd] [datetime] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MApprovalDelegation] PRIMARY KEY CLUSTERED 
(
	[ApprovalDelegateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MBidTypes]    Script Date: 11/21/2019 4:08:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MBidTypes](
	[BidTypeID] [varchar](20) NOT NULL,
	[Descriptions] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MBidType__EF38F757F10527BE] PRIMARY KEY CLUSTERED 
(
	[BidTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MBudgetPlanTemplate]    Script Date: 11/21/2019 4:08:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MBudgetPlanTemplate](
	[BudgetPlanTemplateID] [varchar](3) NOT NULL,
	[BudgetPlanTemplateDesc] [varchar](40) NOT NULL,
	[BudgetPlanTypeID] [varchar](3) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MBudgetPlanTemplate] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanTemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MBudgetPlanType]    Script Date: 11/21/2019 4:08:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MBudgetPlanType](
	[BudgetPlanTypeID] [varchar](3) NOT NULL,
	[BudgetPlanTypeDesc] [varchar](40) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MBudgetPlanType] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MBusinessUnit]    Script Date: 11/21/2019 4:08:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MBusinessUnit](
	[BusinessUnitID] [varchar](32) NOT NULL,
	[Descriptions] [varchar](50) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MTCDivis__5481588B4CBD8E3A] PRIMARY KEY CLUSTERED 
(
	[BusinessUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MCluster]    Script Date: 11/21/2019 4:08:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MCluster](
	[ClusterID] [varchar](3) NOT NULL,
	[ClusterDesc] [varchar](20) NOT NULL,
	[ProjectID] [varchar](32) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MCluster] PRIMARY KEY CLUSTERED 
(
	[ClusterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MCommunicationTypes]    Script Date: 11/21/2019 4:08:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MCommunicationTypes](
	[CommunicationTypeID] [varchar](32) NOT NULL,
	[CommTypeDesc] [varchar](35) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MCommuni__7F9554828C375F2C] PRIMARY KEY CLUSTERED 
(
	[CommunicationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MCompany]    Script Date: 11/21/2019 4:08:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MCompany](
	[CompanyID] [varchar](32) NOT NULL,
	[CompanyDesc] [varchar](50) NOT NULL,
	[CountryID] [varchar](3) NOT NULL,
	[City] [varchar](25) NOT NULL,
	[Street] [varchar](255) NOT NULL,
	[Postal] [varchar](6) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MCompany] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MCountry]    Script Date: 11/21/2019 4:08:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MCountry](
	[CountryID] [varchar](3) NOT NULL,
	[CountryDesc] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MCountry] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MCurrency]    Script Date: 11/21/2019 4:08:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MCurrency](
	[CurrencyID] [varchar](3) NOT NULL,
	[CurrencyDesc] [varchar](40) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MCurrency] PRIMARY KEY CLUSTERED 
(
	[CurrencyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MDeviationTypes]    Script Date: 11/21/2019 4:08:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MDeviationTypes](
	[DeviationTypeID] [varchar](20) NOT NULL,
	[Descriptions] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MDeviati__2A46D8ED37176E58] PRIMARY KEY CLUSTERED 
(
	[DeviationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MDivision]    Script Date: 11/21/2019 4:08:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MDivision](
	[DivisionID] [varchar](32) NOT NULL,
	[DivisionDesc] [varchar](50) NOT NULL,
	[BusinessUnitID] [varchar](32) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MDivision] PRIMARY KEY CLUSTERED 
(
	[DivisionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MEmployee]    Script Date: 11/21/2019 4:08:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MEmployee](
	[EmployeeID] [varchar](20) NOT NULL,
	[FirstName] [nvarchar](40) NOT NULL,
	[MiddleName] [nvarchar](40) NOT NULL,
	[LastName] [nvarchar](80) NULL,
	[Salutation] [char](1) NOT NULL,
	[Gender] [char](1) NOT NULL,
	[LanguageID] [char](1) NULL,
	[BirthDate] [datetime] NULL,
	[BirthPlace] [varchar](40) NULL,
	[BirthCountry] [varchar](3) NULL,
	[Nationality] [varchar](3) NULL,
	[MaritalStatus] [char](1) NULL,
	[Religion] [varchar](2) NULL,
	[BloodType] [varchar](2) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[JoinDate] [datetime] NULL,
	[PermanentDate] [datetime] NULL,
	[ResignDate] [datetime] NULL,
	[RemunerationType] [varchar](4) NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_MEmployee] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MEmployee_]    Script Date: 11/21/2019 4:08:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MEmployee_](
	[EmployeeID] [varchar](20) NOT NULL,
	[FirstName] [nvarchar](40) NOT NULL,
	[MiddleName] [nvarchar](40) NOT NULL,
	[LastName] [nvarchar](80) NULL,
	[Salutation] [char](1) NOT NULL,
	[Gender] [char](1) NOT NULL,
	[LanguageID] [char](1) NULL,
	[BirthDate] [datetime] NULL,
	[BirthPlace] [varchar](40) NULL,
	[BirthCountry] [varchar](3) NULL,
	[Nationality] [varchar](3) NULL,
	[MaritalStatus] [char](1) NULL,
	[Religion] [varchar](2) NULL,
	[BloodType] [varchar](2) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[JoinDate] [datetime] NULL,
	[PermanentDate] [datetime] NULL,
	[ResignDate] [datetime] NULL,
	[RemunerationType] [varchar](4) NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MEmployeeGroup]    Script Date: 11/21/2019 4:08:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MEmployeeGroup](
	[EmployeeGroupID] [char](1) NOT NULL,
	[EmployeeGroupDesc] [varchar](20) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_MEmployeeGroup] PRIMARY KEY CLUSTERED 
(
	[EmployeeGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MEmployeeSubgroup]    Script Date: 11/21/2019 4:08:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MEmployeeSubgroup](
	[EmployeeSubgroupID] [varchar](2) NOT NULL,
	[EmployeeSubgroupDesc] [varchar](20) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_MEmployeeSubgroup] PRIMARY KEY CLUSTERED 
(
	[EmployeeSubgroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MFieldTagReferences]    Script Date: 11/21/2019 4:08:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MFieldTagReferences](
	[FieldTagID] [varchar](20) NOT NULL,
	[TagDesc] [varchar](100) NOT NULL,
	[RefTable] [varchar](20) NULL,
	[RefIDColumn] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MFieldTa__1156B630F4DE9223] PRIMARY KEY CLUSTERED 
(
	[FieldTagID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MFPT]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MFPT](
	[FPTID] [varchar](50) NOT NULL,
	[Descriptions] [varchar](max) NULL,
	[ClusterID] [varchar](3) NULL,
	[ProjectID] [varchar](32) NULL,
	[DivisionID] [varchar](32) NULL,
	[BusinessUnitID] [varchar](32) NULL,
	[IsSync] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MFPT__3F478E19E82D64C5] PRIMARY KEY CLUSTERED 
(
	[FPTID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MFunctions]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MFunctions](
	[FunctionID] [varchar](20) NOT NULL,
	[FunctionDesc] [varchar](100) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MFunctio__31ABF91835853F13] PRIMARY KEY CLUSTERED 
(
	[FunctionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MHolidays]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MHolidays](
	[HolidayID] [varchar](20) NOT NULL,
	[Descriptions] [varchar](50) NULL,
	[HolidayDate] [date] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MHoliday__2D35D59A279527E6] PRIMARY KEY CLUSTERED 
(
	[HolidayID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MItem]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MItem](
	[ItemID] [varchar](20) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[ItemGroupID] [varchar](3) NOT NULL,
	[UoMID] [varchar](3) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MItem] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MItemComparison]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MItemComparison](
	[ItemComparisonID] [varchar](32) NOT NULL,
	[ItemComparisonDesc] [nvarchar](50) NOT NULL,
	[UserID] [nvarchar](20) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [nvarchar](20) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [nvarchar](20) NULL,
 CONSTRAINT [PK__MItemCom__08672FDBB9497D08] PRIMARY KEY CLUSTERED 
(
	[ItemComparisonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MItemDetailTypes]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MItemDetailTypes](
	[ItemDetailTypeID] [varchar](50) NOT NULL,
	[ItemDetailTypeDesc] [varchar](250) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK__MItemDet__A8D277EAA7243034] PRIMARY KEY CLUSTERED 
(
	[ItemDetailTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MItemGroup]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MItemGroup](
	[ItemGroupID] [varchar](3) NOT NULL,
	[ItemGroupDesc] [varchar](50) NOT NULL,
	[ItemTypeID] [varchar](3) NOT NULL,
	[HasParameter] [bit] NOT NULL,
	[HasPrice] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MItemGroup] PRIMARY KEY CLUSTERED 
(
	[ItemGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MItemShowCase]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MItemShowCase](
	[ShowCaseID] [varchar](32) NOT NULL,
	[Filename] [varchar](100) NOT NULL,
	[ContentType] [varchar](50) NOT NULL,
	[RawData] [ntext] NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[VendorID] [varchar](10) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK__MItemSho__517556F14A087BB7] PRIMARY KEY CLUSTERED 
(
	[ShowCaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MItemType]    Script Date: 11/21/2019 4:08:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MItemType](
	[ItemTypeID] [varchar](3) NOT NULL,
	[ItemTypeDesc] [varchar](40) NOT NULL,
	[ItemTypeParentID] [varchar](3) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MItemType] PRIMARY KEY CLUSTERED 
(
	[ItemTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MItemUpload]    Script Date: 11/21/2019 4:08:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MItemUpload](
	[ItemUploadID] [varchar](32) NOT NULL,
	[ItemID] [varchar](20) NOT NULL,
	[ItemDesc] [varchar](100) NOT NULL,
	[ItemGroupID] [varchar](3) NOT NULL,
	[UoMID] [varchar](3) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[TaskID] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MItemUpload] PRIMARY KEY CLUSTERED 
(
	[ItemUploadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MLocation]    Script Date: 11/21/2019 4:08:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MLocation](
	[LocationID] [varchar](3) NOT NULL,
	[LocationDesc] [varchar](20) NOT NULL,
	[RegionID] [varchar](3) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MLocation] PRIMARY KEY CLUSTERED 
(
	[LocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MMailNotifications]    Script Date: 11/21/2019 4:08:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MMailNotifications](
	[MailNotificationID] [varchar](32) NOT NULL,
	[Importance] [bit] NULL,
	[Subject] [varchar](255) NULL,
	[Contents] [ntext] NULL,
	[StatusID] [int] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[TaskID] [varchar](20) NULL,
	[FPTID] [varchar](50) NULL,
	[FunctionID] [varchar](20) NOT NULL,
	[NotificationTemplateID] [varchar](20) NOT NULL,
 CONSTRAINT [PK__MMailNot__6317238E882CF202] PRIMARY KEY CLUSTERED 
(
	[MailNotificationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MMinuteTemplates]    Script Date: 11/21/2019 4:08:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MMinuteTemplates](
	[MinuteTemplateID] [varchar](20) NOT NULL,
	[MinuteTemplateDescriptions] [varchar](100) NULL,
	[Contents] [ntext] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[FunctionID] [varchar](20) NULL,
 CONSTRAINT [PK__MMinuteT__5CDDF699989A8A50] PRIMARY KEY CLUSTERED 
(
	[MinuteTemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MNotificationTemplates]    Script Date: 11/21/2019 4:08:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MNotificationTemplates](
	[NotificationTemplateID] [varchar](20) NOT NULL,
	[NotificationTemplateDesc] [varchar](50) NULL,
	[Contents] [ntext] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MNotific__9C914C2B3BD30A03] PRIMARY KEY CLUSTERED 
(
	[NotificationTemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MParameter]    Script Date: 11/21/2019 4:08:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MParameter](
	[ParameterID] [varchar](3) NOT NULL,
	[ParameterDesc] [varchar](40) NOT NULL,
	[DataType] [varchar](10) NOT NULL,
	[Length] [int] NOT NULL,
	[Precision] [int] NOT NULL,
	[Scale] [int] NOT NULL,
	[RefTable] [varchar](40) NOT NULL,
	[RefIDColumn] [varchar](40) NOT NULL,
	[RefDescColumn] [varchar](40) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MParameter] PRIMARY KEY CLUSTERED 
(
	[ParameterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MPersonnelArea]    Script Date: 11/21/2019 4:08:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MPersonnelArea](
	[PersonnelAreaID] [varchar](4) NOT NULL,
	[CompanyID] [varchar](32) NOT NULL,
	[PersonnelAreaDesc] [nvarchar](30) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_MPersonnelArea] PRIMARY KEY CLUSTERED 
(
	[PersonnelAreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MPersonnelSubarea]    Script Date: 11/21/2019 4:08:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MPersonnelSubarea](
	[PersonnelAreaID] [varchar](4) NOT NULL,
	[PersonnelSubareaID] [varchar](4) NOT NULL,
	[PersonnelSubareaDesc] [nvarchar](15) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_MPersonnelSubarea] PRIMARY KEY CLUSTERED 
(
	[PersonnelAreaID] ASC,
	[PersonnelSubareaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MPICTypes]    Script Date: 11/21/2019 4:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MPICTypes](
	[PICTypeID] [varchar](20) NOT NULL,
	[Descriptions] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MPICType__2DD598C943BC28B7] PRIMARY KEY CLUSTERED 
(
	[PICTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MPosition]    Script Date: 11/21/2019 4:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MPosition](
	[PositionID] [varchar](12) NOT NULL,
	[PositionAltID] [varchar](25) NOT NULL,
	[PositionDesc] [varchar](80) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[OrganizationID] [varchar](12) NOT NULL,
	[OrganizationAltID] [varchar](12) NOT NULL,
	[OrganizationDesc] [varchar](80) NOT NULL,
	[OrgSectionID] [varchar](12) NOT NULL,
	[OrgSectionAltID] [varchar](50) NOT NULL,
	[OrgSectionDesc] [varchar](80) NOT NULL,
	[OrgDepartmentID] [varchar](12) NOT NULL,
	[OrgDepartmentAltID] [varchar](12) NOT NULL,
	[OrgDepartmentDesc] [varchar](80) NOT NULL,
	[OrgDivisionID] [varchar](12) NOT NULL,
	[OrgDivisionAltID] [varchar](12) NOT NULL,
	[OrgDivisionDesc] [varchar](80) NOT NULL,
	[OrgBusinessUnitID] [varchar](12) NOT NULL,
	[OrgBusinessUnitAltID] [varchar](12) NOT NULL,
	[OrgBusinessUnitDesc] [varchar](80) NOT NULL,
	[OrgGCEOID] [varchar](12) NOT NULL,
	[OrgGCEOAltID] [varchar](12) NOT NULL,
	[OrgGCEODesc] [varchar](80) NOT NULL,
	[OrgChairmanID] [varchar](12) NOT NULL,
	[OrgChairmanAltID] [varchar](12) NOT NULL,
	[OrgChairmanDesc] [varchar](80) NOT NULL,
	[HierSectionID] [varchar](12) NOT NULL,
	[HierSectionAltID] [varchar](50) NOT NULL,
	[HierSectionDesc] [varchar](80) NOT NULL,
	[HierDepartmentID] [varchar](12) NOT NULL,
	[HierDepartmentAltID] [varchar](12) NOT NULL,
	[HierDepartmentDesc] [varchar](80) NOT NULL,
	[HierDivisionID] [varchar](12) NOT NULL,
	[HierDivisionAltID] [varchar](12) NOT NULL,
	[HierDivisionDesc] [varchar](80) NOT NULL,
	[HierBusinessUnitID] [varchar](12) NOT NULL,
	[HierBusinessUnitAltID] [varchar](12) NOT NULL,
	[HierBusinessUnitDesc] [varchar](80) NOT NULL,
	[HierGCEOID] [varchar](12) NOT NULL,
	[HierGCEOAltID] [varchar](12) NOT NULL,
	[HierGCEODesc] [varchar](80) NOT NULL,
	[HierChairmanID] [varchar](12) NOT NULL,
	[HierChairmanAltID] [varchar](12) NOT NULL,
	[HierChairmanDesc] [varchar](80) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_MPosition] PRIMARY KEY CLUSTERED 
(
	[PositionID] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MPreBuildRecipientTemplates]    Script Date: 11/21/2019 4:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MPreBuildRecipientTemplates](
	[PreBuildRecTemplateID] [varchar](20) NOT NULL,
	[PreBuildDesc] [varchar](50) NULL,
	[IsPIC] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[FunctionID] [varchar](20) NOT NULL,
 CONSTRAINT [PK__MPreBuil__4B9E41610B57D252] PRIMARY KEY CLUSTERED 
(
	[PreBuildRecTemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MPriceType]    Script Date: 11/21/2019 4:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MPriceType](
	[PriceTypeID] [varchar](3) NOT NULL,
	[PriceTypeDesc] [varchar](40) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MPriceType] PRIMARY KEY CLUSTERED 
(
	[PriceTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MProject]    Script Date: 11/21/2019 4:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MProject](
	[ProjectID] [varchar](32) NOT NULL,
	[ProjectDesc] [varchar](50) NOT NULL,
	[CompanyID] [varchar](32) NULL,
	[DivisionID] [varchar](32) NOT NULL,
	[LocationID] [varchar](3) NULL,
	[City] [varchar](25) NOT NULL,
	[Street] [varchar](255) NOT NULL,
	[Postal] [varchar](6) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MProject] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MRecipientTypes]    Script Date: 11/21/2019 4:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MRecipientTypes](
	[RecipientTypeID] [varchar](20) NOT NULL,
	[Descriptions] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MRecipie__AA7E6E7A993091B7] PRIMARY KEY CLUSTERED 
(
	[RecipientTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MRegion]    Script Date: 11/21/2019 4:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MRegion](
	[RegionID] [varchar](3) NOT NULL,
	[RegionDesc] [varchar](20) NOT NULL,
	[CountryID] [varchar](3) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MRegion] PRIMARY KEY CLUSTERED 
(
	[RegionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MRole]    Script Date: 11/21/2019 4:08:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MRole](
	[RoleID] [varchar](8) NOT NULL,
	[RoleDesc] [varchar](255) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MRole] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MSchedules]    Script Date: 11/21/2019 4:08:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MSchedules](
	[ScheduleID] [varchar](32) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Subject] [varchar](255) NULL,
	[Notes] [ntext] NULL,
	[Weblink] [nvarchar](255) NULL,
	[Location] [nvarchar](100) NULL,
	[Priority] [int] NULL,
	[ProjectID] [varchar](32) NULL,
	[ClusterID] [varchar](3) NULL,
	[IsAllDay] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[TaskID] [varchar](20) NULL,
	[StatusID] [varchar](20) NOT NULL,
	[FPTID] [varchar](50) NULL,
	[MailNotificationID] [varchar](32) NOT NULL,
	[IsBatchMail] [bit] NULL,
 CONSTRAINT [PK__MSchedul__9C8A5B69E7A1C68F] PRIMARY KEY CLUSTERED 
(
	[ScheduleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MStatus]    Script Date: 11/21/2019 4:08:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MStatus](
	[TableName] [varchar](80) NOT NULL,
	[StatusID] [int] NOT NULL,
	[StatusDesc] [varchar](80) NOT NULL,
	[Visible] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MStatus] PRIMARY KEY CLUSTERED 
(
	[TableName] ASC,
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MTaskGroup]    Script Date: 11/21/2019 4:08:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MTaskGroup](
	[TaskGroupID] [varchar](5) NOT NULL,
	[TaskGroupDesc] [varchar](70) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MTaskGroup] PRIMARY KEY CLUSTERED 
(
	[TaskGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MTasks]    Script Date: 11/21/2019 4:08:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MTasks](
	[TaskID] [varchar](20) NOT NULL,
	[TaskTypeID] [varchar](10) NOT NULL,
	[TaskDateTimeStamp] [datetime] NOT NULL,
	[TaskOwnerID] [varchar](20) NOT NULL,
	[StatusID] [int] NOT NULL,
	[CurrentApprovalLvl] [int] NULL,
	[Remarks] [varchar](max) NULL,
	[Summary] [varchar](max) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[TaskDesc] [varchar](100) NULL,
 CONSTRAINT [PK__MTasks__7C6949D164B7E415] PRIMARY KEY CLUSTERED 
(
	[TaskID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MTaskTypes]    Script Date: 11/21/2019 4:08:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MTaskTypes](
	[TaskTypeID] [varchar](10) NOT NULL,
	[Descriptions] [varchar](50) NOT NULL,
	[TaskGroupID] [varchar](5) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MTaskTyp__66B23E537E9B5001] PRIMARY KEY CLUSTERED 
(
	[TaskTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MTCTypes]    Script Date: 11/21/2019 4:08:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MTCTypes](
	[TCTypeID] [varchar](20) NOT NULL,
	[Descriptions] [varchar](10) NOT NULL,
	[TCTypeParentID] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__MTCTypes__7B271A5AC8BB5520] PRIMARY KEY CLUSTERED 
(
	[TCTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MUnitType]    Script Date: 11/21/2019 4:08:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MUnitType](
	[UnitTypeID] [varchar](5) NOT NULL,
	[UnitTypeDesc] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MUnitType] PRIMARY KEY CLUSTERED 
(
	[UnitTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MUoM]    Script Date: 11/21/2019 4:08:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MUoM](
	[UoMID] [varchar](3) NOT NULL,
	[UoMDesc] [varchar](25) NOT NULL,
	[DimensionID] [varchar](6) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MUoM] PRIMARY KEY CLUSTERED 
(
	[UoMID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MUser]    Script Date: 11/21/2019 4:08:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MUser](
	[UserID] [nvarchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NULL,
	[FullName] [nvarchar](50) NOT NULL,
	[BusinessUnitID] [varchar](32) NULL,
	[DivisionID] [varchar](32) NULL,
	[ProjectID] [varchar](32) NULL,
	[ClusterID] [varchar](3) NULL,
	[Password] [nvarchar](100) NOT NULL,
	[VendorID] [varchar](32) NULL,
	[LastLogin] [datetime] NOT NULL,
	[HostIP] [nvarchar](40) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MUser] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MVendor]    Script Date: 11/21/2019 4:08:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MVendor](
	[VendorID] [varchar](32) NOT NULL,
	[FirstName] [varchar](35) NOT NULL,
	[LastName] [varchar](35) NOT NULL,
	[VendorSubcategoryID] [varchar](4) NOT NULL,
	[City] [varchar](25) NOT NULL,
	[Street] [varchar](255) NOT NULL,
	[Postal] [varchar](6) NOT NULL,
	[Phone] [varchar](20) NOT NULL,
	[Email] [varchar](128) NOT NULL,
	[IDNo] [varchar](20) NOT NULL,
	[NPWP] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MVendor] PRIMARY KEY CLUSTERED 
(
	[VendorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MVendor_]    Script Date: 11/21/2019 4:08:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MVendor_](
	[VendorID] [varchar](32) NOT NULL,
	[FirstName] [varchar](35) NOT NULL,
	[LastName] [varchar](35) NOT NULL,
	[VendorSubcategoryID] [varchar](4) NOT NULL,
	[City] [varchar](25) NOT NULL,
	[Street] [varchar](255) NOT NULL,
	[Postal] [varchar](6) NOT NULL,
	[Phone] [varchar](20) NOT NULL,
	[Email] [varchar](128) NOT NULL,
	[IDNo] [varchar](20) NOT NULL,
	[NPWP] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MVendorCategory]    Script Date: 11/21/2019 4:08:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MVendorCategory](
	[VendorCategoryID] [varchar](4) NOT NULL,
	[VendorCategoryDesc] [varchar](40) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MVendorCategory] PRIMARY KEY CLUSTERED 
(
	[VendorCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MVendorDocumentTypes]    Script Date: 11/21/2019 4:08:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MVendorDocumentTypes](
	[VendorDocumentTypeID] [varchar](32) NOT NULL,
	[DocumentTypeDesc] [varchar](30) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[VendorDocumentTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MVendorSubcategory]    Script Date: 11/21/2019 4:08:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MVendorSubcategory](
	[VendorSubcategoryID] [varchar](4) NOT NULL,
	[VendorSubcategoryDesc] [varchar](40) NOT NULL,
	[VendorCategoryID] [varchar](4) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MVendorSubcategory] PRIMARY KEY CLUSTERED 
(
	[VendorSubcategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MWBS]    Script Date: 11/21/2019 4:08:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MWBS](
	[WBSID] [varchar](100) NOT NULL,
	[WBSDesc] [varchar](100) NOT NULL,
	[ProjectID] [varchar](32) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_MWBS] PRIMARY KEY CLUSTERED 
(
	[WBSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MWorkContract]    Script Date: 11/21/2019 4:08:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MWorkContract](
	[WorkContractID] [varchar](2) NOT NULL,
	[WorkContractDesc] [nvarchar](15) NOT NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHostName] [nvarchar](15) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHostName] [nvarchar](15) NULL,
 CONSTRAINT [PK_MWorkContract] PRIMARY KEY CLUSTERED 
(
	[WorkContractID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SAction]    Script Date: 11/21/2019 4:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SAction](
	[ActionID] [varchar](30) NOT NULL,
	[ActionDesc] [varchar](30) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_SAction] PRIMARY KEY CLUSTERED 
(
	[ActionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SBudgetPlanPeriod]    Script Date: 11/21/2019 4:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SBudgetPlanPeriod](
	[BudgetPlanPeriodID] [varchar](2) NOT NULL,
	[BudgetPlanPeriodDesc] [varchar](40) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_SBudgetPlanPeriod] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanPeriodID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SDimension]    Script Date: 11/21/2019 4:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SDimension](
	[DimensionID] [varchar](6) NOT NULL,
	[DimensionDesc] [varchar](40) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_SDimension] PRIMARY KEY CLUSTERED 
(
	[DimensionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMenu]    Script Date: 11/21/2019 4:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMenu](
	[MenuID] [varchar](50) NOT NULL,
	[MenuHierarchy] [varchar](20) NOT NULL,
	[MenuDesc] [varchar](100) NOT NULL,
	[MenuIcon] [varchar](100) NOT NULL,
	[MenuUrl] [varchar](100) NOT NULL,
	[MenuVisible] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_SMenu] PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SNegotiationConfigTypes]    Script Date: 11/21/2019 4:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SNegotiationConfigTypes](
	[NegotiationConfigTypeID] [varchar](20) NOT NULL,
	[Descriptions] [varchar](20) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__SNegotia__47570C5DEE5903B2] PRIMARY KEY CLUSTERED 
(
	[NegotiationConfigTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SReport]    Script Date: 11/21/2019 4:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SReport](
	[ReportID] [varchar](20) NOT NULL,
	[ReportDesc] [varchar](50) NOT NULL,
	[ReportViewName] [varchar](100) NOT NULL,
	[ReportVisible] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_SReport] PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TBudgetPlan]    Script Date: 11/21/2019 4:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TBudgetPlan](
	[BudgetPlanID] [varchar](20) NOT NULL,
	[BudgetPlanTemplateID] [varchar](3) NOT NULL,
	[ProjectID] [varchar](32) NOT NULL,
	[ClusterID] [varchar](3) NULL,
	[UnitTypeID] [varchar](5) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_TBudgetPlan] PRIMARY KEY CLUSTERED 
(
	[BudgetPlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TCatalogCart]    Script Date: 11/21/2019 4:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TCatalogCart](
	[CatalogCartID] [varchar](32) NOT NULL,
	[CatalogCartDesc] [varchar](100) NULL,
	[UserID] [nvarchar](20) NULL,
	[StatusID] [int] NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [nvarchar](20) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [nvarchar](20) NULL,
 CONSTRAINT [PK__TCatalog__776D33958212CC41] PRIMARY KEY CLUSTERED 
(
	[CatalogCartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TEMPMUser]    Script Date: 11/21/2019 4:08:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TEMPMUser](
	[UserID] [nvarchar](20) NOT NULL,
	[RoleID] [varchar](8) NOT NULL,
	[FullName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[VendorID] [varchar](10) NULL,
	[LastLogin] [datetime] NOT NULL,
	[HostIP] [nvarchar](40) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TEMPSMenu]    Script Date: 11/21/2019 4:08:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TEMPSMenu](
	[MenuID] [varchar](50) NOT NULL,
	[MenuHierarchy] [varchar](20) NOT NULL,
	[MenuDesc] [varchar](100) NOT NULL,
	[MenuIcon] [varchar](100) NOT NULL,
	[MenuUrl] [varchar](100) NOT NULL,
	[MenuVisible] [bit] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_TEMPSMenu] PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TEntryValues]    Script Date: 11/21/2019 4:08:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TEntryValues](
	[EntryValueID] [varchar](32) NOT NULL,
	[Value] [varchar](max) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[MinuteEntryID] [varchar](32) NULL,
	[FieldTagID] [varchar](20) NULL,
 CONSTRAINT [PK__TEntryVa__241B2F1ADD48070F] PRIMARY KEY CLUSTERED 
(
	[EntryValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TEventLog]    Script Date: 11/21/2019 4:08:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TEventLog](
	[LogID] [varchar](32) NOT NULL,
	[MenuID] [varchar](50) NOT NULL,
	[ActionID] [varchar](30) NOT NULL,
	[DataID] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_TEventLog] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TFPTAttendances]    Script Date: 11/21/2019 4:08:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TFPTAttendances](
	[FPTAttendanceID] [varchar](32) NOT NULL,
	[FPTID] [varchar](50) NOT NULL,
	[AttendeeType] [nvarchar](50) NULL,
	[IDAttendee] [varchar](20) NULL,
	[IsAttend] [bit] NULL,
	[AttendanceDesc] [nvarchar](200) NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [nvarchar](20) NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [nvarchar](20) NULL,
 CONSTRAINT [PK__TFPTAtte__E9F9753BEDE7416F] PRIMARY KEY CLUSTERED 
(
	[FPTAttendanceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TMailHistories]    Script Date: 11/21/2019 4:08:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TMailHistories](
	[MailHistoryID] [varchar](32) NOT NULL,
	[StatusDate] [datetime] NULL,
	[To] [varchar](max) NULL,
	[CC] [varchar](max) NULL,
	[BCC] [varchar](max) NULL,
	[Subject] [varchar](255) NULL,
	[Content] [ntext] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[MailNotificationID] [varchar](32) NOT NULL,
	[StatusID] [varchar](20) NOT NULL,
 CONSTRAINT [PK__TMailHis__15A39A7C4401D42C] PRIMARY KEY CLUSTERED 
(
	[MailHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TMinuteEntries]    Script Date: 11/21/2019 4:08:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TMinuteEntries](
	[MinuteEntryID] [varchar](32) NOT NULL,
	[Subject] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[FPTID] [varchar](50) NULL,
	[MinuteTemplateID] [varchar](20) NOT NULL,
	[StatusID] [int] NOT NULL,
	[TaskID] [varchar](20) NOT NULL,
	[MailNotificationID] [varchar](32) NULL,
	[ScheduleID] [varchar](32) NULL,
 CONSTRAINT [PK__TMinuteE__53F5F7E8E135874F] PRIMARY KEY CLUSTERED 
(
	[MinuteEntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TNegotiationBidStructures]    Script Date: 11/21/2019 4:08:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TNegotiationBidStructures](
	[NegotiationBidID] [varchar](32) NOT NULL,
	[NegotiationConfigID] [varchar](20) NOT NULL,
	[Sequence] [int] NULL,
	[ItemID] [varchar](20) NULL,
	[ItemDesc] [varchar](255) NULL,
	[ItemParentID] [varchar](20) NULL,
	[Version] [int] NULL,
	[ParentVersion] [int] NULL,
	[ParentSequence] [int] NULL,
	[BudgetPlanDefaultValue] [decimal](18, 2) NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__TNegotia__3564CF780222E99C] PRIMARY KEY CLUSTERED 
(
	[NegotiationBidID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TNotificationAttachments]    Script Date: 11/21/2019 4:08:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TNotificationAttachments](
	[AttachmentValueID] [varchar](20) NOT NULL,
	[Filename] [varchar](50) NOT NULL,
	[ContentType] [varchar](50) NOT NULL,
	[RawData] [ntext] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[MailNotificationID] [varchar](32) NOT NULL,
 CONSTRAINT [PK__TNotific__3F76F93F19BEC282] PRIMARY KEY CLUSTERED 
(
	[AttachmentValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TNotificationValues]    Script Date: 11/21/2019 4:08:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TNotificationValues](
	[NotificationValueID] [varchar](32) NOT NULL,
	[Value] [ntext] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
	[MailNotificationID] [varchar](32) NOT NULL,
	[FieldTagID] [varchar](20) NOT NULL,
 CONSTRAINT [PK__TNotific__2E86788D29C3A9A8] PRIMARY KEY CLUSTERED 
(
	[NotificationValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TNumbering]    Script Date: 11/21/2019 4:08:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TNumbering](
	[Header] [varchar](3) NOT NULL,
	[Year] [varchar](4) NOT NULL,
	[Month] [varchar](2) NOT NULL,
	[CompanyID] [varchar](32) NOT NULL,
	[ProjectID] [varchar](32) NOT NULL,
	[LastNo] [int] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_TNumbering] PRIMARY KEY CLUSTERED 
(
	[Header] ASC,
	[Year] ASC,
	[Month] ASC,
	[CompanyID] ASC,
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TPackage]    Script Date: 11/21/2019 4:08:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TPackage](
	[PackageID] [varchar](20) NOT NULL,
	[PackageDesc] [varchar](40) NOT NULL,
	[StatusID] [int] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedHost] [varchar](50) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime2](7) NULL,
	[ModifiedHost] [varchar](50) NULL,
 CONSTRAINT [PK_TPackage] PRIMARY KEY CLUSTERED 
(
	[PackageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TTCAppliedTypes]    Script Date: 11/21/2019 4:08:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TTCAppliedTypes](
	[TCAppliedID] [varchar](20) NOT NULL,
	[TCMemberID] [varchar](20) NOT NULL,
	[TCTypeID] [varchar](20) NOT NULL,
	[TCDivisionID] [varchar](20) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__TTCAppli__AD890332E86E2C90] PRIMARY KEY CLUSTERED 
(
	[TCAppliedID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TTCMemberDelegations]    Script Date: 11/21/2019 4:08:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TTCMemberDelegations](
	[TCDelegationID] [varchar](32) NOT NULL,
	[TCMemberID] [varchar](20) NOT NULL,
	[DelegateTo] [varchar](20) NOT NULL,
	[DelegateStartDate] [date] NOT NULL,
	[DelegateEndDate] [date] NOT NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [date] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedHost] [varchar](20) NULL,
	[ModifiedDate] [date] NULL,
 CONSTRAINT [PK__TTCMembe__6ACAF7A5F54EBFC4] PRIMARY KEY CLUSTERED 
(
	[TCDelegationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TTCMembers]    Script Date: 11/21/2019 4:08:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TTCMembers](
	[TCMemberID] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[SuperiorID] [varchar](20) NULL,
	[TCTypeID] [varchar](20) NOT NULL,
	[BusinessUnitID] [varchar](32) NOT NULL,
	[PeriodStart] [date] NULL,
	[PeriodEnd] [date] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedHost] [varchar](20) NULL,
	[ModifiedBy] [varchar](20) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedHost] [varchar](20) NULL,
 CONSTRAINT [PK__TTCMembe__388FCD5530441BD6] PRIMARY KEY CLUSTERED 
(
	[TCMemberID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UConfig]    Script Date: 11/21/2019 4:08:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UConfig](
	[Key1] [nvarchar](50) NOT NULL,
	[Key2] [nvarchar](50) NOT NULL,
	[Key3] [nvarchar](50) NOT NULL,
	[Key4] [nvarchar](50) NOT NULL,
	[Desc1] [nvarchar](1000) NOT NULL,
	[Desc2] [nvarchar](1000) NULL,
	[Desc3] [nvarchar](1000) NULL,
	[Desc4] [nvarchar](1000) NULL,
 CONSTRAINT [PK_UConfig] PRIMARY KEY CLUSTERED 
(
	[Key1] ASC,
	[Key2] ASC,
	[Key3] ASC,
	[Key4] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DBudgetPlanVersion] ADD  CONSTRAINT [DF_DBudgetPlanVersion_IsBidOpen]  DEFAULT ((0)) FOR [IsBidOpen]
GO
ALTER TABLE [dbo].[DFPTNegotiationRounds] ADD  CONSTRAINT [DF_DFPTNegotiationRounds_TopValue]  DEFAULT ((0)) FOR [TopValue]
GO
ALTER TABLE [dbo].[DFPTNegotiationRounds] ADD  CONSTRAINT [DF_DFPTNegotiationRounds_BottomValue]  DEFAULT ((0)) FOR [BottomValue]
GO
ALTER TABLE [dbo].[MFPT] ADD  CONSTRAINT [DF_MFPT_IsSync]  DEFAULT ((0)) FOR [IsSync]
GO
ALTER TABLE [dbo].[TNegotiationBidStructures] ADD  CONSTRAINT [DF__TNegotiat__Budge__5086CE36]  DEFAULT ((0)) FOR [BudgetPlanDefaultValue]
GO
ALTER TABLE [dbo].[CApprovalPath]  WITH CHECK ADD  CONSTRAINT [MTaskTypes_CApprovalPath_FK1] FOREIGN KEY([TaskTypeID])
REFERENCES [dbo].[MTaskTypes] ([TaskTypeID])
GO
ALTER TABLE [dbo].[CApprovalPath] CHECK CONSTRAINT [MTaskTypes_CApprovalPath_FK1]
GO
ALTER TABLE [dbo].[CMenuAction]  WITH CHECK ADD  CONSTRAINT [FK_CMenuAction_SAction] FOREIGN KEY([ActionID])
REFERENCES [dbo].[SAction] ([ActionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[CMenuAction] CHECK CONSTRAINT [FK_CMenuAction_SAction]
GO
ALTER TABLE [dbo].[CMenuObject]  WITH CHECK ADD  CONSTRAINT [FK_CMenuObject_SMenu] FOREIGN KEY([MenuID])
REFERENCES [dbo].[SMenu] ([MenuID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CMenuObject] CHECK CONSTRAINT [FK_CMenuObject_SMenu]
GO
ALTER TABLE [dbo].[CNegotiationConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_CNegotiationConfigurations_MFPT] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[CNegotiationConfigurations] CHECK CONSTRAINT [FK_CNegotiationConfigurations_MFPT]
GO
ALTER TABLE [dbo].[CNegotiationConfigurations]  WITH CHECK ADD  CONSTRAINT [MTasks_CNegotiationConfigurations_FK2] FOREIGN KEY([TaskID])
REFERENCES [dbo].[MTasks] ([TaskID])
GO
ALTER TABLE [dbo].[CNegotiationConfigurations] CHECK CONSTRAINT [MTasks_CNegotiationConfigurations_FK2]
GO
ALTER TABLE [dbo].[CNegotiationConfigurations]  WITH CHECK ADD  CONSTRAINT [SNegotiationConfigTypes_CNegotiationConfigurations_FK1] FOREIGN KEY([NegotiationConfigTypeID])
REFERENCES [dbo].[SNegotiationConfigTypes] ([NegotiationConfigTypeID])
GO
ALTER TABLE [dbo].[CNegotiationConfigurations] CHECK CONSTRAINT [SNegotiationConfigTypes_CNegotiationConfigurations_FK1]
GO
ALTER TABLE [dbo].[DApprovalDelegationUser]  WITH CHECK ADD  CONSTRAINT [FK_DApprovalDelegationUser_DApprovalDelegationUser] FOREIGN KEY([ApprovalDelegateID])
REFERENCES [dbo].[MApprovalDelegation] ([ApprovalDelegateID])
GO
ALTER TABLE [dbo].[DApprovalDelegationUser] CHECK CONSTRAINT [FK_DApprovalDelegationUser_DApprovalDelegationUser]
GO
ALTER TABLE [dbo].[DApprovalDelegationUser]  WITH CHECK ADD  CONSTRAINT [FK_DApprovalDelegationUser_MUser] FOREIGN KEY([DelegateUserID])
REFERENCES [dbo].[MUser] ([UserID])
GO
ALTER TABLE [dbo].[DApprovalDelegationUser] CHECK CONSTRAINT [FK_DApprovalDelegationUser_MUser]
GO
ALTER TABLE [dbo].[DBudgetPlanTCBidOpening]  WITH CHECK ADD  CONSTRAINT [DBudgetPlanbidOpening_DBudgetPlanTCBidOpening_FK2] FOREIGN KEY([BPBidOpeningID])
REFERENCES [dbo].[DBudgetPlanBidOpening] ([BPBidOpeningID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanTCBidOpening] CHECK CONSTRAINT [DBudgetPlanbidOpening_DBudgetPlanTCBidOpening_FK2]
GO
ALTER TABLE [dbo].[DBudgetPlanTCBidOpening]  WITH CHECK ADD  CONSTRAINT [TTCMembers_DBudgetPlanTCBidOpening_FK1] FOREIGN KEY([TCMemberID])
REFERENCES [dbo].[TTCMembers] ([TCMemberID])
GO
ALTER TABLE [dbo].[DBudgetPlanTCBidOpening] CHECK CONSTRAINT [TTCMembers_DBudgetPlanTCBidOpening_FK1]
GO
ALTER TABLE [dbo].[DBudgetPlanTemplateStructure]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanTemplateStructure_DItemVersion] FOREIGN KEY([ItemID], [Version])
REFERENCES [dbo].[DItemVersion] ([ItemID], [Version])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanTemplateStructure] CHECK CONSTRAINT [FK_DBudgetPlanTemplateStructure_DItemVersion]
GO
ALTER TABLE [dbo].[DBudgetPlanTemplateStructure]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanTemplateStructure_MBudgetPlanTemplate] FOREIGN KEY([BudgetPlanTemplateID])
REFERENCES [dbo].[MBudgetPlanTemplate] ([BudgetPlanTemplateID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanTemplateStructure] CHECK CONSTRAINT [FK_DBudgetPlanTemplateStructure_MBudgetPlanTemplate]
GO
ALTER TABLE [dbo].[DBudgetPlanVersion]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersion_TBudgetPlan] FOREIGN KEY([BudgetPlanID])
REFERENCES [dbo].[TBudgetPlan] ([BudgetPlanID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersion] CHECK CONSTRAINT [FK_DBudgetPlanVersion_TBudgetPlan]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionAdditional]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionAdditional_DBudgetPlanVersionVendor] FOREIGN KEY([BudgetPlanVersionVendorID])
REFERENCES [dbo].[DBudgetPlanVersionVendor] ([BudgetPlanVersionVendorID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionAdditional] CHECK CONSTRAINT [FK_DBudgetPlanVersionAdditional_DBudgetPlanVersionVendor]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionAdditional]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionAdditional_DItemVersion] FOREIGN KEY([ItemID], [Version])
REFERENCES [dbo].[DItemVersion] ([ItemID], [Version])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionAdditional] CHECK CONSTRAINT [FK_DBudgetPlanVersionAdditional_DItemVersion]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionAssignment]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionAssignment_DBudgetPlanVersionStructure] FOREIGN KEY([BudgetPlanVersionStructureID])
REFERENCES [dbo].[DBudgetPlanVersionStructure] ([BudgetPlanVersionStructureID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionAssignment] CHECK CONSTRAINT [FK_DBudgetPlanVersionAssignment_DBudgetPlanVersionStructure]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionAssignment]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionAssignment_MVendor] FOREIGN KEY([VendorID])
REFERENCES [dbo].[MVendor] ([VendorID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionAssignment] CHECK CONSTRAINT [FK_DBudgetPlanVersionAssignment_MVendor]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionEntry]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionEntry_DBudgetPlanVersionVendor] FOREIGN KEY([BudgetPlanVersionVendorID])
REFERENCES [dbo].[DBudgetPlanVersionVendor] ([BudgetPlanVersionVendorID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionEntry] CHECK CONSTRAINT [FK_DBudgetPlanVersionEntry_DBudgetPlanVersionVendor]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionMutual]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionMutual_DBudgetPlanVersionStructure] FOREIGN KEY([BudgetPlanVersionStructureID])
REFERENCES [dbo].[DBudgetPlanVersionStructure] ([BudgetPlanVersionStructureID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionMutual] CHECK CONSTRAINT [FK_DBudgetPlanVersionMutual_DBudgetPlanVersionStructure]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionPeriod]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionPeriod_DBudgetPlanVersion] FOREIGN KEY([BudgetPlanID], [BudgetPlanVersion])
REFERENCES [dbo].[DBudgetPlanVersion] ([BudgetPlanID], [BudgetPlanVersion])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionPeriod] CHECK CONSTRAINT [FK_DBudgetPlanVersionPeriod_DBudgetPlanVersion]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionPeriod]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionPeriod_SBudgetPlanPeriod] FOREIGN KEY([BudgetPlanPeriodID])
REFERENCES [dbo].[SBudgetPlanPeriod] ([BudgetPlanPeriodID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionPeriod] CHECK CONSTRAINT [FK_DBudgetPlanVersionPeriod_SBudgetPlanPeriod]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionVendor]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionVendor_DBudgetPlanVersionPeriod] FOREIGN KEY([BudgetPlanVersionPeriodID])
REFERENCES [dbo].[DBudgetPlanVersionPeriod] ([BudgetPlanVersionPeriodID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionVendor] CHECK CONSTRAINT [FK_DBudgetPlanVersionVendor_DBudgetPlanVersionPeriod]
GO
ALTER TABLE [dbo].[DBudgetPlanVersionVendor]  WITH CHECK ADD  CONSTRAINT [FK_DBudgetPlanVersionVendor_MVendor] FOREIGN KEY([VendorID])
REFERENCES [dbo].[MVendor] ([VendorID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DBudgetPlanVersionVendor] CHECK CONSTRAINT [FK_DBudgetPlanVersionVendor_MVendor]
GO
ALTER TABLE [dbo].[DCatalogCartItems]  WITH CHECK ADD  CONSTRAINT [FK_DCatalogCartItems_TCatalogCart] FOREIGN KEY([CatalogCartID])
REFERENCES [dbo].[TCatalogCart] ([CatalogCartID])
GO
ALTER TABLE [dbo].[DCatalogCartItems] CHECK CONSTRAINT [FK_DCatalogCartItems_TCatalogCart]
GO
ALTER TABLE [dbo].[DEmpCommunication]  WITH NOCHECK ADD  CONSTRAINT [FK_DEmpCommunication_MEmployee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MEmployee] ([EmployeeID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[DEmpCommunication] NOCHECK CONSTRAINT [FK_DEmpCommunication_MEmployee]
GO
ALTER TABLE [dbo].[DEmpOrgAss]  WITH NOCHECK ADD  CONSTRAINT [FK_DEmpOrgAss_MEmployee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MEmployee] ([EmployeeID])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[DEmpOrgAss] NOCHECK CONSTRAINT [FK_DEmpOrgAss_MEmployee]
GO
ALTER TABLE [dbo].[DEmpOrgAss]  WITH NOCHECK ADD  CONSTRAINT [FK_DEmpOrgAss_MEmployee1] FOREIGN KEY([SupervisorID])
REFERENCES [dbo].[MEmployee] ([EmployeeID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[DEmpOrgAss] NOCHECK CONSTRAINT [FK_DEmpOrgAss_MEmployee1]
GO
ALTER TABLE [dbo].[DEmpOrgAss]  WITH NOCHECK ADD  CONSTRAINT [FK_DEmpOrgAss_MEmployeeGroup] FOREIGN KEY([EmployeeGroupID])
REFERENCES [dbo].[MEmployeeGroup] ([EmployeeGroupID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[DEmpOrgAss] NOCHECK CONSTRAINT [FK_DEmpOrgAss_MEmployeeGroup]
GO
ALTER TABLE [dbo].[DEmpOrgAss]  WITH NOCHECK ADD  CONSTRAINT [FK_DEmpOrgAss_MEmployeeSubgroup] FOREIGN KEY([EmployeeSubgroupID])
REFERENCES [dbo].[MEmployeeSubgroup] ([EmployeeSubgroupID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[DEmpOrgAss] NOCHECK CONSTRAINT [FK_DEmpOrgAss_MEmployeeSubgroup]
GO
ALTER TABLE [dbo].[DEmpOrgAss]  WITH NOCHECK ADD  CONSTRAINT [FK_DEmpOrgAss_MPersonnelSubarea] FOREIGN KEY([PersonnelAreaID], [PersonnelSubareaID])
REFERENCES [dbo].[MPersonnelSubarea] ([PersonnelAreaID], [PersonnelSubareaID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[DEmpOrgAss] NOCHECK CONSTRAINT [FK_DEmpOrgAss_MPersonnelSubarea]
GO
ALTER TABLE [dbo].[DEmpOrgAss]  WITH NOCHECK ADD  CONSTRAINT [FK_DEmpOrgAss_MWorkContract] FOREIGN KEY([WorkContractID])
REFERENCES [dbo].[MWorkContract] ([WorkContractID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[DEmpOrgAss] NOCHECK CONSTRAINT [FK_DEmpOrgAss_MWorkContract]
GO
ALTER TABLE [dbo].[DEventPIC]  WITH CHECK ADD  CONSTRAINT [MFPT_MEventPIC_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DEventPIC] CHECK CONSTRAINT [MFPT_MEventPIC_FK1]
GO
ALTER TABLE [dbo].[DEventPIC]  WITH CHECK ADD  CONSTRAINT [MFunctions_MEventPIC_FK3] FOREIGN KEY([FunctionID])
REFERENCES [dbo].[MFunctions] ([FunctionID])
GO
ALTER TABLE [dbo].[DEventPIC] CHECK CONSTRAINT [MFunctions_MEventPIC_FK3]
GO
ALTER TABLE [dbo].[DEventPIC]  WITH CHECK ADD  CONSTRAINT [MPICTypes_MEventPIC_FK2] FOREIGN KEY([PICTypeID])
REFERENCES [dbo].[MPICTypes] ([PICTypeID])
GO
ALTER TABLE [dbo].[DEventPIC] CHECK CONSTRAINT [MPICTypes_MEventPIC_FK2]
GO
ALTER TABLE [dbo].[DFPTAdditionalInfo]  WITH CHECK ADD  CONSTRAINT [FK_DFPTAdditionalInfo_MAdditionalInfoItems] FOREIGN KEY([FPTAdditionalInfoItemID])
REFERENCES [dbo].[MAdditionalInfoItems] ([FPTAdditionalInfoItemID])
GO
ALTER TABLE [dbo].[DFPTAdditionalInfo] CHECK CONSTRAINT [FK_DFPTAdditionalInfo_MAdditionalInfoItems]
GO
ALTER TABLE [dbo].[DFPTAdditionalInfo]  WITH CHECK ADD  CONSTRAINT [FK_DFPTAdditionalInfo_MFPT] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTAdditionalInfo] CHECK CONSTRAINT [FK_DFPTAdditionalInfo_MFPT]
GO
ALTER TABLE [dbo].[DFPTDeviations]  WITH CHECK ADD  CONSTRAINT [MDeviationTypes_DFPTDeviations_FK2] FOREIGN KEY([DeviationTypeID])
REFERENCES [dbo].[MDeviationTypes] ([DeviationTypeID])
GO
ALTER TABLE [dbo].[DFPTDeviations] CHECK CONSTRAINT [MDeviationTypes_DFPTDeviations_FK2]
GO
ALTER TABLE [dbo].[DFPTDeviations]  WITH CHECK ADD  CONSTRAINT [MFPT_DFPTDeviations_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTDeviations] CHECK CONSTRAINT [MFPT_DFPTDeviations_FK1]
GO
ALTER TABLE [dbo].[DFPTHistories]  WITH CHECK ADD  CONSTRAINT [MFPT_DFPTHistories_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTHistories] CHECK CONSTRAINT [MFPT_DFPTHistories_FK1]
GO
ALTER TABLE [dbo].[DFPTNegotiationRounds]  WITH CHECK ADD  CONSTRAINT [MFPT_DFPTNegotiationRounds_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTNegotiationRounds] CHECK CONSTRAINT [MFPT_DFPTNegotiationRounds_FK1]
GO
ALTER TABLE [dbo].[DFPTProjects]  WITH CHECK ADD  CONSTRAINT [MFPT_DFPTProjects_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTProjects] CHECK CONSTRAINT [MFPT_DFPTProjects_FK1]
GO
ALTER TABLE [dbo].[DFPTStatus]  WITH CHECK ADD  CONSTRAINT [DFPTSTatus_MFPT_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTStatus] CHECK CONSTRAINT [DFPTSTatus_MFPT_FK1]
GO
ALTER TABLE [dbo].[DFPTTCParticipants]  WITH CHECK ADD  CONSTRAINT [FK_DFPTTCParticipants_MFPT] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTTCParticipants] CHECK CONSTRAINT [FK_DFPTTCParticipants_MFPT]
GO
ALTER TABLE [dbo].[DFPTVendorParticipants]  WITH CHECK ADD  CONSTRAINT [FK_DFPTVendorParticipants_CNegotiationConfigurations] FOREIGN KEY([NegotiationConfigID])
REFERENCES [dbo].[CNegotiationConfigurations] ([NegotiationConfigID])
GO
ALTER TABLE [dbo].[DFPTVendorParticipants] CHECK CONSTRAINT [FK_DFPTVendorParticipants_CNegotiationConfigurations]
GO
ALTER TABLE [dbo].[DFPTVendorParticipants]  WITH CHECK ADD  CONSTRAINT [MFPT_DFPTVendorParticipants_FK1] FOREIGN KEY([VendorID])
REFERENCES [dbo].[MVendor] ([VendorID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DFPTVendorParticipants] CHECK CONSTRAINT [MFPT_DFPTVendorParticipants_FK1]
GO
ALTER TABLE [dbo].[DFPTVendorRecommendations]  WITH CHECK ADD  CONSTRAINT [DFPTVendorParticipants_DFPTVendorRecommendations_FK4] FOREIGN KEY([FPTVendorParticipantID])
REFERENCES [dbo].[DFPTVendorParticipants] ([FPTVendorParticipantID])
GO
ALTER TABLE [dbo].[DFPTVendorRecommendations] CHECK CONSTRAINT [DFPTVendorParticipants_DFPTVendorRecommendations_FK4]
GO
ALTER TABLE [dbo].[DFPTVendorRecommendations]  WITH CHECK ADD  CONSTRAINT [MFPT_DFPTVendorRecommendations_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTVendorRecommendations] CHECK CONSTRAINT [MFPT_DFPTVendorRecommendations_FK1]
GO
ALTER TABLE [dbo].[DFPTVendorRecommendations]  WITH CHECK ADD  CONSTRAINT [MTasks_DFPTVendorRecommendations_FK3] FOREIGN KEY([TaskID])
REFERENCES [dbo].[MTasks] ([TaskID])
GO
ALTER TABLE [dbo].[DFPTVendorRecommendations] CHECK CONSTRAINT [MTasks_DFPTVendorRecommendations_FK3]
GO
ALTER TABLE [dbo].[DFPTVendorRecommendations]  WITH CHECK ADD  CONSTRAINT [TTCMembers_DFPTVendorRecommendations_FK2] FOREIGN KEY([TCMemberID])
REFERENCES [dbo].[TTCMembers] ([TCMemberID])
GO
ALTER TABLE [dbo].[DFPTVendorRecommendations] CHECK CONSTRAINT [TTCMembers_DFPTVendorRecommendations_FK2]
GO
ALTER TABLE [dbo].[DFPTVendorWinner]  WITH CHECK ADD  CONSTRAINT [DFPTVendorParticipant_DFPTVendorWinner_FK2] FOREIGN KEY([FPTVendorParticipantID])
REFERENCES [dbo].[DFPTVendorParticipants] ([FPTVendorParticipantID])
GO
ALTER TABLE [dbo].[DFPTVendorWinner] CHECK CONSTRAINT [DFPTVendorParticipant_DFPTVendorWinner_FK2]
GO
ALTER TABLE [dbo].[DFPTVendorWinner]  WITH CHECK ADD  CONSTRAINT [FK_DFPTVendorWinner_DNegotiationBidEntry] FOREIGN KEY([NegotiationEntryID])
REFERENCES [dbo].[DNegotiationBidEntry] ([NegotiationEntryID])
GO
ALTER TABLE [dbo].[DFPTVendorWinner] CHECK CONSTRAINT [FK_DFPTVendorWinner_DNegotiationBidEntry]
GO
ALTER TABLE [dbo].[DFPTVendorWinner]  WITH CHECK ADD  CONSTRAINT [MFPT_DFPTVendorWinner_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[DFPTVendorWinner] CHECK CONSTRAINT [MFPT_DFPTVendorWinner_FK1]
GO
ALTER TABLE [dbo].[DFPTVendorWinner]  WITH CHECK ADD  CONSTRAINT [MTasks_DFPTVendorWinner_FK3] FOREIGN KEY([TaskID])
REFERENCES [dbo].[MTasks] ([TaskID])
GO
ALTER TABLE [dbo].[DFPTVendorWinner] CHECK CONSTRAINT [MTasks_DFPTVendorWinner_FK3]
GO
ALTER TABLE [dbo].[DItemComparisonDetails]  WITH CHECK ADD  CONSTRAINT [FK_DItemComparisonDetails_MItemComparison] FOREIGN KEY([ItemComparisonID])
REFERENCES [dbo].[MItemComparison] ([ItemComparisonID])
GO
ALTER TABLE [dbo].[DItemComparisonDetails] CHECK CONSTRAINT [FK_DItemComparisonDetails_MItemComparison]
GO
ALTER TABLE [dbo].[DItemGroupParameter]  WITH CHECK ADD  CONSTRAINT [FK_DItemGroupParameter_MItemGroup] FOREIGN KEY([ItemGroupID])
REFERENCES [dbo].[MItemGroup] ([ItemGroupID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DItemGroupParameter] CHECK CONSTRAINT [FK_DItemGroupParameter_MItemGroup]
GO
ALTER TABLE [dbo].[DItemGroupParameter]  WITH CHECK ADD  CONSTRAINT [FK_DItemGroupParameter_MParameter] FOREIGN KEY([ParameterID])
REFERENCES [dbo].[MParameter] ([ParameterID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DItemGroupParameter] CHECK CONSTRAINT [FK_DItemGroupParameter_MParameter]
GO
ALTER TABLE [dbo].[DItemParameter]  WITH CHECK ADD  CONSTRAINT [FK_DItemParameter_MItem] FOREIGN KEY([ItemID])
REFERENCES [dbo].[MItem] ([ItemID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DItemParameter] CHECK CONSTRAINT [FK_DItemParameter_MItem]
GO
ALTER TABLE [dbo].[DItemPrice]  WITH CHECK ADD  CONSTRAINT [FK_DItemPrice_MItem] FOREIGN KEY([ItemID])
REFERENCES [dbo].[MItem] ([ItemID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DItemPrice] CHECK CONSTRAINT [FK_DItemPrice_MItem]
GO
ALTER TABLE [dbo].[DItemPrice]  WITH CHECK ADD  CONSTRAINT [FK_DItemPrice_MPriceType] FOREIGN KEY([PriceTypeID])
REFERENCES [dbo].[MPriceType] ([PriceTypeID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DItemPrice] CHECK CONSTRAINT [FK_DItemPrice_MPriceType]
GO
ALTER TABLE [dbo].[DItemPriceVendor]  WITH CHECK ADD  CONSTRAINT [FK_DItemPriceVendor_DItemPrice] FOREIGN KEY([ItemPriceID])
REFERENCES [dbo].[DItemPrice] ([ItemPriceID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DItemPriceVendor] CHECK CONSTRAINT [FK_DItemPriceVendor_DItemPrice]
GO
ALTER TABLE [dbo].[DItemPriceVendorPeriod]  WITH CHECK ADD  CONSTRAINT [FK_DItemPriceVendorPeriod_DItemPriceVendor] FOREIGN KEY([ItemPriceID], [VendorID])
REFERENCES [dbo].[DItemPriceVendor] ([ItemPriceID], [VendorID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DItemPriceVendorPeriod] CHECK CONSTRAINT [FK_DItemPriceVendorPeriod_DItemPriceVendor]
GO
ALTER TABLE [dbo].[DItemPriceVendorPeriod]  WITH CHECK ADD  CONSTRAINT [FK_DItemPriceVendorPeriod_MCurrency] FOREIGN KEY([CurrencyID])
REFERENCES [dbo].[MCurrency] ([CurrencyID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DItemPriceVendorPeriod] CHECK CONSTRAINT [FK_DItemPriceVendorPeriod_MCurrency]
GO
ALTER TABLE [dbo].[DItemVersion]  WITH CHECK ADD  CONSTRAINT [FK_DItemVersion_MItem] FOREIGN KEY([ItemID])
REFERENCES [dbo].[MItem] ([ItemID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DItemVersion] CHECK CONSTRAINT [FK_DItemVersion_MItem]
GO
ALTER TABLE [dbo].[DItemVersionChild]  WITH CHECK ADD  CONSTRAINT [FK_DItemVersionChild_DItemVersion] FOREIGN KEY([ItemID], [Version])
REFERENCES [dbo].[DItemVersion] ([ItemID], [Version])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DItemVersionChild] CHECK CONSTRAINT [FK_DItemVersionChild_DItemVersion]
GO
ALTER TABLE [dbo].[DItemVersionChildAlt]  WITH CHECK ADD  CONSTRAINT [FK_DItemVersionChildAlt_DItemVersionChild] FOREIGN KEY([ItemVersionChildID])
REFERENCES [dbo].[DItemVersionChild] ([ItemVersionChildID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DItemVersionChildAlt] CHECK CONSTRAINT [FK_DItemVersionChildAlt_DItemVersionChild]
GO
ALTER TABLE [dbo].[DItemVersionChildFormula]  WITH CHECK ADD  CONSTRAINT [FK_DItemVersionChildFormula_DItemVersionChild] FOREIGN KEY([ItemVersionChildID])
REFERENCES [dbo].[DItemVersionChild] ([ItemVersionChildID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DItemVersionChildFormula] CHECK CONSTRAINT [FK_DItemVersionChildFormula_DItemVersionChild]
GO
ALTER TABLE [dbo].[DNegotiationBidEntry]  WITH CHECK ADD  CONSTRAINT [DFPTVendorParticipants_DNegotiationBidEntry_FK4] FOREIGN KEY([FPTVendorParticipantID])
REFERENCES [dbo].[DFPTVendorParticipants] ([FPTVendorParticipantID])
GO
ALTER TABLE [dbo].[DNegotiationBidEntry] CHECK CONSTRAINT [DFPTVendorParticipants_DNegotiationBidEntry_FK4]
GO
ALTER TABLE [dbo].[DNegotiationBidEntry]  WITH CHECK ADD  CONSTRAINT [MBidTypes_DNegotiationBidEntry_FK1] FOREIGN KEY([BidTypeID])
REFERENCES [dbo].[MBidTypes] ([BidTypeID])
GO
ALTER TABLE [dbo].[DNegotiationBidEntry] CHECK CONSTRAINT [MBidTypes_DNegotiationBidEntry_FK1]
GO
ALTER TABLE [dbo].[DNegotiationBidEntry]  WITH CHECK ADD  CONSTRAINT [TNegotiationBidStructure_DNegotiationBidEntry_FK2] FOREIGN KEY([NegotiationBidID])
REFERENCES [dbo].[TNegotiationBidStructures] ([NegotiationBidID])
GO
ALTER TABLE [dbo].[DNegotiationBidEntry] CHECK CONSTRAINT [TNegotiationBidStructure_DNegotiationBidEntry_FK2]
GO
ALTER TABLE [dbo].[DNotificationMap]  WITH CHECK ADD  CONSTRAINT [MFunctions_DNotificationMap_FK1] FOREIGN KEY([FunctionID])
REFERENCES [dbo].[MFunctions] ([FunctionID])
GO
ALTER TABLE [dbo].[DNotificationMap] CHECK CONSTRAINT [MFunctions_DNotificationMap_FK1]
GO
ALTER TABLE [dbo].[DNotificationMap]  WITH CHECK ADD  CONSTRAINT [MNotificationTemplates_DNotificationMap_FK2] FOREIGN KEY([NotificationTemplateID])
REFERENCES [dbo].[MNotificationTemplates] ([NotificationTemplateID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DNotificationMap] CHECK CONSTRAINT [MNotificationTemplates_DNotificationMap_FK2]
GO
ALTER TABLE [dbo].[DPackageList]  WITH CHECK ADD  CONSTRAINT [FK_DPackageList_DBudgetPlanVersion] FOREIGN KEY([BudgetPlanID], [BudgetPlanVersion])
REFERENCES [dbo].[DBudgetPlanVersion] ([BudgetPlanID], [BudgetPlanVersion])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DPackageList] CHECK CONSTRAINT [FK_DPackageList_DBudgetPlanVersion]
GO
ALTER TABLE [dbo].[DPackageList]  WITH CHECK ADD  CONSTRAINT [FK_DPackageList_TPackage] FOREIGN KEY([PackageID])
REFERENCES [dbo].[TPackage] ([PackageID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DPackageList] CHECK CONSTRAINT [FK_DPackageList_TPackage]
GO
ALTER TABLE [dbo].[DPreBuildRecipients]  WITH CHECK ADD  CONSTRAINT [MEmployee_DPreBuildRecipients_FK2] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MEmployee] ([EmployeeID])
GO
ALTER TABLE [dbo].[DPreBuildRecipients] CHECK CONSTRAINT [MEmployee_DPreBuildRecipients_FK2]
GO
ALTER TABLE [dbo].[DPreBuildRecipients]  WITH CHECK ADD  CONSTRAINT [MPreBuildRecipientTemplateID_DPreBuildRecipients_FK1] FOREIGN KEY([PreBuildRecTemplateID])
REFERENCES [dbo].[MPreBuildRecipientTemplates] ([PreBuildRecTemplateID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DPreBuildRecipients] CHECK CONSTRAINT [MPreBuildRecipientTemplateID_DPreBuildRecipients_FK1]
GO
ALTER TABLE [dbo].[DPreBuildRecipients]  WITH CHECK ADD  CONSTRAINT [MRecipientTypes_DPreBuildRecipients_FK3] FOREIGN KEY([RecipientTypeID])
REFERENCES [dbo].[MRecipientTypes] ([RecipientTypeID])
GO
ALTER TABLE [dbo].[DPreBuildRecipients] CHECK CONSTRAINT [MRecipientTypes_DPreBuildRecipients_FK3]
GO
ALTER TABLE [dbo].[DRecipients]  WITH CHECK ADD  CONSTRAINT [MMailNotifications_DRecipients_FK2] FOREIGN KEY([MailNotificationID])
REFERENCES [dbo].[MMailNotifications] ([MailNotificationID])
GO
ALTER TABLE [dbo].[DRecipients] CHECK CONSTRAINT [MMailNotifications_DRecipients_FK2]
GO
ALTER TABLE [dbo].[DRecipients]  WITH CHECK ADD  CONSTRAINT [MRecipientTypes_DRecipients_FK1] FOREIGN KEY([RecipientTypeID])
REFERENCES [dbo].[MRecipientTypes] ([RecipientTypeID])
GO
ALTER TABLE [dbo].[DRecipients] CHECK CONSTRAINT [MRecipientTypes_DRecipients_FK1]
GO
ALTER TABLE [dbo].[DRoleFunction]  WITH CHECK ADD  CONSTRAINT [FK_DRoleFunction_DRoleFunction] FOREIGN KEY([FunctionID])
REFERENCES [dbo].[MFunctions] ([FunctionID])
GO
ALTER TABLE [dbo].[DRoleFunction] CHECK CONSTRAINT [FK_DRoleFunction_DRoleFunction]
GO
ALTER TABLE [dbo].[DRoleFunction]  WITH CHECK ADD  CONSTRAINT [FK_DRoleFunction_MRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[MRole] ([RoleID])
GO
ALTER TABLE [dbo].[DRoleFunction] CHECK CONSTRAINT [FK_DRoleFunction_MRole]
GO
ALTER TABLE [dbo].[DRoleMenuAction]  WITH CHECK ADD  CONSTRAINT [FK_DRoleMenuAction_CMenuAction] FOREIGN KEY([MenuID], [ActionID])
REFERENCES [dbo].[CMenuAction] ([MenuID], [ActionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DRoleMenuAction] CHECK CONSTRAINT [FK_DRoleMenuAction_CMenuAction]
GO
ALTER TABLE [dbo].[DRoleMenuAction]  WITH CHECK ADD  CONSTRAINT [FK_DRoleMenuAction_MRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[MRole] ([RoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DRoleMenuAction] CHECK CONSTRAINT [FK_DRoleMenuAction_MRole]
GO
ALTER TABLE [dbo].[DRoleMenuObject]  WITH CHECK ADD  CONSTRAINT [FK_DRoleMenuObject_CMenuObject] FOREIGN KEY([MenuID], [ObjectID])
REFERENCES [dbo].[CMenuObject] ([MenuID], [ObjectID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DRoleMenuObject] CHECK CONSTRAINT [FK_DRoleMenuObject_CMenuObject]
GO
ALTER TABLE [dbo].[DRoleMenuObject]  WITH CHECK ADD  CONSTRAINT [FK_DRoleMenuObject_MRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[MRole] ([RoleID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DRoleMenuObject] CHECK CONSTRAINT [FK_DRoleMenuObject_MRole]
GO
ALTER TABLE [dbo].[DTaskDetails]  WITH CHECK ADD  CONSTRAINT [MTasks_DTaskDetails_FK1] FOREIGN KEY([TaskID])
REFERENCES [dbo].[MTasks] ([TaskID])
GO
ALTER TABLE [dbo].[DTaskDetails] CHECK CONSTRAINT [MTasks_DTaskDetails_FK1]
GO
ALTER TABLE [dbo].[DTCFunctions]  WITH CHECK ADD  CONSTRAINT [MFunctions_DTCFunctions_FK2] FOREIGN KEY([FunctionID])
REFERENCES [dbo].[MFunctions] ([FunctionID])
GO
ALTER TABLE [dbo].[DTCFunctions] CHECK CONSTRAINT [MFunctions_DTCFunctions_FK2]
GO
ALTER TABLE [dbo].[DTCFunctions]  WITH CHECK ADD  CONSTRAINT [TTCMembers_DTCFunctions_FK1] FOREIGN KEY([TCMemberID])
REFERENCES [dbo].[TTCMembers] ([TCMemberID])
GO
ALTER TABLE [dbo].[DTCFunctions] CHECK CONSTRAINT [TTCMembers_DTCFunctions_FK1]
GO
ALTER TABLE [dbo].[DTemplateTags]  WITH CHECK ADD  CONSTRAINT [MFieldTagReferences_DNotificationTemplateTags_FK2] FOREIGN KEY([FieldTagID])
REFERENCES [dbo].[MFieldTagReferences] ([FieldTagID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DTemplateTags] CHECK CONSTRAINT [MFieldTagReferences_DNotificationTemplateTags_FK2]
GO
ALTER TABLE [dbo].[DUserRole]  WITH CHECK ADD  CONSTRAINT [FK_DUserRole_MRole] FOREIGN KEY([RoleID])
REFERENCES [dbo].[MRole] ([RoleID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[DUserRole] CHECK CONSTRAINT [FK_DUserRole_MRole]
GO
ALTER TABLE [dbo].[DUserRole]  WITH CHECK ADD  CONSTRAINT [FK_DUserRole_MUser] FOREIGN KEY([UserID])
REFERENCES [dbo].[MUser] ([UserID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DUserRole] CHECK CONSTRAINT [FK_DUserRole_MUser]
GO
ALTER TABLE [dbo].[DVendorCommunications]  WITH CHECK ADD  CONSTRAINT [DVendorCommunications_DVendorPICs] FOREIGN KEY([VendorPICID])
REFERENCES [dbo].[DVendorPICs] ([VendorPICID])
GO
ALTER TABLE [dbo].[DVendorCommunications] CHECK CONSTRAINT [DVendorCommunications_DVendorPICs]
GO
ALTER TABLE [dbo].[DVendorCommunications]  WITH CHECK ADD  CONSTRAINT [DVendorCommunications_MCommunicationTypes] FOREIGN KEY([CommunicationTypeID])
REFERENCES [dbo].[MCommunicationTypes] ([CommunicationTypeID])
GO
ALTER TABLE [dbo].[DVendorCommunications] CHECK CONSTRAINT [DVendorCommunications_MCommunicationTypes]
GO
ALTER TABLE [dbo].[DVendorPICs]  WITH CHECK ADD  CONSTRAINT [FK_DVendorPICs_MVendor] FOREIGN KEY([VendorID])
REFERENCES [dbo].[MVendor] ([VendorID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DVendorPICs] CHECK CONSTRAINT [FK_DVendorPICs_MVendor]
GO
ALTER TABLE [dbo].[MApprovalDelegation]  WITH CHECK ADD  CONSTRAINT [FK_MApprovalDelegation_MTaskTypes] FOREIGN KEY([TaskTypeID])
REFERENCES [dbo].[MTaskTypes] ([TaskTypeID])
GO
ALTER TABLE [dbo].[MApprovalDelegation] CHECK CONSTRAINT [FK_MApprovalDelegation_MTaskTypes]
GO
ALTER TABLE [dbo].[MApprovalDelegation]  WITH CHECK ADD  CONSTRAINT [FK_MApprovalDelegation_MUser] FOREIGN KEY([UserID])
REFERENCES [dbo].[MUser] ([UserID])
GO
ALTER TABLE [dbo].[MApprovalDelegation] CHECK CONSTRAINT [FK_MApprovalDelegation_MUser]
GO
ALTER TABLE [dbo].[MBudgetPlanTemplate]  WITH CHECK ADD  CONSTRAINT [FK_MBudgetPlanTemplate_MBudgetPlanType] FOREIGN KEY([BudgetPlanTypeID])
REFERENCES [dbo].[MBudgetPlanType] ([BudgetPlanTypeID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MBudgetPlanTemplate] CHECK CONSTRAINT [FK_MBudgetPlanTemplate_MBudgetPlanType]
GO
ALTER TABLE [dbo].[MCluster]  WITH CHECK ADD  CONSTRAINT [FK_MCluster_MProject] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[MProject] ([ProjectID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MCluster] CHECK CONSTRAINT [FK_MCluster_MProject]
GO
ALTER TABLE [dbo].[MCompany]  WITH CHECK ADD  CONSTRAINT [FK_MCompany_MCountry] FOREIGN KEY([CountryID])
REFERENCES [dbo].[MCountry] ([CountryID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MCompany] CHECK CONSTRAINT [FK_MCompany_MCountry]
GO
ALTER TABLE [dbo].[MDivision]  WITH CHECK ADD  CONSTRAINT [FK_MDivision_MBusinessUnit] FOREIGN KEY([BusinessUnitID])
REFERENCES [dbo].[MBusinessUnit] ([BusinessUnitID])
GO
ALTER TABLE [dbo].[MDivision] CHECK CONSTRAINT [FK_MDivision_MBusinessUnit]
GO
ALTER TABLE [dbo].[MEmployee]  WITH NOCHECK ADD  CONSTRAINT [FK_MEmployee_MCountry] FOREIGN KEY([BirthCountry])
REFERENCES [dbo].[MCountry] ([CountryID])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[MEmployee] NOCHECK CONSTRAINT [FK_MEmployee_MCountry]
GO
ALTER TABLE [dbo].[MEmployee]  WITH NOCHECK ADD  CONSTRAINT [FK_MEmployee_MCountry1] FOREIGN KEY([Nationality])
REFERENCES [dbo].[MCountry] ([CountryID])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[MEmployee] NOCHECK CONSTRAINT [FK_MEmployee_MCountry1]
GO
ALTER TABLE [dbo].[MFPT]  WITH CHECK ADD  CONSTRAINT [FK_MFPT_MBusinessUnit] FOREIGN KEY([BusinessUnitID])
REFERENCES [dbo].[MBusinessUnit] ([BusinessUnitID])
GO
ALTER TABLE [dbo].[MFPT] CHECK CONSTRAINT [FK_MFPT_MBusinessUnit]
GO
ALTER TABLE [dbo].[MFPT]  WITH CHECK ADD  CONSTRAINT [FK_MFPT_MCluster] FOREIGN KEY([ClusterID])
REFERENCES [dbo].[MCluster] ([ClusterID])
GO
ALTER TABLE [dbo].[MFPT] CHECK CONSTRAINT [FK_MFPT_MCluster]
GO
ALTER TABLE [dbo].[MFPT]  WITH CHECK ADD  CONSTRAINT [FK_MFPT_MDivision] FOREIGN KEY([DivisionID])
REFERENCES [dbo].[MDivision] ([DivisionID])
GO
ALTER TABLE [dbo].[MFPT] CHECK CONSTRAINT [FK_MFPT_MDivision]
GO
ALTER TABLE [dbo].[MFPT]  WITH CHECK ADD  CONSTRAINT [FK_MFPT_MProject] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[MProject] ([ProjectID])
GO
ALTER TABLE [dbo].[MFPT] CHECK CONSTRAINT [FK_MFPT_MProject]
GO
ALTER TABLE [dbo].[MItem]  WITH CHECK ADD  CONSTRAINT [FK_MItem_MItemGroup] FOREIGN KEY([ItemGroupID])
REFERENCES [dbo].[MItemGroup] ([ItemGroupID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MItem] CHECK CONSTRAINT [FK_MItem_MItemGroup]
GO
ALTER TABLE [dbo].[MItem]  WITH CHECK ADD  CONSTRAINT [FK_MItem_MUoM] FOREIGN KEY([UoMID])
REFERENCES [dbo].[MUoM] ([UoMID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MItem] CHECK CONSTRAINT [FK_MItem_MUoM]
GO
ALTER TABLE [dbo].[MItemComparison]  WITH CHECK ADD  CONSTRAINT [FK_MItemComparison_MUsers] FOREIGN KEY([UserID])
REFERENCES [dbo].[MUser] ([UserID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MItemComparison] CHECK CONSTRAINT [FK_MItemComparison_MUsers]
GO
ALTER TABLE [dbo].[MItemGroup]  WITH CHECK ADD  CONSTRAINT [FK_MItemGroup_MItemType] FOREIGN KEY([ItemTypeID])
REFERENCES [dbo].[MItemType] ([ItemTypeID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MItemGroup] CHECK CONSTRAINT [FK_MItemGroup_MItemType]
GO
ALTER TABLE [dbo].[MItemType]  WITH CHECK ADD  CONSTRAINT [FK_MItemType_MItemType] FOREIGN KEY([ItemTypeParentID])
REFERENCES [dbo].[MItemType] ([ItemTypeID])
GO
ALTER TABLE [dbo].[MItemType] CHECK CONSTRAINT [FK_MItemType_MItemType]
GO
ALTER TABLE [dbo].[MLocation]  WITH CHECK ADD  CONSTRAINT [FK_MLocation_MRegion] FOREIGN KEY([RegionID])
REFERENCES [dbo].[MRegion] ([RegionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MLocation] CHECK CONSTRAINT [FK_MLocation_MRegion]
GO
ALTER TABLE [dbo].[MMailNotifications]  WITH CHECK ADD  CONSTRAINT [FK_MMailNotifications_MFunctions] FOREIGN KEY([FunctionID])
REFERENCES [dbo].[MFunctions] ([FunctionID])
GO
ALTER TABLE [dbo].[MMailNotifications] CHECK CONSTRAINT [FK_MMailNotifications_MFunctions]
GO
ALTER TABLE [dbo].[MMailNotifications]  WITH CHECK ADD  CONSTRAINT [FK_MMailNotifications_MNotificationTemplates] FOREIGN KEY([NotificationTemplateID])
REFERENCES [dbo].[MNotificationTemplates] ([NotificationTemplateID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MMailNotifications] CHECK CONSTRAINT [FK_MMailNotifications_MNotificationTemplates]
GO
ALTER TABLE [dbo].[MMailNotifications]  WITH CHECK ADD  CONSTRAINT [MFPT_MMailNotifications_FK2] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[MMailNotifications] CHECK CONSTRAINT [MFPT_MMailNotifications_FK2]
GO
ALTER TABLE [dbo].[MMailNotifications]  WITH CHECK ADD  CONSTRAINT [MTasks_MMailNotifications_FK1] FOREIGN KEY([TaskID])
REFERENCES [dbo].[MTasks] ([TaskID])
GO
ALTER TABLE [dbo].[MMailNotifications] CHECK CONSTRAINT [MTasks_MMailNotifications_FK1]
GO
ALTER TABLE [dbo].[MMinuteTemplates]  WITH CHECK ADD  CONSTRAINT [MFunctions_MMinuteTemplates_FK1] FOREIGN KEY([FunctionID])
REFERENCES [dbo].[MFunctions] ([FunctionID])
GO
ALTER TABLE [dbo].[MMinuteTemplates] CHECK CONSTRAINT [MFunctions_MMinuteTemplates_FK1]
GO
ALTER TABLE [dbo].[MPersonnelArea]  WITH NOCHECK ADD  CONSTRAINT [FK_MPersonnelArea_MCompanyCode] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[MCompany] ([CompanyID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[MPersonnelArea] NOCHECK CONSTRAINT [FK_MPersonnelArea_MCompanyCode]
GO
ALTER TABLE [dbo].[MPersonnelSubarea]  WITH NOCHECK ADD  CONSTRAINT [FK_MPersonnelSubarea_MPersonnelArea] FOREIGN KEY([PersonnelAreaID])
REFERENCES [dbo].[MPersonnelArea] ([PersonnelAreaID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[MPersonnelSubarea] NOCHECK CONSTRAINT [FK_MPersonnelSubarea_MPersonnelArea]
GO
ALTER TABLE [dbo].[MPreBuildRecipientTemplates]  WITH CHECK ADD  CONSTRAINT [MPreBuildRecipientTemplates_MFunctions_FK1] FOREIGN KEY([FunctionID])
REFERENCES [dbo].[MFunctions] ([FunctionID])
GO
ALTER TABLE [dbo].[MPreBuildRecipientTemplates] CHECK CONSTRAINT [MPreBuildRecipientTemplates_MFunctions_FK1]
GO
ALTER TABLE [dbo].[MProject]  WITH CHECK ADD  CONSTRAINT [FK_MProject_MCompany] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[MCompany] ([CompanyID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MProject] CHECK CONSTRAINT [FK_MProject_MCompany]
GO
ALTER TABLE [dbo].[MProject]  WITH CHECK ADD  CONSTRAINT [FK_MProject_MDivision] FOREIGN KEY([DivisionID])
REFERENCES [dbo].[MDivision] ([DivisionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MProject] CHECK CONSTRAINT [FK_MProject_MDivision]
GO
ALTER TABLE [dbo].[MRegion]  WITH CHECK ADD  CONSTRAINT [FK_MRegion_MCountry] FOREIGN KEY([CountryID])
REFERENCES [dbo].[MCountry] ([CountryID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MRegion] CHECK CONSTRAINT [FK_MRegion_MCountry]
GO
ALTER TABLE [dbo].[MSchedules]  WITH CHECK ADD  CONSTRAINT [FK_MSchedules_MCluster] FOREIGN KEY([ClusterID])
REFERENCES [dbo].[MCluster] ([ClusterID])
GO
ALTER TABLE [dbo].[MSchedules] CHECK CONSTRAINT [FK_MSchedules_MCluster]
GO
ALTER TABLE [dbo].[MSchedules]  WITH CHECK ADD  CONSTRAINT [FK_MSchedules_MProject] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[MProject] ([ProjectID])
GO
ALTER TABLE [dbo].[MSchedules] CHECK CONSTRAINT [FK_MSchedules_MProject]
GO
ALTER TABLE [dbo].[MSchedules]  WITH CHECK ADD  CONSTRAINT [MFPT_MSchedules_FK2] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[MSchedules] CHECK CONSTRAINT [MFPT_MSchedules_FK2]
GO
ALTER TABLE [dbo].[MSchedules]  WITH CHECK ADD  CONSTRAINT [MMailNotifications_MSchedules_FK4] FOREIGN KEY([MailNotificationID])
REFERENCES [dbo].[MMailNotifications] ([MailNotificationID])
GO
ALTER TABLE [dbo].[MSchedules] CHECK CONSTRAINT [MMailNotifications_MSchedules_FK4]
GO
ALTER TABLE [dbo].[MSchedules]  WITH CHECK ADD  CONSTRAINT [MTasks_MSchedules_FK1] FOREIGN KEY([TaskID])
REFERENCES [dbo].[MTasks] ([TaskID])
GO
ALTER TABLE [dbo].[MSchedules] CHECK CONSTRAINT [MTasks_MSchedules_FK1]
GO
ALTER TABLE [dbo].[MTasks]  WITH CHECK ADD  CONSTRAINT [MTaskTypes_MTasks_FK1] FOREIGN KEY([TaskTypeID])
REFERENCES [dbo].[MTaskTypes] ([TaskTypeID])
GO
ALTER TABLE [dbo].[MTasks] CHECK CONSTRAINT [MTaskTypes_MTasks_FK1]
GO
ALTER TABLE [dbo].[MTaskTypes]  WITH CHECK ADD  CONSTRAINT [FK_MTaskTypes_MTaskGroup] FOREIGN KEY([TaskGroupID])
REFERENCES [dbo].[MTaskGroup] ([TaskGroupID])
GO
ALTER TABLE [dbo].[MTaskTypes] CHECK CONSTRAINT [FK_MTaskTypes_MTaskGroup]
GO
ALTER TABLE [dbo].[MUoM]  WITH CHECK ADD  CONSTRAINT [FK_MUoM_SDimension] FOREIGN KEY([DimensionID])
REFERENCES [dbo].[SDimension] ([DimensionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MUoM] CHECK CONSTRAINT [FK_MUoM_SDimension]
GO
ALTER TABLE [dbo].[MUser]  WITH CHECK ADD  CONSTRAINT [FK_MUser_MBusinessUnit] FOREIGN KEY([BusinessUnitID])
REFERENCES [dbo].[MBusinessUnit] ([BusinessUnitID])
GO
ALTER TABLE [dbo].[MUser] CHECK CONSTRAINT [FK_MUser_MBusinessUnit]
GO
ALTER TABLE [dbo].[MUser]  WITH CHECK ADD  CONSTRAINT [FK_MUser_MCluster] FOREIGN KEY([ClusterID])
REFERENCES [dbo].[MCluster] ([ClusterID])
GO
ALTER TABLE [dbo].[MUser] CHECK CONSTRAINT [FK_MUser_MCluster]
GO
ALTER TABLE [dbo].[MUser]  WITH CHECK ADD  CONSTRAINT [FK_MUser_MDivision] FOREIGN KEY([DivisionID])
REFERENCES [dbo].[MDivision] ([DivisionID])
GO
ALTER TABLE [dbo].[MUser] CHECK CONSTRAINT [FK_MUser_MDivision]
GO
ALTER TABLE [dbo].[MUser]  WITH CHECK ADD  CONSTRAINT [FK_MUser_MEmployee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MEmployee] ([EmployeeID])
GO
ALTER TABLE [dbo].[MUser] CHECK CONSTRAINT [FK_MUser_MEmployee]
GO
ALTER TABLE [dbo].[MUser]  WITH CHECK ADD  CONSTRAINT [FK_MUser_MProject] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[MProject] ([ProjectID])
GO
ALTER TABLE [dbo].[MUser] CHECK CONSTRAINT [FK_MUser_MProject]
GO
ALTER TABLE [dbo].[MUser]  WITH CHECK ADD  CONSTRAINT [FK_MUser_MVendor] FOREIGN KEY([VendorID])
REFERENCES [dbo].[MVendor] ([VendorID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MUser] CHECK CONSTRAINT [FK_MUser_MVendor]
GO
ALTER TABLE [dbo].[MVendor]  WITH CHECK ADD  CONSTRAINT [FK_MVendor_MVendorSubcategory] FOREIGN KEY([VendorSubcategoryID])
REFERENCES [dbo].[MVendorSubcategory] ([VendorSubcategoryID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MVendor] CHECK CONSTRAINT [FK_MVendor_MVendorSubcategory]
GO
ALTER TABLE [dbo].[MVendorSubcategory]  WITH CHECK ADD  CONSTRAINT [FK_MVendorSubcategory_MVendorCategory] FOREIGN KEY([VendorCategoryID])
REFERENCES [dbo].[MVendorCategory] ([VendorCategoryID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MVendorSubcategory] CHECK CONSTRAINT [FK_MVendorSubcategory_MVendorCategory]
GO
ALTER TABLE [dbo].[MWBS]  WITH CHECK ADD  CONSTRAINT [FK_MWBS_MProject] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[MProject] ([ProjectID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[MWBS] CHECK CONSTRAINT [FK_MWBS_MProject]
GO
ALTER TABLE [dbo].[TBudgetPlan]  WITH CHECK ADD  CONSTRAINT [FK_TBudgetPlan_MBudgetPlanTemplate] FOREIGN KEY([BudgetPlanTemplateID])
REFERENCES [dbo].[MBudgetPlanTemplate] ([BudgetPlanTemplateID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[TBudgetPlan] CHECK CONSTRAINT [FK_TBudgetPlan_MBudgetPlanTemplate]
GO
ALTER TABLE [dbo].[TBudgetPlan]  WITH CHECK ADD  CONSTRAINT [FK_TBudgetPlan_MProject] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[MProject] ([ProjectID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[TBudgetPlan] CHECK CONSTRAINT [FK_TBudgetPlan_MProject]
GO
ALTER TABLE [dbo].[TBudgetPlan]  WITH CHECK ADD  CONSTRAINT [FK_TBudgetPlan_MUnitType] FOREIGN KEY([UnitTypeID])
REFERENCES [dbo].[MUnitType] ([UnitTypeID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[TBudgetPlan] CHECK CONSTRAINT [FK_TBudgetPlan_MUnitType]
GO
ALTER TABLE [dbo].[TCatalogCart]  WITH CHECK ADD  CONSTRAINT [FK_TCatalogCart_MUsers] FOREIGN KEY([UserID])
REFERENCES [dbo].[MUser] ([UserID])
GO
ALTER TABLE [dbo].[TCatalogCart] CHECK CONSTRAINT [FK_TCatalogCart_MUsers]
GO
ALTER TABLE [dbo].[TEntryValues]  WITH CHECK ADD  CONSTRAINT [MFieldTagReferences_TEntryValues_FK2] FOREIGN KEY([FieldTagID])
REFERENCES [dbo].[MFieldTagReferences] ([FieldTagID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TEntryValues] CHECK CONSTRAINT [MFieldTagReferences_TEntryValues_FK2]
GO
ALTER TABLE [dbo].[TEntryValues]  WITH CHECK ADD  CONSTRAINT [TMinuteEntry_TEntryValues_FK1] FOREIGN KEY([MinuteEntryID])
REFERENCES [dbo].[TMinuteEntries] ([MinuteEntryID])
GO
ALTER TABLE [dbo].[TEntryValues] CHECK CONSTRAINT [TMinuteEntry_TEntryValues_FK1]
GO
ALTER TABLE [dbo].[TEventLog]  WITH CHECK ADD  CONSTRAINT [FK_TEventLog_SAction] FOREIGN KEY([ActionID])
REFERENCES [dbo].[SAction] ([ActionID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[TEventLog] CHECK CONSTRAINT [FK_TEventLog_SAction]
GO
ALTER TABLE [dbo].[TEventLog]  WITH CHECK ADD  CONSTRAINT [FK_TEventLog_SMenu] FOREIGN KEY([MenuID])
REFERENCES [dbo].[SMenu] ([MenuID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[TEventLog] CHECK CONSTRAINT [FK_TEventLog_SMenu]
GO
ALTER TABLE [dbo].[TFPTAttendances]  WITH CHECK ADD  CONSTRAINT [MFPT_TFPTAttendances_FK1] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[TFPTAttendances] CHECK CONSTRAINT [MFPT_TFPTAttendances_FK1]
GO
ALTER TABLE [dbo].[TMailHistories]  WITH CHECK ADD  CONSTRAINT [MMailNotifications_TMailHistories_FK1] FOREIGN KEY([MailNotificationID])
REFERENCES [dbo].[MMailNotifications] ([MailNotificationID])
GO
ALTER TABLE [dbo].[TMailHistories] CHECK CONSTRAINT [MMailNotifications_TMailHistories_FK1]
GO
ALTER TABLE [dbo].[TMinuteEntries]  WITH CHECK ADD  CONSTRAINT [MFPT_TMinuteEntries_FK5] FOREIGN KEY([FPTID])
REFERENCES [dbo].[MFPT] ([FPTID])
GO
ALTER TABLE [dbo].[TMinuteEntries] CHECK CONSTRAINT [MFPT_TMinuteEntries_FK5]
GO
ALTER TABLE [dbo].[TMinuteEntries]  WITH CHECK ADD  CONSTRAINT [MMailNotifications_TMinuteEntries_FK4] FOREIGN KEY([MailNotificationID])
REFERENCES [dbo].[MMailNotifications] ([MailNotificationID])
GO
ALTER TABLE [dbo].[TMinuteEntries] CHECK CONSTRAINT [MMailNotifications_TMinuteEntries_FK4]
GO
ALTER TABLE [dbo].[TMinuteEntries]  WITH CHECK ADD  CONSTRAINT [MMinuteTemplates_TMinuteEntries_FK2] FOREIGN KEY([MinuteTemplateID])
REFERENCES [dbo].[MMinuteTemplates] ([MinuteTemplateID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TMinuteEntries] CHECK CONSTRAINT [MMinuteTemplates_TMinuteEntries_FK2]
GO
ALTER TABLE [dbo].[TMinuteEntries]  WITH CHECK ADD  CONSTRAINT [MTasks_TMinuteEntries_FK3] FOREIGN KEY([TaskID])
REFERENCES [dbo].[MTasks] ([TaskID])
GO
ALTER TABLE [dbo].[TMinuteEntries] CHECK CONSTRAINT [MTasks_TMinuteEntries_FK3]
GO
ALTER TABLE [dbo].[TNegotiationBidStructures]  WITH CHECK ADD  CONSTRAINT [CNegotiationConfigurations_TNegotiationBidStructures_FK1] FOREIGN KEY([NegotiationConfigID])
REFERENCES [dbo].[CNegotiationConfigurations] ([NegotiationConfigID])
GO
ALTER TABLE [dbo].[TNegotiationBidStructures] CHECK CONSTRAINT [CNegotiationConfigurations_TNegotiationBidStructures_FK1]
GO
ALTER TABLE [dbo].[TNotificationAttachments]  WITH CHECK ADD  CONSTRAINT [MMailNotifications_TNotificationAttachments_FK1] FOREIGN KEY([MailNotificationID])
REFERENCES [dbo].[MMailNotifications] ([MailNotificationID])
GO
ALTER TABLE [dbo].[TNotificationAttachments] CHECK CONSTRAINT [MMailNotifications_TNotificationAttachments_FK1]
GO
ALTER TABLE [dbo].[TNotificationValues]  WITH CHECK ADD  CONSTRAINT [MFieldTagReferences_TNotificationValues_FK2] FOREIGN KEY([FieldTagID])
REFERENCES [dbo].[MFieldTagReferences] ([FieldTagID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TNotificationValues] CHECK CONSTRAINT [MFieldTagReferences_TNotificationValues_FK2]
GO
ALTER TABLE [dbo].[TNotificationValues]  WITH CHECK ADD  CONSTRAINT [MMailNotification_TNotificationValues_FK1] FOREIGN KEY([MailNotificationID])
REFERENCES [dbo].[MMailNotifications] ([MailNotificationID])
GO
ALTER TABLE [dbo].[TNotificationValues] CHECK CONSTRAINT [MMailNotification_TNotificationValues_FK1]
GO
ALTER TABLE [dbo].[TNumbering]  WITH NOCHECK ADD  CONSTRAINT [FK_TNumbering_MCompany] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[MCompany] ([CompanyID])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[TNumbering] NOCHECK CONSTRAINT [FK_TNumbering_MCompany]
GO
ALTER TABLE [dbo].[TNumbering]  WITH NOCHECK ADD  CONSTRAINT [FK_TNumbering_MProject] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[MProject] ([ProjectID])
ON UPDATE CASCADE
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[TNumbering] NOCHECK CONSTRAINT [FK_TNumbering_MProject]
GO
ALTER TABLE [dbo].[TTCMemberDelegations]  WITH CHECK ADD  CONSTRAINT [TTCMembers_TTCDelegations_FK1] FOREIGN KEY([TCMemberID])
REFERENCES [dbo].[TTCMembers] ([TCMemberID])
GO
ALTER TABLE [dbo].[TTCMemberDelegations] CHECK CONSTRAINT [TTCMembers_TTCDelegations_FK1]
GO
ALTER TABLE [dbo].[TTCMemberDelegations]  WITH CHECK ADD  CONSTRAINT [TTCMembers_TTCDelegations_FK2] FOREIGN KEY([DelegateTo])
REFERENCES [dbo].[TTCMembers] ([TCMemberID])
GO
ALTER TABLE [dbo].[TTCMemberDelegations] CHECK CONSTRAINT [TTCMembers_TTCDelegations_FK2]
GO
ALTER TABLE [dbo].[TTCMembers]  WITH CHECK ADD  CONSTRAINT [FK_TTCMembers_MBusinessUnit] FOREIGN KEY([BusinessUnitID])
REFERENCES [dbo].[MBusinessUnit] ([BusinessUnitID])
GO
ALTER TABLE [dbo].[TTCMembers] CHECK CONSTRAINT [FK_TTCMembers_MBusinessUnit]
GO
ALTER TABLE [dbo].[TTCMembers]  WITH CHECK ADD  CONSTRAINT [FK_TTCMembers_MEmployee] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[MEmployee] ([EmployeeID])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[TTCMembers] CHECK CONSTRAINT [FK_TTCMembers_MEmployee]
GO
ALTER TABLE [dbo].[TTCMembers]  WITH CHECK ADD  CONSTRAINT [FK_TTCMembers_MEmployee1] FOREIGN KEY([SuperiorID])
REFERENCES [dbo].[MEmployee] ([EmployeeID])
GO
ALTER TABLE [dbo].[TTCMembers] CHECK CONSTRAINT [FK_TTCMembers_MEmployee1]
GO
ALTER TABLE [dbo].[TTCMembers]  WITH CHECK ADD  CONSTRAINT [FK_TTCMembers_MTCTypes] FOREIGN KEY([TCTypeID])
REFERENCES [dbo].[MTCTypes] ([TCTypeID])
GO
ALTER TABLE [dbo].[TTCMembers] CHECK CONSTRAINT [FK_TTCMembers_MTCTypes]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jika TemplateType = MOM refers to MMinusTemplates.MinutesTemplateID;
Jika TemplateType = NOT refers to MNotificationTemplates.NotificationTemplateID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'DTemplateTags', @level2type=N'COLUMN',@level2name=N'TemplateID'
GO
