USE [hrms]
GO
/****** Object:  UserDefinedFunction [dbo].[GetDistanceInKilometersByCoordinates]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: Sept 25, 2020
-- =============================================
CREATE FUNCTION [dbo].[GetDistanceInKilometersByCoordinates]
(
	@LatitudeOrigin float,
	@LongitudeOrigin float,
	@LatitudeDistination float,
	@LongitudeDistination float
)
RETURNS float
AS
BEGIN
	-- CONSTANTS
	DECLARE @EarthRadiusInKM float,
	@PI float,
	@LatitudeOriginRadians float,
	@LongitudeOriginRadians float,
	@LatitudeDistinationRadians float,
	@LongitudeDistinationRadians float,
	@Distance decimal;

	SET @EarthRadiusInKM = 6371;
	SET @PI = PI();
	
	-- RADIANS conversion
	SET @LatitudeOriginRadians = @LatitudeOrigin * @PI / 180;
	SET @LongitudeOriginRadians = @LongitudeOrigin * @PI / 180;
	SET @LatitudeDistinationRadians = @LatitudeDistination * @PI / 180;
	SET @LongitudeDistinationRadians = @LongitudeDistination * @PI / 180;
	SET @Distance = ROUND(Acos(
	Cos(@LatitudeOriginRadians) * Cos(@LongitudeOriginRadians) * Cos(@LatitudeDistinationRadians) * Cos(@LongitudeDistinationRadians) +
	Cos(@LatitudeOriginRadians) * Sin(@LongitudeOriginRadians) * Cos(@LatitudeDistinationRadians) * Sin(@LongitudeDistinationRadians) +
	Sin(@LatitudeOriginRadians) * Sin(@LatitudeDistinationRadians)
	) * @EarthRadiusInKM, 2);
	RETURN @Distance;
END


GO
/****** Object:  UserDefinedFunction [dbo].[LPAD]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: Sept 25, 2020
-- Description:	FOR GENERATING SEQUENCE ID WITH PADDING LENGTH
-- =============================================
CREATE FUNCTION [dbo].[LPAD]
(
    @string VARCHAR(MAX), -- Initial string
    @length INT,          -- Size of final string
    @pad CHAR             -- Pad character
)
RETURNS VARCHAR(MAX)
AS
BEGIN
    RETURN REPLICATE(@pad, @length - LEN(@string)) + @string;
END




GO
/****** Object:  UserDefinedFunction [dbo].[svf_getUserFullName]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: Sept 25, 2020
-- Description:	FOR GENERATING SEQUENCE ID WITH PADDING LENGTH
-- =============================================
CREATE FUNCTION [dbo].[svf_getUserFullName]
(
    @SystemUserId NVARCHAR(100)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
    RETURN (SELECT TOP 1 CONCAT(ISNULL(le.[FirstName],''), ' ', ISNULL(le.[MiddleName],''), ' ', ISNULL(le.[LastName],'')) 
	FROM [dbo].[SystemUser] AS su
	LEFT JOIN [dbo].[LegalEntity] le ON su.LegalEntityId = le.LegalEntityId
	WHERE su.SystemUserId = @SystemUserId)
END




GO
/****** Object:  UserDefinedFunction [dbo].[tvf_SplitString]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[tvf_SplitString]
(    
      @Input NVARCHAR(MAX),
      @Character CHAR(1)
)
RETURNS @Output TABLE (
      Item NVARCHAR(1000)
)
AS
BEGIN
      DECLARE @StartIndex INT, @EndIndex INT
 
      SET @StartIndex = 1
      IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
      BEGIN
            SET @Input = @Input + @Character
      END
 
      WHILE CHARINDEX(@Character, @Input) > 0
      BEGIN
            SET @EndIndex = CHARINDEX(@Character, @Input)
           
            INSERT INTO @Output(Item)
            SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)
           
            SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
      END
		delete from @Output where Item = ''
      RETURN
END


GO
/****** Object:  Table [dbo].[AppointmentStatus]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AppointmentStatus](
	[AppointmentStatusId] [bigint] NOT NULL,
	[AppointmentStatusName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_AppointmentStatus] PRIMARY KEY CLUSTERED 
(
	[AppointmentStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntityApprovalStatus]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntityApprovalStatus](
	[ApprovalStatusId] [bigint] NOT NULL,
	[ApprovalStatusName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_EntityApprovalStatus] PRIMARY KEY CLUSTERED 
(
	[ApprovalStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntityGender]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntityGender](
	[GenderId] [bigint] NOT NULL,
	[GenderName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_EntityGender] PRIMARY KEY CLUSTERED 
(
	[GenderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntityStatus]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntityStatus](
	[EntityStatusId] [bigint] NOT NULL,
	[EntityStatusName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_EntityStatus] PRIMARY KEY CLUSTERED 
(
	[EntityStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[File]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[File](
	[FileId] [nvarchar](100) NOT NULL,
	[FileName] [nvarchar](250) NOT NULL,
	[MimeType] [nvarchar](100) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[FileContent] [varbinary](max) NULL,
	[IsFromStorage] [bit] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[FileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LegalEntity]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LegalEntity](
	[LegalEntityId] [nvarchar](100) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[MiddleName] [nvarchar](50) NULL,
	[GenderId] [nvarchar](100) NOT NULL,
	[BirthDate] [date] NOT NULL,
	[Age]  AS ((CONVERT([int],CONVERT([char](8),getdate(),(112)))-CONVERT([char](8),[BirthDate],(112)))/(10000)),
	[EmailAddress] [nvarchar](100) NOT NULL,
	[MobileNumber] [bigint] NOT NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [LegalEntityId] PRIMARY KEY CLUSTERED 
(
	[LegalEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LegalEntityAddress]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LegalEntityAddress](
	[LegalEntityAddressId] [nvarchar](100) NOT NULL,
	[LegalEntityId] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_LegalGeoAddress] PRIMARY KEY CLUSTERED 
(
	[LegalEntityAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sequence]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sequence](
	[SequenceId] [bigint] NOT NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[Prefix] [nvarchar](10) NOT NULL,
	[Length] [int] NOT NULL,
	[LastNumber] [bigint] NOT NULL,
 CONSTRAINT [PK_Sequence] PRIMARY KEY CLUSTERED 
(
	[SequenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemConfig]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfig](
	[SystemConfigId] [bigint] NOT NULL,
	[ConfigName] [nvarchar](100) NOT NULL,
	[ConfigGroup] [nvarchar](100) NOT NULL,
	[ConfigKey] [nvarchar](100) NOT NULL,
	[ConfigValue] [varchar](max) NOT NULL,
	[SystemConfigTypeId] [bigint] NOT NULL,
	[IsUserConfigurable] [bit] NOT NULL,
 CONSTRAINT [PK_SystemConfig] PRIMARY KEY CLUSTERED 
(
	[SystemConfigId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemConfigType]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemConfigType](
	[SystemConfigTypeId] [bigint] NOT NULL,
	[ValueType] [nvarchar](100) NULL,
 CONSTRAINT [PK_SystemConfigType] PRIMARY KEY CLUSTERED 
(
	[SystemConfigTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemToken]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemToken](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TokenId] [nvarchar](max) NOT NULL,
	[SystemUserId] [nvarchar](100) NOT NULL,
	[ClientId] [nvarchar](250) NOT NULL,
	[Subject] [nvarchar](250) NOT NULL,
	[IssuedUtc] [datetime] NOT NULL,
	[ExpiresUtc] [datetime] NOT NULL,
	[ProtectedTicket] [nvarchar](max) NOT NULL,
	[TokenType] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_SystemToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemUser]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemUser](
	[SystemUserId] [nvarchar](100) NOT NULL,
	[SystemUserTypeId] [bigint] NOT NULL,
	[LegalEntityId] [nvarchar](100) NOT NULL,
	[ProfilePictureFile] [nvarchar](100) NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[RefreshToken] [nvarchar](max) NULL,
	[IsWebAdminGuestUser] [bit] NULL,
	[HasFirstLogin] [bit] NULL,
	[LasteDateTimeActive] [datetime] NULL,
	[LasteDateTimeLogin] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[SystemUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemUserConfig]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemUserConfig](
	[SystemUserConfigId] [nvarchar](100) NOT NULL,
	[SystemUserId] [nvarchar](100) NOT NULL,
	[IsUserEnable] [bit] NOT NULL,
 CONSTRAINT [PK_SystemUserConfig] PRIMARY KEY CLUSTERED 
(
	[SystemUserConfigId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemUserContact]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemUserContact](
	[SystemUserContactId] [nvarchar](100) NOT NULL,
	[SystemUsersId] [nvarchar](100) NOT NULL,
	[ContactTypeId] [bigint] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_SystemUserContact] PRIMARY KEY CLUSTERED 
(
	[SystemUserContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemUserType]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemUserType](
	[SystemUserTypeId] [bigint] NOT NULL,
	[SystemUserTypeName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SystemUserType] PRIMARY KEY CLUSTERED 
(
	[SystemUserTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemUserVerification]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemUserVerification](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[VerificationSender] [nvarchar](100) NOT NULL,
	[VerificationTypeId] [bigint] NOT NULL,
	[VerificationCode] [nvarchar](50) NOT NULL,
	[IsVerified] [bit] NOT NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_SystemUserVerification] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemUserVerificationType]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemUserVerificationType](
	[VerificationTypeId] [bigint] NOT NULL,
	[VerificationTypeName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SystemUserVerificationType] PRIMARY KEY CLUSTERED 
(
	[VerificationTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWebAdminMenu]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWebAdminMenu](
	[SystemWebAdminMenuId] [bigint] NOT NULL,
	[SystemWebAdminModuleId] [bigint] NOT NULL,
	[SystemWebAdminMenuName] [nvarchar](100) NOT NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_SystemWebAdminMenu] PRIMARY KEY CLUSTERED 
(
	[SystemWebAdminMenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWebAdminMenuRole]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWebAdminMenuRole](
	[SystemWebAdminMenuRoleId] [nvarchar](100) NOT NULL,
	[SystemWebAdminMenuId] [nvarchar](100) NOT NULL,
	[SystemWebAdminRoleId] [nvarchar](100) NOT NULL,
	[IsAllowed] [bit] NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_SystemWebAdminMenuRole] PRIMARY KEY CLUSTERED 
(
	[SystemWebAdminMenuRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWebAdminModule]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWebAdminModule](
	[SystemWebAdminModuleId] [bigint] NOT NULL,
	[SystemWebAdminModuleName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SystemWebAdminModule] PRIMARY KEY CLUSTERED 
(
	[SystemWebAdminModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWebAdminPrivileges]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWebAdminPrivileges](
	[SystemWebAdminPrivilegeId] [bigint] NOT NULL,
	[SystemWebAdminPrivilegeName] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_SystemWebAdminPrivileges] PRIMARY KEY CLUSTERED 
(
	[SystemWebAdminPrivilegeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWebAdminRole]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWebAdminRole](
	[SystemWebAdminRoleId] [nvarchar](100) NOT NULL,
	[RoleName] [nvarchar](250) NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_SystemRole] PRIMARY KEY CLUSTERED 
(
	[SystemWebAdminRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWebAdminRolePrivileges]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWebAdminRolePrivileges](
	[SystemWebAdminRolePrivilegeId] [nvarchar](100) NOT NULL,
	[SystemWebAdminRoleId] [nvarchar](100) NOT NULL,
	[SystemWebAdminPrivilegeId] [bigint] NOT NULL,
	[IsAllowed] [bit] NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_SystemWebAdminRolePrivileges] PRIMARY KEY CLUSTERED 
(
	[SystemWebAdminRolePrivilegeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemWebAdminUserRole]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemWebAdminUserRole](
	[SystemWebAdminUserRoleId] [nvarchar](100) NOT NULL,
	[SystemUserId] [nvarchar](100) NOT NULL,
	[SystemWebAdminRoleId] [nvarchar](100) NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_SystemUserRoles] PRIMARY KEY CLUSTERED 
(
	[SystemWebAdminUserRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[EntityApprovalStatus] ([ApprovalStatusId], [ApprovalStatusName]) VALUES (1, N'Approved')
GO
INSERT [dbo].[EntityApprovalStatus] ([ApprovalStatusId], [ApprovalStatusName]) VALUES (2, N'Declined')
GO
INSERT [dbo].[EntityApprovalStatus] ([ApprovalStatusId], [ApprovalStatusName]) VALUES (3, N'Pending')
GO
INSERT [dbo].[EntityGender] ([GenderId], [GenderName]) VALUES (1, N'Male')
GO
INSERT [dbo].[EntityGender] ([GenderId], [GenderName]) VALUES (2, N'Female')
GO
INSERT [dbo].[EntityStatus] ([EntityStatusId], [EntityStatusName]) VALUES (1, N'Active')
GO
INSERT [dbo].[EntityStatus] ([EntityStatusId], [EntityStatusName]) VALUES (2, N'Deleted')
GO
INSERT [dbo].[File] ([FileId], [FileName], [MimeType], [FileSize], [FileContent], [IsFromStorage], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'F-0000000001', N'C:\Archive\Development\web-hrms-tqmo\HRMS.API\App_Data\DefaultFiles\SystemUser\icons8_user_90_gray.png', N'[ImageFormat: b96b3caf-0728-11d3-9d7b-0000f81ef32e]', 0, 0x89504E470D0A1A0A0000000D494844520000005A0000005A080600000038A84102000000017352474200AECE1CE90000000467414D410000B18F0BFC61050000042E49444154785EED9CC96B144114C6E38AE6A02071017109D1AB7810515410D1A0078FAD267A98617A324D8720F90FFA287A88A28610E2C9E83F2078F1247109495C50C7048DE04D302E1121EED1EFE10B34452999A1BB7AA6EAFDE0A3B34CE6BDEF9B9E4E75754D370882200882200882200855502814B674747474168BC5EBD83E84DE43DF59EFE867F43B28C4D72DFC67C27C88A26831823B89E0EE41BF2BD45DA8DDF3BC45FC74828E52A9741841BD880557AD26A0567E5A610E84D288BD7820165422C20BD7DFDDDDBD9CCBB84D10046B10CA0335A404359ACFE757733937E190E96DAE0B28494D381B36CC374269EEC9AA4673B9DC322EEF0E691C93E7A13E2EEF0630DCAA04604C78818F701B7643E364184E620857ADC69D186763C8754A63DEA8B0579FE076EC0541DFD7993729047D87DBB1139EBBD09A37AC59DFF79BB92DFB80C14EC570962A715BF681B72CCDC2E94C1B170E6183DC967DC0E023D5705642D063DC967DC020CD216B8D67A0B7DC967DC0DC37C56C96FACA6DD987C66CA6E2B6EC4367364B715BF6A1339BA5B82DFB803939469B00E664D46102989371B40960F09A6A38435DE5B6EC03A7E0B4D045673A0BD93BD701732DD06CCC6C569A0D826033B765273059CD0AA4A435C4EDD80B4CB62BA68D0B87B063DC8EBDD0F53A9835B196E35F2A3BB3360F43AB439A008CC8F7FD03DC861B20EC7E5D1029AB97CBBB032D3E84F15125883435ECE44A2582D6C3210013C7EB71A889CBBA09873D120B25690D3BBF9A740E3E8CF4290125A15E670F17FF03E3DB830887DEE6BAD02A51D9B9D145A5D018172392365A4D84C02A395DA7C70EE16F8F4751B4909F4E980FB49A08E121BBD220B6B4969AE6B3E9E201690A3F1FE3DF95AC9FBB10044110044110044170069CE5ADC4D9DE6E9C8217B13D8FEF6F42C3104DA7BE81BEB0E8EB713C863E7C448FE9817032E9EFEAEAEA5AC14F27CC1186E13A0444176CAF40AF20DD3C46359AC46B358017A20DE1AFE5726E8100B62388B3D0B35830698A269C9EA0EE1984BE8DDBB01318DD08A3A7B1A55BF6E8C230A93214616FDFCAEDD53F30B417866E605B0B2B9454514FB7D0DF51B4BAE06FC77584E7794BD17C01269EC64CD5BA1EE31D97C37609DBA86D68EF40B32F6306EA4D7443000F566A730F0F826027F608BA2AA26BBE1E751B3BCD0EB6973D7C5B8808FA196BD216FD822E40D91E4EE8F2119AA07BCDE99AB4492374C300B66D1684BC1F0D4C2B0DD9AC8F1883EF63FB66A082284CA7C1BA866CD60C76B03D1C43BAA05813544B9FAE32AD291C4656711CE981423479A36BC0196134728EE3480F14AAE73172527ACE71A4078ACC28455DD4678E233D50E4B552D4454D721CE981221795A22EAA87E3480F0C6FD6A3D007A5B04B9A0EC37003C7912EF8AF4B1FF6F9A434E082E8A4C5EC7260146C46E097519C66BB7EC49AB14DE48D3C5EC2F87913DB170441100441100441100441709886863F689E7D32F47C17490000000049454E44AE426082, 0, NULL, CAST(N'2022-06-09T22:02:33.050' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[File] ([FileId], [FileName], [MimeType], [FileSize], [FileContent], [IsFromStorage], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'F-0000000002', N'C:\Archive\Development\web-hrms-tqmo\HRMS.API\App_Data\DefaultFiles\SystemUser\icons8_user_90_gray.png', N'[ImageFormat: b96b3caf-0728-11d3-9d7b-0000f81ef32e]', 0, 0x89504E470D0A1A0A0000000D494844520000005A0000005A080600000038A84102000000017352474200AECE1CE90000000467414D410000B18F0BFC61050000042E49444154785EED9CC96B144114C6E38AE6A02071017109D1AB7810515410D1A0078FAD267A98617A324D8720F90FFA287A88A28610E2C9E83F2078F1247109495C50C7048DE04D302E1121EED1EFE10B34452999A1BB7AA6EAFDE0A3B34CE6BDEF9B9E4E75754D370882200882200882200855502814B674747474168BC5EBD83E84DE43DF59EFE867F43B28C4D72DFC67C27C88A26831823B89E0EE41BF2BD45DA8DDF3BC45FC74828E52A9741841BD880557AD26A0567E5A610E84D288BD7820165422C20BD7DFDDDDBD9CCBB84D10046B10CA0335A404359ACFE757733937E190E96DAE0B28494D381B36CC374269EEC9AA4673B9DC322EEF0E691C93E7A13E2EEF0630DCAA04604C78818F701B7643E364184E620857ADC69D186763C8754A63DEA8B0579FE076EC0541DFD7993729047D87DBB1139EBBD09A37AC59DFF79BB92DFB80C14EC570962A715BF681B72CCDC2E94C1B170E6183DC967DC0E023D5705642D063DC967DC020CD216B8D67A0B7DC967DC0DC37C56C96FACA6DD987C66CA6E2B6EC4367364B715BF6A1339BA5B82DFB803939469B00E664D46102989371B40960F09A6A38435DE5B6EC03A7E0B4D045673A0BD93BD701732DD06CCC6C569A0D826033B765273059CD0AA4A435C4EDD80B4CB62BA68D0B87B063DC8EBDD0F53A9835B196E35F2A3BB3360F43AB439A008CC8F7FD03DC861B20EC7E5D1029AB97CBBB032D3E84F15125883435ECE44A2582D6C3210013C7EB71A889CBBA09873D120B25690D3BBF9A740E3E8CF4290125A15E670F17FF03E3DB830887DEE6BAD02A51D9B9D145A5D018172392365A4D84C02A395DA7C70EE16F8F4751B4909F4E980FB49A08E121BBD220B6B4969AE6B3E9E201690A3F1FE3DF95AC9FBB10044110044110044170069CE5ADC4D9DE6E9C8217B13D8FEF6F42C3104DA7BE81BEB0E8EB713C863E7C448FE9817032E9EFEAEAEA5AC14F27CC1186E13A0444176CAF40AF20DD3C46359AC46B358017A20DE1AFE5726E8100B62388B3D0B35830698A269C9EA0EE1984BE8DDBB01318DD08A3A7B1A55BF6E8C230A93214616FDFCAEDD53F30B417866E605B0B2B9454514FB7D0DF51B4BAE06FC77584E7794BD17C01269EC64CD5BA1EE31D97C37609DBA86D68EF40B32F6306EA4D7443000F566A730F0F826027F608BA2AA26BBE1E751B3BCD0EB6973D7C5B8808FA196BD216FD822E40D91E4EE8F2119AA07BCDE99AB4492374C300B66D1684BC1F0D4C2B0DD9AC8F1883EF63FB66A082284CA7C1BA866CD60C76B03D1C43BAA05813544B9FAE32AD291C4656711CE981423479A36BC0196134728EE3480F14AAE73172527ACE71A4078ACC28455DD4678E233D50E4B552D4454D721CE981221795A22EAA87E3480F0C6FD6A3D007A5B04B9A0EC37003C7912EF8AF4B1FF6F9A434E082E8A4C5EC7260146C46E097519C66BB7EC49AB14DE48D3C5EC2F87913DB170441100441100441100441709886863F689E7D32F47C17490000000049454E44AE426082, 0, N'SU-0000000001', CAST(N'2022-06-09T22:15:10.770' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[LegalEntity] ([LegalEntityId], [FirstName], [LastName], [MiddleName], [GenderId], [BirthDate], [EmailAddress], [MobileNumber], [EntityStatusId]) VALUES (N'LE-0000000001', N'Admin', N'Admin', N'', N'1', CAST(N'2022-06-09' AS Date), N'admin@admin.com', 0, 1)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (1, N'SystemUser', N'SU-', 10, 1)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (2, N'SystemUserConfig', N'SUC-', 10, 1)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (3, N'SystemUserContact', N'SUCON-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (4, N'SystemWebAdminUserRole', N'SWAUR-', 10, 1)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (5, N'SystemWebAdminRole', N'SWAR-', 10, 1)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (6, N'LegalEntity', N'LE-', 10, 1)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (7, N'LegalGeoAddress', N'LGA-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (8, N'SystemWebAdminMenuRole', N'SWAMR-', 10, 6)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (9, N'File', N'F-', 10, 2)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (10, N'LegalEntityAddress', N'LEA-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (11, N'SystemWebAdminRolePrivileges', N'SWARP-', 10, 0)
GO
INSERT [dbo].[SystemConfig] ([SystemConfigId], [ConfigName], [ConfigGroup], [ConfigKey], [ConfigValue], [SystemConfigTypeId], [IsUserConfigurable]) VALUES (1, N'System Version', N'System Version', N'SYSTEM_VERSION', N'1', 2, 0)
GO
INSERT [dbo].[SystemConfig] ([SystemConfigId], [ConfigName], [ConfigGroup], [ConfigKey], [ConfigValue], [SystemConfigTypeId], [IsUserConfigurable]) VALUES (2, N'API Version', N'System Version', N'API_VERSION', N'1', 2, 0)
GO
INSERT [dbo].[SystemConfigType] ([SystemConfigTypeId], [ValueType]) VALUES (1, N'BOOLEAN')
GO
INSERT [dbo].[SystemConfigType] ([SystemConfigTypeId], [ValueType]) VALUES (2, N'TEXT')
GO
SET IDENTITY_INSERT [dbo].[SystemToken] ON 
GO
INSERT [dbo].[SystemToken] ([Id], [TokenId], [SystemUserId], [ClientId], [Subject], [IssuedUtc], [ExpiresUtc], [ProtectedTicket], [TokenType]) VALUES (1, N'vD9Elpvx8XhJrXlZhTA3sJT6wdp2cqZiPzX0+pX8z+w=', N'SU-0000000001', N'http://www.POSWeblandingpage.somee.com', N'admin', CAST(N'2022-06-09T22:16:26.633' AS DateTime), CAST(N'2022-06-10T22:16:26.633' AS DateTime), N'IAxGjRnztbfH3ZKFRVtDy8SD857jdemDgWigG66U2wffdFS1qGsPaKqBvYqCFY1p0Alq7qt7MrrNCZXyf07FLxbidc7tlmV-FilQx94wA8SjS37oARg1o5THPKKmUu81sEdQiumAspdeiNI0rP3UxywkOGw-q-tdaJ8btGdo9Edc05hEUKJhrjed7Z6UoboNtmzXLzxQNhzZ7aNQw_88CgpCXqJ9QolY4SXA3h6kjnbR8xNqP__bqICloyZU1OIkuNouD02Lzb8LXCzCF7gZi0RO2erZV3gk_VH5nxavGsA', N'REFRESH')
GO
SET IDENTITY_INSERT [dbo].[SystemToken] OFF
GO
INSERT [dbo].[SystemUser] ([SystemUserId], [SystemUserTypeId], [LegalEntityId], [ProfilePictureFile], [UserName], [Password], [RefreshToken], [IsWebAdminGuestUser], [HasFirstLogin], [LasteDateTimeActive], [LasteDateTimeLogin], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SU-0000000001', 1, N'LE-0000000001', N'F-0000000002', N'admin', N'ꈙ䆅뙄輺᝶ĥኛ', NULL, 0, 0, NULL, NULL, N'SU-0000000001', CAST(N'2022-06-09T22:02:33.060' AS DateTime), N'SU-0000000001', CAST(N'2022-06-09T22:15:10.800' AS DateTime), 1)
GO
INSERT [dbo].[SystemUserConfig] ([SystemUserConfigId], [SystemUserId], [IsUserEnable]) VALUES (N'SUC-0000000001', N'SU-0000000001', 1)
GO
INSERT [dbo].[SystemUserType] ([SystemUserTypeId], [SystemUserTypeName]) VALUES (1, N'WebAppAdmin')
GO
INSERT [dbo].[SystemUserType] ([SystemUserTypeId], [SystemUserTypeName]) VALUES (2, N'MobileAppUser')
GO
SET IDENTITY_INSERT [dbo].[SystemUserVerification] ON 
GO
INSERT [dbo].[SystemUserVerification] ([Id], [VerificationSender], [VerificationTypeId], [VerificationCode], [IsVerified], [EntityStatusId]) VALUES (1, N'admin@admin.com', 2, N'99740', 1, 1)
GO
SET IDENTITY_INSERT [dbo].[SystemUserVerification] OFF
GO
INSERT [dbo].[SystemUserVerificationType] ([VerificationTypeId], [VerificationTypeName]) VALUES (1, N'MobileNumber')
GO
INSERT [dbo].[SystemUserVerificationType] ([VerificationTypeId], [VerificationTypeName]) VALUES (2, N'Email')
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (1, 1, N'Dashboard', 1)
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (2, 3, N'System Configuration', 2)
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (3, 2, N'System Role', 1)
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (4, 2, N'System User', 1)
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (5, 2, N'System Menu Roles', 1)
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (6, 2, N'System Role Privileges', 1)
GO
INSERT [dbo].[SystemWebAdminMenuRole] ([SystemWebAdminMenuRoleId], [SystemWebAdminMenuId], [SystemWebAdminRoleId], [IsAllowed], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SWAMR-0000000001', N'1', N'SWAR-0000000001', 1, N'SU-0000000001', CAST(N'2022-06-09T22:07:10.667' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[SystemWebAdminMenuRole] ([SystemWebAdminMenuRoleId], [SystemWebAdminMenuId], [SystemWebAdminRoleId], [IsAllowed], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SWAMR-0000000002', N'2', N'SWAR-0000000001', 1, N'SU-0000000001', CAST(N'2022-06-09T22:07:10.667' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[SystemWebAdminMenuRole] ([SystemWebAdminMenuRoleId], [SystemWebAdminMenuId], [SystemWebAdminRoleId], [IsAllowed], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SWAMR-0000000003', N'3', N'SWAR-0000000001', 1, N'SU-0000000001', CAST(N'2022-06-09T22:07:10.670' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[SystemWebAdminMenuRole] ([SystemWebAdminMenuRoleId], [SystemWebAdminMenuId], [SystemWebAdminRoleId], [IsAllowed], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SWAMR-0000000004', N'4', N'SWAR-0000000001', 1, N'SU-0000000001', CAST(N'2022-06-09T22:07:10.670' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[SystemWebAdminMenuRole] ([SystemWebAdminMenuRoleId], [SystemWebAdminMenuId], [SystemWebAdminRoleId], [IsAllowed], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SWAMR-0000000005', N'5', N'SWAR-0000000001', 1, N'SU-0000000001', CAST(N'2022-06-09T22:07:10.673' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[SystemWebAdminMenuRole] ([SystemWebAdminMenuRoleId], [SystemWebAdminMenuId], [SystemWebAdminRoleId], [IsAllowed], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SWAMR-0000000006', N'6', N'SWAR-0000000001', 1, N'SU-0000000001', CAST(N'2022-06-09T22:07:10.673' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[SystemWebAdminModule] ([SystemWebAdminModuleId], [SystemWebAdminModuleName]) VALUES (1, N'Dashboard')
GO
INSERT [dbo].[SystemWebAdminModule] ([SystemWebAdminModuleId], [SystemWebAdminModuleName]) VALUES (2, N'System Admin Security')
GO
INSERT [dbo].[SystemWebAdminModule] ([SystemWebAdminModuleId], [SystemWebAdminModuleName]) VALUES (3, N'System Setup')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (1, N'Allowed to validate other enforcement''s pending report validation')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (2, N'Allowed to approve crime/incident report')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (3, N'Allowed to decline crime/incident report')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (4, N'Allowed to validate enforcement report validation')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (5, N'Allowed to reject enforcement report validation')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (6, N'Allowed to cancel enforcement report validation')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (7, N'Allowed to add user')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (8, N'Allowed to update user')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (9, N'Allowed to delete user')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (10, N'Allowed to add system web admin role')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (11, N'Allowed to update system web admin role')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (12, N'Allowed to delete system web admin role')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (13, N'Allowed to update system web admin menu role')
GO
INSERT [dbo].[SystemWebAdminRole] ([SystemWebAdminRoleId], [RoleName], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SWAR-0000000001', N'Admin', N'SU-0000000001', CAST(N'2022-06-09T22:06:35.917' AS DateTime), NULL, NULL, 1)
GO
INSERT [dbo].[SystemWebAdminUserRole] ([SystemWebAdminUserRoleId], [SystemUserId], [SystemWebAdminRoleId], [CreatedBy], [CreatedAt], [LastUpdatedBy], [LastUpdatedAt], [EntityStatusId]) VALUES (N'SWAUR-0000000001', N'SU-0000000001', N'SWAR-0000000001', N'2022-06-09', CAST(N'2022-06-09T22:09:42.443' AS DateTime), NULL, NULL, 1)
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [U_Sequence]    Script Date: 6/9/2022 10:19:25 PM ******/
ALTER TABLE [dbo].[Sequence] ADD  CONSTRAINT [U_Sequence] UNIQUE NONCLUSTERED 
(
	[TableName] ASC,
	[Prefix] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemUser]    Script Date: 6/9/2022 10:19:25 PM ******/
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [UK_SystemUser] UNIQUE NONCLUSTERED 
(
	[UserName] ASC,
	[EntityStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemWebAdminMenu]    Script Date: 6/9/2022 10:19:25 PM ******/
ALTER TABLE [dbo].[SystemWebAdminMenu] ADD  CONSTRAINT [UK_SystemWebAdminMenu] UNIQUE NONCLUSTERED 
(
	[SystemWebAdminModuleId] ASC,
	[SystemWebAdminMenuName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemWebAdminMenuRole]    Script Date: 6/9/2022 10:19:25 PM ******/
ALTER TABLE [dbo].[SystemWebAdminMenuRole] ADD  CONSTRAINT [UK_SystemWebAdminMenuRole] UNIQUE NONCLUSTERED 
(
	[SystemWebAdminMenuId] ASC,
	[SystemWebAdminRoleId] ASC,
	[IsAllowed] ASC,
	[EntityStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemWebAdminModule]    Script Date: 6/9/2022 10:19:25 PM ******/
ALTER TABLE [dbo].[SystemWebAdminModule] ADD  CONSTRAINT [UK_SystemWebAdminModule] UNIQUE NONCLUSTERED 
(
	[SystemWebAdminModuleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemRole]    Script Date: 6/9/2022 10:19:25 PM ******/
ALTER TABLE [dbo].[SystemWebAdminRole] ADD  CONSTRAINT [UK_SystemRole] UNIQUE NONCLUSTERED 
(
	[RoleName] ASC,
	[EntityStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_FileSize]  DEFAULT ((0)) FOR [FileSize]
GO
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_IsFromStorage]  DEFAULT ((0)) FOR [IsFromStorage]
GO
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[LegalEntity] ADD  CONSTRAINT [DF_LegalEntity_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[LegalEntityAddress] ADD  CONSTRAINT [DF_LegalGeoAddress_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[Sequence] ADD  CONSTRAINT [DF_Sequence_SequenceLength]  DEFAULT ((0)) FOR [Length]
GO
ALTER TABLE [dbo].[Sequence] ADD  CONSTRAINT [DF_Sequence_LastNumber]  DEFAULT ((0)) FOR [LastNumber]
GO
ALTER TABLE [dbo].[SystemConfig] ADD  CONSTRAINT [DF_SystemConfig_IsUserConfigurable]  DEFAULT ((1)) FOR [IsUserConfigurable]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_IsWebAdminGuestUser]  DEFAULT ((0)) FOR [IsWebAdminGuestUser]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_HasFirstLogin]  DEFAULT ((0)) FOR [HasFirstLogin]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[SystemUserConfig] ADD  CONSTRAINT [DF_SystemUserConfig_IsUserEnable]  DEFAULT ((1)) FOR [IsUserEnable]
GO
ALTER TABLE [dbo].[SystemUserContact] ADD  CONSTRAINT [DF_SystemUserContact_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[SystemUserVerification] ADD  CONSTRAINT [DF_SystemUserVerification_IsVerifed]  DEFAULT ((0)) FOR [IsVerified]
GO
ALTER TABLE [dbo].[SystemWebAdminMenu] ADD  CONSTRAINT [DF_SystemWebAdminMenu_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[SystemWebAdminMenuRole] ADD  CONSTRAINT [DF_SystemWebAdminMenuRole_IsAllowed]  DEFAULT ((0)) FOR [IsAllowed]
GO
ALTER TABLE [dbo].[SystemWebAdminMenuRole] ADD  CONSTRAINT [DF_SystemWebAdminMenuRole_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SystemWebAdminMenuRole] ADD  CONSTRAINT [DF_SystemWebAdminMenuRole_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[SystemWebAdminRole] ADD  CONSTRAINT [DF_Role_IsPostedToMain]  DEFAULT ((0)) FOR [CreatedBy]
GO
ALTER TABLE [dbo].[SystemWebAdminRole] ADD  CONSTRAINT [DF_Role_IsFromMain]  DEFAULT ((0)) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SystemWebAdminRole] ADD  CONSTRAINT [DF_SystemRole_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[SystemWebAdminRolePrivileges] ADD  CONSTRAINT [DF_SystemWebAdminRolePrivileges_IsAllowed]  DEFAULT ((0)) FOR [IsAllowed]
GO
ALTER TABLE [dbo].[SystemWebAdminRolePrivileges] ADD  CONSTRAINT [DF_SystemWebAdminRolePrivileges_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[SystemWebAdminUserRole] ADD  CONSTRAINT [DF_SystemUserRoles_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SystemWebAdminUserRole] ADD  CONSTRAINT [DF_SystemUserRoles_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[SystemUser]  WITH CHECK ADD  CONSTRAINT [FK_SystemUser_LegalEntity] FOREIGN KEY([LegalEntityId])
REFERENCES [dbo].[LegalEntity] ([LegalEntityId])
GO
ALTER TABLE [dbo].[SystemUser] CHECK CONSTRAINT [FK_SystemUser_LegalEntity]
GO
/****** Object:  StoredProcedure [dbo].[usp_file_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_file_add]
	@FileName			NVARCHAR(250),
	@MimeType			NVARCHAR(100),
	@FileContent		VARBINARY(MAX),
	@IsFromStorage		BIT,
	@CreatedBy			NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @FileId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'File', @Id = @FileId OUTPUT

		INSERT INTO [dbo].[File](
			FileId, 
			[FileName], 
			[MimeType],
			[FileContent],
			[IsFromStorage],
			[CreatedBy], 
			[CreatedAt],
			[EntityStatusId]
		)
		VALUES(
			@FileId,
			@FileName, 
			@MimeType,
			@FileContent,
			@IsFromStorage,
			@CreatedBy, 
			GETDATE(),
			1
		);

		SELECT @FileId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_file_getById]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_file_getById]
	@FileId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @CreatedBy						NVARCHAR(100);
		DECLARE @CreatedAt						DATETIME;
		DECLARE @LastUpdatedBy					NVARCHAR(100);
		DECLARE @LastUpdatedAt					DATETIME;
		DECLARE @EntityStatusId					BIGINT;
		SELECT 
		@CreatedBy = f.[CreatedBy],
		@CreatedAt = f.[CreatedAt],
		@LastUpdatedBy = f.[LastUpdatedBy],
		@LastUpdatedAt = f.[LastUpdatedAt],
		@EntityStatusId = f.[EntityStatusId]
		FROM [dbo].[File] f
		WHERE f.[FileId] = @FileId
		AND f.[EntityStatusId] = 1;

		SELECT * FROM [dbo].[File] WHERE [FileId] = @FileId
		AND [EntityStatusId] = 1;

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_file_update]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_file_update]
	@FileId				varchar(100),
	@FileName			NVARCHAR(250),
	@MimeType			NVARCHAR(100),
	@FileContent		VARBINARY(MAX),
	@IsFromStorage		BIT,
	@LastUpdatedBy		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
	
		UPDATE [dbo].[File] SET 
		[FileName] = @FileName, 
		[MimeType] = @MimeType, 
		[FileContent] = @FileContent, 
		[IsFromStorage] = @IsFromStorage, 
		[LastUpdatedBy] = @LastUpdatedBy,
		[LastUpdatedAt] = GETDATE() 
		WHERE FileId = @FileId;
		SELECT @@ROWCOUNT;
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_legalentity_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_legalentity_add]
	@FirstName			NVARCHAR(100) = '',
	@LastName			NVARCHAR(100) = '',
	@MiddleName			NVARCHAR(100) = '',
	@GenderId			NVARCHAR(100) = 1,
	@BirthDate			DATE = GETDATE,
	@EmailAddress		NVARCHAR(100),
	@MobileNumber		BIGINT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @LegalEntityId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'LegalEntity', @Id = @LegalEntityId OUTPUT

		INSERT INTO [dbo].[LegalEntity](
			[LegalEntityId], 
			[FirstName], 
			[LastName],
			[MiddleName],
			[GenderId],
			[BirthDate],
			[EmailAddress],
			[MobileNumber],
			[EntityStatusId]
		)
		VALUES(
			@LegalEntityId,
			@FirstName,
			@LastName,
			ISNULL(@MiddleName, ''),
			@GenderId,
			@BirthDate,
			@EmailAddress,
			ISNULL(@MobileNumber, 0),
			1
		);

		SELECT @LegalEntityId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_legalentity_update]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_legalentity_update]
	@LegalEntityId		NVARCHAR(100),
	@FirstName			NVARCHAR(100),
	@LastName			NVARCHAR(100),
	@MiddleName			NVARCHAR(100),
	@GenderId			NVARCHAR(100),
	@EmailAddress		NVARCHAR(100),
	@MobileNumber		NVARCHAR(100),
	@BirthDate			DATE
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		UPDATE [dbo].[LegalEntity] SET
			[FirstName] = @FirstName, 
			[LastName] = @LastName,
			[MiddleName] = @MiddleName,
			[GenderId] = @GenderId,
			[BirthDate] = @BirthDate,
			[EmailAddress] = @EmailAddress,
			[MobileNumber] = @MobileNumber
		WHERE [LegalEntityId] = @LegalEntityId;
		
		SELECT 1;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_legalentityaddress_add]
	@LegalEntityId		NVARCHAR(100),
	@Address			NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @LegalEntityAddressId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'LegalEntityAddress', @Id = @LegalEntityAddressId OUTPUT

		INSERT INTO [dbo].[LegalEntityAddress](
			[LegalEntityAddressId],
			[LegalEntityId],
			[Address],
			[EntityStatusId]
		)
		VALUES(
			@LegalEntityAddressId,
			@LegalEntityId,
			@Address,
			1
		);

		SELECT @LegalEntityAddressId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_delete]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_legalentityaddress_delete]
	@LegalEntityAddressId		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		IF EXISTS(SELECT * FROM [dbo].[LegalEntityAddress] WHERE [LegalEntityAddressId] = @LegalEntityAddressId AND EntityStatusId = 1)
		BEGIN
			-- UPDATE HERE
			UPDATE [dbo].[LegalEntityAddress]
			SET 
			[Address] = @LegalEntityAddressId + ' - ' + [Address] + '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			EntityStatusId = 2
			WHERE [LegalEntityAddressId] = @LegalEntityAddressId;

			SELECT @@ROWCOUNT;
		END
		ELSE
		BEGIN
			SELECT 1;
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_getById]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_legalentityaddress_getById]
	@LegalEntityAddressId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @LegalEntityId	NVARCHAR(100);
		DECLARE @EntityStatusId	BIGINT;
		
		SELECT 
		@LegalEntityId = lea.[LegalEntityId],
		@EntityStatusId = lea.[EntityStatusId]
		FROM [dbo].[LegalEntityAddress] lea
		WHERE lea.[LegalEntityAddressId] = @LegalEntityAddressId
		AND lea.[EntityStatusId] = 1;
		
		SELECT  *
		FROM [dbo].[LegalEntityAddress] AS lea
		WHERE lea.[LegalEntityAddressId] = @LegalEntityAddressId
		AND lea.[EntityStatusId] = 1;
		
		SELECT le.*
		FROM [dbo].[LegalEntity] le
		WHERE le.[LegalEntityId] = @LegalEntityId
		AND le.[EntityStatusId] = 1;

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_getByLegalEntityId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_legalentityaddress_getByLegalEntityId]
	@LegalEntityId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		SELECT 
		lea.[LegalEntityAddressId],
		lea.[Address],
		lea.[EntityStatusId],
		le.*
		FROM [dbo].[LegalEntity] le
		LEFT JOIN [dbo].[LegalEntityAddress] lea ON le.[LegalEntityId] = lea.[LegalEntityId]
		WHERE le.LegalEntityId = @LegalEntityId
		AND lea.[EntityStatusId] = 1;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_getBySystemUserId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_legalentityaddress_getBySystemUserId]
	@SystemUserId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		SELECT 
		lea.[LegalEntityAddressId],
		lea.[Address],
		lea.[EntityStatusId],
		le.*
		FROM [dbo].[SystemUser] su
		LEFT JOIN [dbo].[LegalEntity] le ON su.[LegalEntityId] = le.[LegalEntityId]
		LEFT JOIN [dbo].[LegalEntityAddress] lea ON le.[LegalEntityId] = lea.[LegalEntityId]
		WHERE su.[SystemUserId] = @SystemUserId
		AND lea.[EntityStatusId] = 1;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_update]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_legalentityaddress_update]
	@LegalEntityAddressId	NVARCHAR(100),
	@Address				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		-- UPDATE HERE
		UPDATE [dbo].[LegalEntityAddress]
		SET 
		[Address] = @Address
		WHERE [LegalEntityAddressId] = @LegalEntityAddressId;

		SELECT @@ROWCOUNT;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_lookuptable_getByTableNames]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_lookuptable_getByTableNames]
	@TableNames NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		SELECT 'EntityGender' AS LookupName,CAST([GenderId] AS nvarchar(100)) AS Id,[GenderName] AS Name FROM [dbo].[EntityGender]
		WHERE 'EntityGender' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )

		UNION ALL

		SELECT 'EntityStatus' AS LookupName,CAST([EntityStatusId] AS nvarchar(100)) AS Id,[EntityStatusName] AS Name FROM [dbo].[EntityStatus]
		WHERE 'EntityStatus' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )

		UNION ALL

		SELECT 'SystemConfigType' AS LookupName,CAST([SystemConfigTypeId] AS nvarchar(100)) AS Id,[ValueType] AS Name FROM [dbo].[SystemConfigType]
		WHERE 'SystemConfigType' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )

		UNION ALL

		SELECT 'SystemConfig' AS LookupName,CAST([ConfigKey] AS nvarchar(100)) AS Id,[ConfigValue] FROM [dbo].[SystemConfig]
		WHERE 'SystemConfig' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )

		UNION ALL

		SELECT 'SystemUserType' AS LookupName,CAST([SystemUserTypeId] AS nvarchar(100)) AS Id,[SystemUserTypeName] AS Name FROM [dbo].[SystemUserType]
		WHERE 'SystemUserType' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )

		UNION ALL

		SELECT 'SystemWebAdminMenu' AS LookupName,CAST([SystemWebAdminMenuId] AS nvarchar(100)) AS Id,[SystemWebAdminMenuName] AS Name FROM [dbo].[SystemWebAdminMenu]
		WHERE 'SystemWebAdminMenu' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )

		UNION ALL

		SELECT 'SystemWebAdminModule' AS LookupName,CAST([SystemWebAdminModuleId] AS nvarchar(100)) AS Id,[SystemWebAdminModuleName] AS Name FROM [dbo].[SystemWebAdminModule]
		WHERE 'SystemWebAdminModule' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )
		
		UNION ALL

		SELECT 'SystemWebAdminRole' AS LookupName,[SystemWebAdminRoleId] AS Id,[RoleName] AS Name FROM [dbo].[SystemWebAdminRole]
		WHERE [EntityStatusId] = 1 AND 'SystemWebAdminRole' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_Reset]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_Reset]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
		SET NOCOUNT ON;
		
		DELETE FROM [dbo].[SystemWebAdminUserRole];
		DELETE FROM [dbo].[SystemWebAdminRolePrivileges];
		DELETE FROM [dbo].[SystemWebAdminMenuRole];
		DELETE FROM [dbo].[SystemUserContact];
		DELETE FROM [dbo].[SystemUserConfig];
		DELETE FROM [dbo].[SystemWebAdminRole];
		DELETE FROM [dbo].[SystemUser];
		DELETE FROM [dbo].[LegalEntityAddress];
		DELETE FROM [dbo].[LegalEntity];
		DELETE FROM [dbo].[File];
		DELETE FROM [dbo].[SystemToken];
		DBCC CHECKIDENT ('SystemToken', RESEED, 0)
		DELETE FROM [dbo].[SystemUserVerification];
		DBCC CHECKIDENT ('SystemUserVerification', RESEED, 0)
		
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'SystemWebAdminRolePrivileges';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'SystemWebAdminUserRole';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'SystemWebAdminMenuRole';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'SystemUserContact';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'SystemUserConfig';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'SystemWebAdminRole';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'SystemUser';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'LegalEntityAddress';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'LegalEntity';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'File';
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_Reset_Activity]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_Reset_Activity]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
		SET NOCOUNT ON;
		
		--DECLARE @MediaFileTable TABLE
		--(
		--	FileId NVARCHAR(100)
		--)
		
		--INSERT INTO @MediaFileTable (FileId)
		--SELECT FileId FROM [dbo].[CrimeIncidentReportMedia];

		--DELETE FROM [dbo].[CrimeIncidentReportMedia];
		--DELETE FROM [dbo].[File] WHERE FileId IN (SELECT FileId FROM @MediaFileTable);
		
		--Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'CrimeIncidentReportMedia';
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_sequence_getNextCode]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_sequence_getNextCode]
	@TableName	NVARCHAR(250),
    @Id NVARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @LastNumber INT;
		DECLARE @SequenceLength BIGINT;
		DECLARE @GeneratedId NVARCHAR(100);
		DECLARE @Prefix NVARCHAR(100);
		SELECT @LastNumber = [LastNumber] + 1,@SequenceLength=[Length],@Prefix=[Prefix] FROM [Sequence] WHERE TableName = @TableName;
		SELECT @GeneratedId = CONCAT(@Prefix, [dbo].LPAD(@LastNumber, @SequenceLength, 0));
		SELECT @Id = IIF(datalength(@GeneratedId)=0,NULL,@GeneratedId);

		UPDATE [dbo].[Sequence] SET [LastNumber] = @LastNumber WHERE [TableName] = @TableName;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemtoken_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemtoken_add]
	@TokenId			NVARCHAR(MAX),
	@SystemUserId		NVARCHAR(100),
	@ClientId			NVARCHAR(100),
	@Subject			NVARCHAR(100),
	@IssuedUtc			DATETIME,
	@ExpiresUtc			DATETIME,
	@ProtectedTicket	NVARCHAR(MAX),
	@TokenType			NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		IF(EXISTS(SELECT * FROM [dbo].[SystemToken] WHERE [SystemUserId] = @SystemUserId))
		BEGIN
			UPDATE [dbo].[SystemToken] SET 
			[TokenId]=@TokenId,
			[ClientId]=@ClientId,
			[Subject]=@Subject,
			[IssuedUtc]=@IssuedUtc,
			[ExpiresUtc]=@ExpiresUtc,
			[ProtectedTicket]=@ProtectedTicket,
			[TokenType]=@TokenType
			WHERE [SystemUserId] = @SystemUserId;
			SELECT 1;
		END
		ELSE
		BEGIN	
			INSERT INTO [dbo].[SystemToken](
				[TokenId], 
				[SystemUserId], 
				[ClientId],
				[Subject],
				[IssuedUtc],
				[ExpiresUtc],
				[ProtectedTicket],
				[TokenType]
			)
			VALUES(
				@TokenId,
				@SystemUserId,
				@ClientId,
				@Subject,
				@IssuedUtc,
				@ExpiresUtc,
				@ProtectedTicket,
				@TokenType
			);
			SELECT SCOPE_IDENTITY();
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemtoken_getById]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemtoken_getById]
	@HashedTokenId	NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		DECLARE @SystemUserId NVARCHAR(100);
		
		SELECT 
		@SystemUserId = st.[SystemUserId]
		FROM [dbo].[SystemToken] st
		WHERE st.[TokenId] = @HashedTokenId
		
		SELECT  *
		FROM [dbo].[SystemToken] AS st
		WHERE st.[TokenId] = @HashedTokenId;

		exec usp_systemuser_getByID @SystemUserId

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_add]
	@SystemUserTypeId		NVARCHAR(100),
	@LegalEntityId			NVARCHAR(100),
	@ProfilePictureFile		NVARCHAR(100),
	@UserName				NVARCHAR(100),
	@Password				NVARCHAR(100),
	@CreatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @SystemUserId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'SystemUser', @Id = @SystemUserId OUTPUT

		INSERT INTO [dbo].[SystemUser](
			[SystemUserId], 
			[SystemUserTypeId],
			[LegalEntityId],
			[ProfilePictureFile],
			[UserName],
			[Password],
			[CreatedBy],
			[CreatedAt],
			[EntityStatusId]
		)
		VALUES(
			@SystemUserId,
			@SystemUserTypeId,
			@LegalEntityId,
			@ProfilePictureFile,
			@UserName,
			HashBytes('MD5', @Password),
			@CreatedBy,
			GETDATE(),
			1
		);

		SELECT @SystemUserId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_changePassword]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_changePassword]
	@SystemUserId	NVARCHAR(100),
	@Password		NVARCHAR(100),
	@LastUpdatedBy	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		-- UPDATE HERE
		UPDATE [dbo].[SystemUser]
		SET 
		[Password] = HashBytes('MD5', @Password),
		[LastUpdatedBy] = @LastUpdatedBy,
		[LastUpdatedAt] = GETDATE()
		WHERE [SystemUserId] = @SystemUserId;

		SELECT @@ROWCOUNT;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_changeUsername]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_changeUsername]
	@SystemUserId	NVARCHAR(100),
	@UserName		NVARCHAR(100),
	@LastUpdatedBy	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		-- UPDATE HERE
		UPDATE [dbo].[SystemUser]
		SET 
		[UserName] = @UserName,
		[LastUpdatedBy] = @LastUpdatedBy,
		[LastUpdatedAt] = GETDATE()
		WHERE [SystemUserId] = @SystemUserId;

		SELECT @@ROWCOUNT;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_createAccount]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_createAccount]
	@SystemUserTypeId		NVARCHAR(100),
	@LegalEntityId			NVARCHAR(100),
	@ProfilePictureFile		NVARCHAR(100),
	@UserName				NVARCHAR(100),
	@Password				NVARCHAR(100),
	@IsWebAdminGuestUser	BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @SystemUserId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'SystemUser', @Id = @SystemUserId OUTPUT

		INSERT INTO [dbo].[SystemUser](
			[SystemUserId], 
			[SystemUserTypeId],
			[LegalEntityId],
			[ProfilePictureFile],
			[UserName],
			[Password],
			[IsWebAdminGuestUser],
			[CreatedBy],
			[CreatedAt],
			[EntityStatusId]
		)
		VALUES(
			@SystemUserId,
			@SystemUserTypeId,
			@LegalEntityId,
			@ProfilePictureFile,
			@UserName,
			HashBytes('MD5', @Password),
			@IsWebAdminGuestUser,
			@SystemUserId,
			GETDATE(),
			1
		);

		SELECT @SystemUserId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_delete]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_delete]
	@SystemUserId		NVARCHAR(100),
	@LastUpdatedBy		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		IF EXISTS(SELECT * FROM [dbo].[SystemUser] WHERE [SystemUserId] = @SystemUserId AND EntityStatusId = 1)
		BEGIN
			-- UPDATE HERE
			UPDATE [dbo].[SystemUser]
			SET 
			[UserName] = @SystemUserId + ' - ' + [UserName] + '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			EntityStatusId = 2,
			[LastUpdatedBy] = @LastUpdatedBy,
			[LastUpdatedAt] = GETDATE()
			WHERE [SystemUserId] = @SystemUserId;

			SELECT @@ROWCOUNT;
		END
		ELSE
		BEGIN
			SELECT 1;
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getByCredentials]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_getByCredentials]
	@Username	NVARCHAR(100),
	@Password	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @SystemUserId			NVARCHAR(100);
		DECLARE @SystemUserTypeId		BIGINT;
		DECLARE @LegalEntityId			NVARCHAR(100);
		DECLARE @ProfilePictureFile		NVARCHAR(100);
		DECLARE @GenderId				BIGINT;
		DECLARE @CreatedBy				NVARCHAR(100);
		DECLARE @CreatedAt				DATETIME;
		DECLARE @LastUpdatedBy			NVARCHAR(100);
		DECLARE @LastUpdatedAt			DATETIME;
		DECLARE @EntityStatusId			BIGINT;
		
		SELECT 
		@SystemUserId = su.[SystemUserId], 
		@SystemUserTypeId = su.[SystemUserTypeId],
		@ProfilePictureFile = su.[ProfilePictureFile],
		@LegalEntityId = su.[LegalEntityId],
		@ProfilePictureFile = su.[ProfilePictureFile],
		@CreatedBy = su.[CreatedBy],
		@CreatedAt = su.[CreatedAt],
		@LastUpdatedBy = su.[LastUpdatedBy],
		@LastUpdatedAt = su.[LastUpdatedAt]
		FROM [dbo].[SystemUser] su
		WHERE su.[UserName] = @Username 
		AND su.[Password] = HashBytes('MD5', @Password)
		AND su.[EntityStatusId] = 1;
		
		SELECT  *
		FROM [dbo].[SystemUser] AS su
		WHERE su.[SystemUserId] = @SystemUserId
		AND su.[EntityStatusId] = 1;

		SELECT  
		sut.[SystemUserTypeId],
		sut.[SystemUserTypeName]
		FROM [dbo].[SystemUserType] AS sut
		WHERE sut.[SystemUserTypeId] = @SystemUserTypeId;
		
		SELECT  *
		FROM [dbo].[File] AS f
		WHERE f.[FileId] = @ProfilePictureFile
		AND f.[EntityStatusId] = 1;

		SELECT  
		@GenderId = le.[GenderId]
		FROM [dbo].[LegalEntity] AS le
		WHERE le.[LegalEntityId] = @LegalEntityId;

		SELECT  
		le.*
		FROM [dbo].[LegalEntity] AS le
		WHERE le.[LegalEntityId] = @LegalEntityId;

		SELECT  
		eg.[GenderId],
		eg.[GenderName]
		FROM [dbo].[EntityGender] AS eg
		WHERE eg.[GenderId] = @GenderId;

		exec [dbo].[usp_systemuserconfig_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminuserroles_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminmenuroles_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminroleprivileges_getBySystemUserId] @SystemUserId

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getByID]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_getByID]
	@SystemUserId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @SystemUserTypeId		BIGINT;
		DECLARE @LegalEntityId			NVARCHAR(100);
		DECLARE @ProfilePictureFile		NVARCHAR(100);
		DECLARE @GenderId				BIGINT;
		DECLARE @CreatedBy				NVARCHAR(100);
		DECLARE @CreatedAt				DATETIME;
		DECLARE @LastUpdatedBy			NVARCHAR(100);
		DECLARE @LastUpdatedAt			DATETIME;
		DECLARE @EntityStatusId			BIGINT;
		
		SELECT 
		@SystemUserTypeId = su.[SystemUserTypeId],
		@LegalEntityId = su.[LegalEntityId],
		@ProfilePictureFile = su.[ProfilePictureFile],
		@CreatedBy = su.[CreatedBy],
		@CreatedAt = su.[CreatedAt],
		@LastUpdatedBy = su.[LastUpdatedBy],
		@LastUpdatedAt = su.[LastUpdatedAt],
		@EntityStatusId = su.[EntityStatusId]
		FROM [dbo].[SystemUser] su
		WHERE su.[SystemUserId] = @SystemUserId
		AND su.[EntityStatusId] = 1;
		
		SELECT  *
		FROM [dbo].[SystemUser] AS su
		WHERE su.[SystemUserId] = @SystemUserId
		AND su.[EntityStatusId] = 1;

		SELECT  
		sut.[SystemUserTypeId],
		sut.[SystemUserTypeName]
		FROM [dbo].[SystemUserType] AS sut
		WHERE sut.[SystemUserTypeId] = @SystemUserTypeId;
		
		SELECT  *
		FROM [dbo].[File] AS f
		WHERE f.[FileId] = @ProfilePictureFile
		AND f.[EntityStatusId] = 1;

		SELECT  
		@GenderId = le.[GenderId]
		FROM [dbo].[LegalEntity] AS le
		WHERE le.[LegalEntityId] = @LegalEntityId;

		SELECT  
		le.*
		FROM [dbo].[LegalEntity] AS le
		WHERE le.[LegalEntityId] = @LegalEntityId;

		SELECT  
		eg.[GenderId],
		eg.[GenderName]
		FROM [dbo].[EntityGender] AS eg
		WHERE eg.[GenderId] = @GenderId;

		exec [dbo].[usp_systemuserconfig_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminuserroles_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminmenuroles_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminroleprivileges_getBySystemUserId] @SystemUserId

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getByUsername]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_getByUsername]
	@Username	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @SystemUserId			NVARCHAR(100);
		DECLARE @SystemUserTypeId		BIGINT;
		DECLARE @LegalEntityId			NVARCHAR(100);
		DECLARE @ProfilePictureFile		NVARCHAR(100);
		DECLARE @GenderId				BIGINT;
		DECLARE @CreatedBy				NVARCHAR(100);
		DECLARE @CreatedAt				DATETIME;
		DECLARE @LastUpdatedBy			NVARCHAR(100);
		DECLARE @LastUpdatedAt			DATETIME;
		DECLARE @EntityStatusId			BIGINT;
		
		SELECT 
		@SystemUserId = su.[SystemUserId],
		@SystemUserTypeId = su.[SystemUserTypeId],
		@LegalEntityId = su.[LegalEntityId],
		@ProfilePictureFile = su.[ProfilePictureFile],
		@CreatedBy = su.[CreatedBy],
		@CreatedAt = su.[CreatedAt],
		@LastUpdatedBy = su.[LastUpdatedBy],
		@LastUpdatedAt = su.[LastUpdatedAt],
		@EntityStatusId = su.[EntityStatusId]
		FROM [dbo].[SystemUser] su
		WHERE su.[UserName] = @Username
		AND su.[EntityStatusId] = 1;
		
		SELECT  *
		FROM [dbo].[SystemUser] AS su
		WHERE su.[SystemUserId] = @SystemUserId
		AND su.[EntityStatusId] = 1;

		SELECT  
		sut.[SystemUserTypeId],
		sut.[SystemUserTypeName]
		FROM [dbo].[SystemUserType] AS sut
		WHERE sut.[SystemUserTypeId] = @SystemUserTypeId;
		
		SELECT  *
		FROM [dbo].[File] AS f
		WHERE f.[FileId] = @ProfilePictureFile
		AND f.[EntityStatusId] = 1;

		SELECT  
		@GenderId = le.[GenderId]
		FROM [dbo].[LegalEntity] AS le
		WHERE le.[LegalEntityId] = @LegalEntityId;

		SELECT  
		le.*
		FROM [dbo].[LegalEntity] AS le
		WHERE le.[LegalEntityId] = @LegalEntityId;

		SELECT  
		eg.[GenderId],
		eg.[GenderName]
		FROM [dbo].[EntityGender] AS eg
		WHERE eg.[GenderId] = @GenderId;

		exec [dbo].[usp_systemuserconfig_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminuserroles_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminmenuroles_getBySystemUserId] @SystemUserId

		exec [dbo].[usp_systemwebadminroleprivileges_getBySystemUserId] @SystemUserId

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getPaged]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		erwin
-- Create date: 2020-09-16
-- Description:	filter contract list by location
-- =============================================
CREATE PROCEDURE [dbo].[usp_systemuser_getPaged]
	@Search					NVARCHAR(50) = '',
	@SystemUserType			BIGINT = 1,
	@ApprovalStatus			NVARCHAR(50),
	@PageNo					BIGINT = 1,
	@PageSize				BIGINT = 10,
	@OrderColumn			NVARCHAR(100),
	@OrderDir				NVARCHAR(5)
AS
BEGIN
	SET NOCOUNT ON;
		DECLARE @IsWebAdminGuestUser NVARCHAR(10);
		SET @IsWebAdminGuestUser = IIF(CAST(@ApprovalStatus AS NVARCHAR(10)) <> 2, IIF(CAST(@ApprovalStatus AS NVARCHAR(10)) <> 1, 1, 0), NULL);
		WITH DATA_CTE
		AS
		(
			Select tableSource.*,
			(CASE @OrderDir
			 WHEN 'asc' THEN
				CASE @OrderColumn 
					WHEN 'SystemUserId' THEN ROW_NUMBER() OVER(ORDER BY [SystemUserId] ASC)
					WHEN 'UserName' THEN ROW_NUMBER() OVER(ORDER BY [UserName] ASC)
					WHEN 'LegalEntity.FullName' THEN ROW_NUMBER() OVER(ORDER BY [FullName] ASC)
					WHEN 'LegalEntity.Gender.GenderName' THEN ROW_NUMBER() OVER(ORDER BY [GenderName] ASC)
					WHEN 'LegalEntity.Age' THEN ROW_NUMBER() OVER(ORDER BY [Age] ASC)
					WHEN 'LegalEntity.EmailAddress' THEN ROW_NUMBER() OVER(ORDER BY [EmailAddress] ASC)
					WHEN 'LegalEntity.MobileNumber' THEN ROW_NUMBER() OVER(ORDER BY [MobileNumber] ASC)
				END
			WHEN 'desc' THEN
				CASE @OrderColumn 
					WHEN 'SystemUserId' THEN ROW_NUMBER() OVER(ORDER BY [SystemUserId] DESC)
					WHEN 'UserName' THEN ROW_NUMBER() OVER(ORDER BY [UserName] DESC)
					WHEN 'LegalEntity.FullName' THEN ROW_NUMBER() OVER(ORDER BY [FullName] DESC)
					WHEN 'LegalEntity.Gender.GenderName' THEN ROW_NUMBER() OVER(ORDER BY [GenderName] DESC)
					WHEN 'LegalEntity.Age' THEN ROW_NUMBER() OVER(ORDER BY [Age] DESC)
					WHEN 'LegalEntity.EmailAddress' THEN ROW_NUMBER() OVER(ORDER BY [EmailAddress] DESC)
					WHEN 'LegalEntity.MobileNumber' THEN ROW_NUMBER() OVER(ORDER BY [MobileNumber] DESC)
				END
			 END) AS row_num ,
			count(*) over() as TotalRows
			FROM (
			 SELECT 
			 su.[SystemUserId],
			 MAX(su.[UserName])[UserName],
			 MAX(su.[Password])[Password],
			 MAX(IIF(su.[IsWebAdminGuestUser] <> 1, 0, 1))[IsWebAdminGuestUser],
			 MAX(pf.[FileId])[FileId],
			 MAX(pf.[FileName])[FileName],
			 MAX(pf.[MimeType])[MimeType],
			 MAX(pf.[FileSize])[FileSize],
			 MAX(pf.[FileContent])[FileContent],
			 MAX(IIF(pf.[IsFromStorage] <> 1, 0, 1))[IsFromStorage],
			 MAX(sut.[SystemUserTypeId])[SystemUserTypeId],
			 MAX(sut.[SystemUserTypeName])[SystemUserTypeName],
			 MAX(le.[LegalEntityId])[LegalEntityId],
			 MAX(le.[FullName])[FullName],
			 MAX(le.[BirthDate])[BirthDate],
			 MAX(le.[Age])[Age],
			 MAX(le.[EmailAddress])[EmailAddress],
			 MAX(le.[MobileNumber])[MobileNumber],
			 MAX(eg.[GenderId])[GenderId],
			 MAX(eg.[GenderName])[GenderName]
			FROM [dbo].[SystemUser] AS su
			LEFT JOIN (SELECT * FROM [dbo].[SystemWebAdminUserRole] WHERE [EntityStatusId] = 1) sur ON su.SystemUserId = sur.SystemUserId
			LEFT JOIN (SELECT * FROM [dbo].[SystemWebAdminRole] WHERE [EntityStatusId] = 1) sr ON sur.[SystemWebAdminRoleId] = sr.[SystemWebAdminRoleId]
			LEFT JOIN (SELECT *,ISNULL([FirstName],'') + ' ' + ISNULL([MiddleName],'') + ' ' + ISNULL([LastName],'') AS [FullName] FROM [dbo].[LegalEntity] WHERE EntityStatusId = 1) AS le ON su.LegalEntityId = le.LegalEntityId
			LEFT JOIN [dbo].[File] AS pf ON su.ProfilePictureFile = pf.FileId
			LEFT JOIN [dbo].[SystemUserType] AS sut ON su.SystemUserTypeId = sut.SystemUserTypeId
			LEFT JOIN [dbo].[EntityGender] AS eg ON le.GenderId = eg.GenderId
			WHERE su.EntityStatusId = 1
			AND su.SystemUserTypeId = @SystemUserType
			--AND su.IsWebAdminGuestUser LIKE (
			--								CASE 
			--									WHEN @ApprovalStatus = 0 THEN 1 
			--									WHEN @ApprovalStatus = 1 THEN 0 
			--									WHEN @ApprovalStatus = 2 THEN '%%'
			--								END
			--								)
			AND su.IsWebAdminGuestUser like '%'+ ISNULL(@IsWebAdminGuestUser, '') +'%'
			AND (su.SystemUserId like '%' + @Search + '%' 
			OR su.[UserName] like '%' + @Search + '%' 
			OR sr.[RoleName] like '%' + @Search + '%' 
			OR le.[FullName] like '%' + @Search + '%' 
			OR eg.[GenderName] like '%' + @Search + '%'
			OR le.[Age] like '%' + @Search + '%' 
			OR le.[EmailAddress] like '%' + @Search + '%'
			OR le.[MobileNumber] like '%' + @Search + '%' )
			GROUP BY su.SystemUserId
			) tableSource
		)
		SELECT 
		src.*,
		sur.[SystemWebAdminUserRoleId],
		sr.[SystemWebAdminRoleId],
		sr.[RoleName],
		src.row_num,
		src.TotalRows
		FROM DATA_CTE src
		 
		LEFT JOIN (SELECT * FROM [dbo].[SystemWebAdminUserRole] WHERE [EntityStatusId] = 1) sur ON src.SystemUserId = sur.SystemUserId
		LEFT JOIN (SELECT * FROM [dbo].[SystemWebAdminRole] WHERE [EntityStatusId] = 1) sr ON sur.[SystemWebAdminRoleId] = sr.[SystemWebAdminRoleId]
		WHERE src.row_num between ((@PageNo - 1) * @PageSize + 1 ) 
		and (@PageNo * @PageSize)
		ORDER BY src.row_num 

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getTrackerStatus]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_getTrackerStatus]
	@SystemUserId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		SELECT *
		FROM [dbo].[SystemUser] AS su
		WHERE su.[SystemUserId] = @SystemUserId
		AND su.[EntityStatusId] = 1;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuser_update]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuser_update]
	@SystemUserId			NVARCHAR(100),
	@IsWebAdminGuestUser	BIT,
	@ProfilePictureFile		NVARCHAR(100),
	@LastUpdatedBy			NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		-- UPDATE HERE
		UPDATE [dbo].[SystemUser]
		SET 
		[LastUpdatedBy] = @LastUpdatedBy,
		[IsWebAdminGuestUser] = @IsWebAdminGuestUser,
		[ProfilePictureFile] = (CASE WHEN ISNULL(@ProfilePictureFile, '') <> '' THEN @ProfilePictureFile END) ,
		[LastUpdatedAt] = GETDATE()
		WHERE [SystemUserId] = @SystemUserId;

		SELECT @@ROWCOUNT;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuserconfig_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuserconfig_add]
	@SystemUserId					NVARCHAR(100),
	@IsUserEnable					BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @SystemUserConfigId NVARCHAR(100);

		exec [dbo].[usp_sequence_getNextCode] 'SystemUserConfig', @Id = @SystemUserConfigId OUTPUT
		
		INSERT INTO [dbo].[SystemUserConfig](
			[SystemUserConfigId], 
			[SystemUserId], 
			[IsUserEnable]
		)
		VALUES(
			@SystemUserConfigId,
			@SystemUserId,
			@IsUserEnable
		);

		SELECT @SystemUserConfigId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuserconfig_getBySystemUserId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuserconfig_getBySystemUserId]
	@SystemUserId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		SELECT  *
		FROM [dbo].[SystemUserConfig] AS suc
		WHERE suc.[SystemUserId] = @SystemUserId;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuserconfig_update]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuserconfig_update]
	@SystemUserConfigId				NVARCHAR(100),
	@IsUserEnable					BIT,
	@IsUserAllowToPostNextReport	BIT,
	@IsNextReportPublic				BIT,
	@IsAnonymousNextReport			BIT,
	@AllowReviewActionNextPost		BIT,
	@AllowReviewCommentNextPost		BIT,
	@IsAllReportPublic				BIT,
	@IsAnonymousAllReport			BIT,
	@AllowReviewActionAllReport		BIT,
	@AllowReviewCommentAllReport	BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		UPDATE [dbo].[SystemUserConfig]
		SET 
		[IsUserEnable] = @IsUserEnable,
		[IsUserAllowToPostNextReport] = @IsUserAllowToPostNextReport,
		[IsNextReportPublic] = @IsNextReportPublic,
		[IsAnonymousNextReport] = @IsAnonymousNextReport,
		[AllowReviewActionNextPost] = @AllowReviewActionNextPost,
		[AllowReviewCommentNextPost] = @AllowReviewActionNextPost,
		[IsAllReportPublic] = @AllowReviewActionNextPost,
		[IsAnonymousAllReport] = @AllowReviewActionNextPost,
		[AllowReviewActionAllReport] = @AllowReviewActionNextPost,
		[AllowReviewCommentAllReport] = @AllowReviewActionNextPost
		WHERE [SystemUserConfigId] = @SystemUserConfigId;

		SELECT @@ROWCOUNT;

		SELECT @SystemUserConfigId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuserverification_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuserverification_add]
	@VerificationSender		NVARCHAR(100),
	@VerificationTypeId		BIGINT,
	@VerificationCode		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		IF EXISTS(SELECT 1 From [SystemUserVerification] WHERE [VerificationSender] = @VerificationSender)
		BEGIN
			DECLARE @id BIGINT;
			SELECT @id = (SELECT [Id] From [SystemUserVerification] WHERE [VerificationSender] = @VerificationSender);
			UPDATE [dbo].[SystemUserVerification] SET 
			[VerificationCode] = @VerificationCode
			WHERE [Id] = @id

			SELECT @id;
			
		END
		ELSE
		BEGIN
			INSERT INTO [dbo].[SystemUserVerification](
				[VerificationSender], 
				[VerificationTypeId], 
				[VerificationCode],
				[EntityStatusId]
			)
			VALUES(
				@VerificationSender,
				@VerificationTypeId,
				@VerificationCode,
				1
			);
		
			SELECT SCOPE_IDENTITY();
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuserverification_getByID]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuserverification_getByID]
	@Id	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		SELECT  *
		FROM [dbo].[SystemUserVerification] AS suv
		WHERE suv.[Id] = @Id
		AND suv.[EntityStatusId] = 1;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuserverification_getBySender]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuserverification_getBySender]
	@VerificationSender	NVARCHAR(100),
	@VerificationCode	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		SELECT  *
		FROM [dbo].[SystemUserVerification] AS suv
		WHERE suv.[VerificationSender] = @VerificationSender
		--AND suv.[VerificationCode] = @VerificationCode
		AND suv.[EntityStatusId] = 1;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemuserverification_verifyUser]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemuserverification_verifyUser]
	@Id		BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		IF EXISTS(SELECT * FROM [dbo].[SystemUserVerification] WHERE [Id] = @Id AND EntityStatusId = 1)
		BEGIN
			-- UPDATE HERE
			UPDATE [dbo].[SystemUserVerification]
			SET 
			[IsVerified] = 1
			WHERE [Id] = @Id;

			SELECT @@ROWCOUNT;
		END
		ELSE
		BEGIN
			SELECT 1;
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenumodule_getBySystemWebAdminMenuId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminmenumodule_getBySystemWebAdminMenuId]
	@SystemWebAdminMenuId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		DECLARE @SystemWebAdminModuleId BIGINT;

		SELECT @SystemWebAdminModuleId = [SystemWebAdminModuleId]
		FROM [dbo].[SystemWebAdminMenu]
		WHERE [SystemWebAdminMenuId] = @SystemWebAdminMenuId;
		
		SELECT [SystemWebAdminModuleId],[SystemWebAdminModuleName]
		FROM [dbo].[SystemWebAdminModule]
		WHERE [SystemWebAdminModuleId] = @SystemWebAdminModuleId;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminmenuroles_add]
	@SystemWebAdminMenuId	NVARCHAR(100),
	@IsAllowed				BIT,
	@SystemWebAdminRoleId	NVARCHAR(100),
	@CreatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @SystemWebAdminMenuRoleId nvarchar(100);
		
		IF EXISTS(SELECT * FROM [dbo].[SystemWebAdminMenuRole] WHERE [SystemWebAdminMenuId] = @SystemWebAdminMenuId AND [SystemWebAdminRoleId] = @SystemWebAdminRoleId)
		BEGIN
			SELECT @SystemWebAdminMenuRoleId = [SystemWebAdminMenuRoleId] FROM [dbo].[SystemWebAdminMenuRole] WHERE [SystemWebAdminMenuId] = @SystemWebAdminMenuId AND [SystemWebAdminRoleId] = @SystemWebAdminRoleId;
			UPDATE [dbo].[SystemWebAdminMenuRole] SET IsAllowed = 1, EntityStatusId = 1 WHERE [SystemWebAdminMenuRoleId] = @SystemWebAdminMenuRoleId;
		END
		ELSE
		BEGIN
		
			exec [dbo].[usp_sequence_getNextCode] 'SystemWebAdminMenuRole', @Id = @SystemWebAdminMenuRoleId OUTPUT

			INSERT INTO [dbo].[SystemWebAdminMenuRole](
				[SystemWebAdminMenuRoleId],
				[SystemWebAdminMenuId],
				[SystemWebAdminRoleId],
				[IsAllowed],
				[CreatedBy],
				[CreatedAt],
				[EntityStatusId]
			)
			VALUES(
				@SystemWebAdminMenuRoleId,
				@SystemWebAdminMenuId,
				@SystemWebAdminRoleId,
				@IsAllowed,
				@CreatedBy,
				GETDATE(),
				1
			);

		END

		SELECT @SystemWebAdminMenuRoleId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_delete]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminmenuroles_delete]
	@SystemWebAdminMenuRoleId	NVARCHAR(100),
	@LastUpdatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		IF EXISTS(SELECT * FROM [dbo].[SystemWebAdminMenuRole] WHERE [SystemWebAdminMenuRoleId] = @SystemWebAdminMenuRoleId AND EntityStatusId = 1)
		BEGIN
			-- UPDATE HERE
			UPDATE [dbo].[SystemWebAdminMenuRole]
			SET 
			EntityStatusId = 2,
			LastUpdatedBy = @LastUpdatedBy,
			LastUpdatedAt = GETDATE()
			WHERE [SystemWebAdminMenuRoleId] = @SystemWebAdminMenuRoleId;

			SELECT @@ROWCOUNT;
		END
		ELSE
		BEGIN
			SELECT 1;
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_getBySystemUserId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminmenuroles_getBySystemUserId]
	@SystemUserId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		SELECT 
		swam.[SystemWebAdminMenuId],
		MAX(swam.[SystemWebAdminMenuName])[SystemWebAdminMenuName],
		MAX(swamod.[SystemWebAdminModuleId])[SystemWebAdminModuleId],
		MAX(swamod.[SystemWebAdminModuleName])[SystemWebAdminModuleName]
		FROM [dbo].[SystemWebAdminUserRole] swaur
		LEFT JOIN [dbo].[SystemWebAdminMenuRole] swamr ON swaur.SystemWebAdminRoleId = swamr.SystemWebAdminRoleId
		LEFT JOIN [dbo].[SystemWebAdminRole] swar ON swamr.[SystemWebAdminRoleId] = swar.[SystemWebAdminRoleId]
		LEFT JOIN [dbo].[SystemWebAdminMenu] swam ON swamr.[SystemWebAdminMenuId] = swam.[SystemWebAdminMenuId]
		LEFT JOIN [dbo].[SystemWebAdminModule] swamod ON swam.[SystemWebAdminModuleId] = swamod.[SystemWebAdminModuleId]
		WHERE swaur.SystemUserId = @SystemUserId
		AND swaur.EntityStatusId = 1
		AND swamr.[EntityStatusId] = 1
		AND swamr.IsAllowed = 1
		GROUP BY swam.SystemWebAdminMenuId

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminMenuIdAndSystemWebAdminRoleId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminMenuIdAndSystemWebAdminRoleId]
	@SystemWebAdminMenuId	BIGINT,
	@SystemWebAdminRoleId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @SystemWebAdminMenuRoleId	NVARCHAR(100);
		DECLARE @CreatedBy			NVARCHAR(100);
		DECLARE @CreatedAt			DATETIME;
		DECLARE @LastUpdatedBy		NVARCHAR(100);
		DECLARE @LastUpdatedAt		DATETIME;
		DECLARE @EntityStatusId		BIGINT;
		
		SELECT 
		@SystemWebAdminMenuRoleId = swamr.[SystemWebAdminMenuRoleId],
		@SystemWebAdminRoleId = swamr.[SystemWebAdminRoleId],
		@SystemWebAdminMenuId = swamr.[SystemWebAdminMenuId],
		@CreatedBy = swamr.[CreatedBy],
		@CreatedAt = swamr.[CreatedAt],
		@LastUpdatedBy = swamr.[LastUpdatedBy],
		@LastUpdatedAt = swamr.[LastUpdatedAt],
		@EntityStatusId = swamr.[EntityStatusId]
		FROM [dbo].[SystemWebAdminMenuRole] swamr
		WHERE swamr.[SystemWebAdminRoleId] = @SystemWebAdminRoleId
		AND swamr.[SystemWebAdminMenuId] = @SystemWebAdminMenuId
		AND swamr.[EntityStatusId] = 1;
		
		SELECT *
		FROM [dbo].[SystemWebAdminMenuRole] swamr
		WHERE swamr.[SystemWebAdminMenuRoleId] = @SystemWebAdminMenuRoleId
		AND swamr.[EntityStatusId] = 1;

		SELECT *
		FROM [dbo].[SystemWebAdminRole] AS swar
		WHERE swar.[SystemWebAdminRoleId] = @SystemWebAdminRoleId;
		
		SELECT *
		FROM [dbo].[SystemWebAdminMenu] AS swam
		WHERE swam.[SystemWebAdminMenuId] = @SystemWebAdminMenuId;

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminRoleId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminRoleId]
	@SystemWebAdminRoleId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		SELECT 
		swamr.SystemWebAdminMenuRoleId,
		swamr.IsAllowed,
		swam.*,
		swar.*
		FROM [dbo].[SystemWebAdminMenuRole] swamr
		LEFT JOIN [dbo].[SystemWebAdminRole] swar ON swamr.[SystemWebAdminRoleId] = swar.[SystemWebAdminRoleId]
		LEFT JOIN [dbo].[SystemWebAdminMenu] swam ON swamr.[SystemWebAdminMenuId] = swam.[SystemWebAdminMenuId]
		WHERE swamr.[SystemWebAdminRoleId] = @SystemWebAdminRoleId
		AND swam.EntityStatusId = 1
		AND swamr.[EntityStatusId] = 1;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminRoleIdandSystemWebAdminModuleId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminRoleIdandSystemWebAdminModuleId]
	@SystemWebAdminRoleId	NVARCHAR(100),
	@SystemWebAdminModuleId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		SELECT 
		MAX(systemWebAdminMenuRole.[SystemWebAdminMenuRoleId])[SystemWebAdminMenuRoleId], 
		MAX(systemWebAdminMenuRole.[IsAllowed])[IsAllowed], 
		systemWebAdminMenuRole.[SystemWebAdminMenuId], 
		MAX(systemWebAdminMenuRole.[SystemWebAdminMenuName])[SystemWebAdminMenuName], 
		MAX(systemWebAdminMenuRole.[SystemWebAdminModuleId])[SystemWebAdminModuleId], 
		MAX(systemWebAdminMenuRole.[SystemWebAdminModuleName])[SystemWebAdminModuleName], 
		MAX(systemWebAdminMenuRole.[SystemWebAdminRoleId])[SystemWebAdminRoleId], 
		MAX(systemWebAdminMenuRole.[RoleName])[RoleName]
		FROM (
			SELECT 
			swamr.[SystemWebAdminMenuRoleId],
			swamr.[IsAllowed],
			swam.[SystemWebAdminMenuId],
			swam.[SystemWebAdminMenuName],
			swamod.[SystemWebAdminModuleId],
			swamod.[SystemWebAdminModuleName],
			swar.[SystemWebAdminRoleId],
			swar.[RoleName]
			FROM [dbo].[SystemWebAdminMenuRole] swamr
			LEFT JOIN [dbo].[SystemWebAdminRole] swar ON swamr.[SystemWebAdminRoleId] = swar.[SystemWebAdminRoleId]
			LEFT JOIN [dbo].[SystemWebAdminMenu] swam ON swamr.[SystemWebAdminMenuId] = swam.[SystemWebAdminMenuId]
			LEFT JOIN [dbo].[SystemWebAdminModule] swamod ON swam.[SystemWebAdminModuleId] = swamod.[SystemWebAdminModuleId]
			WHERE swamr.[SystemWebAdminRoleId] = @SystemWebAdminRoleId
			AND swam.SystemWebAdminModuleId = @SystemWebAdminModuleId
			AND swam.EntityStatusId = 1
			AND swamr.[EntityStatusId] = 1

			UNION ALL

			SELECT 
			CAST(swam.[SystemWebAdminMenuId] AS NVARCHAR(100)) AS [SystemWebAdminMenuRoleId],
			0 [IsAllowed],
			swam.[SystemWebAdminMenuId],
			swam.[SystemWebAdminMenuName],
			swamod.[SystemWebAdminModuleId],
			swamod.[SystemWebAdminModuleName],
			'' [SystemWebAdminRoleId],
			'' [RoleName]
			FROM [dbo].[SystemWebAdminMenu] swam
			LEFT JOIN [dbo].[SystemWebAdminModule] swamod ON swam.[SystemWebAdminModuleId] = swamod.[SystemWebAdminModuleId]
			WHERE swam.SystemWebAdminModuleId = @SystemWebAdminModuleId
			AND swam.EntityStatusId = 1
		) systemWebAdminMenuRole
		GROUP BY systemWebAdminMenuRole.SystemWebAdminMenuId
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_update]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminmenuroles_update]
	@SystemWebAdminMenuRoleId		NVARCHAR(100),
	@IsAllowed						BIT,
	@LastUpdatedBy					NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		-- UPDATE HERE
		UPDATE [dbo].[SystemWebAdminMenuRole]
		SET 
		[IsAllowed] = @IsAllowed,
		[LastUpdatedBy] = @LastUpdatedBy,
		[LastUpdatedAt] = GETDATE()
		WHERE [SystemWebAdminMenuRoleId] = @SystemWebAdminMenuRoleId;

		SELECT @@ROWCOUNT;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminrole_add]
	@RoleName			NVARCHAR(100),
	@CreatedBy			NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @SystemWebAdminRoleId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'SystemWebAdminRole', @Id = @SystemWebAdminRoleId OUTPUT

		INSERT INTO [dbo].[SystemWebAdminRole](
			[SystemWebAdminRoleId],
			[RoleName],
			[CreatedBy],
			[CreatedAt],
			[EntityStatusId]
		)
		VALUES(
			@SystemWebAdminRoleId,
			@RoleName,
			@CreatedBy,
			GETDATE(),
			1
		);

		SELECT @SystemWebAdminRoleId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_delete]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminrole_delete]
	@SystemWebAdminRoleId		NVARCHAR(100),
	@LastUpdatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		IF EXISTS(SELECT * FROM [dbo].[SystemWebAdminRole] WHERE [SystemWebAdminRoleId] = @SystemWebAdminRoleId AND EntityStatusId = 1)
		BEGIN
			-- UPDATE HERE
			UPDATE [dbo].[SystemWebAdminRole]
			SET 
			[RoleName] = @SystemWebAdminRoleId + ' - ' + [RoleName] + '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			EntityStatusId = 2,
			[LastUpdatedBy] = @LastUpdatedBy,
			[LastUpdatedAt] = GETDATE()
			WHERE [SystemWebAdminRoleId] = @SystemWebAdminRoleId;

			SELECT @@ROWCOUNT;
		END
		ELSE
		BEGIN
			SELECT 1;
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_getByID]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminrole_getByID]
	@SystemWebAdminRoleId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @CreatedBy			NVARCHAR(100);
		DECLARE @CreatedAt			DATETIME;
		DECLARE @LastUpdatedBy		NVARCHAR(100);
		DECLARE @LastUpdatedAt		DATETIME;
		DECLARE @EntityStatusId		BIGINT;
		
		SELECT 
		@CreatedBy = swar.[CreatedBy],
		@CreatedAt = swar.[CreatedAt],
		@LastUpdatedBy = swar.[LastUpdatedBy],
		@LastUpdatedAt = swar.[LastUpdatedAt],
		@EntityStatusId = swar.[EntityStatusId]
		FROM [dbo].[SystemWebAdminRole] swar
		WHERE swar.[SystemWebAdminRoleId] = @SystemWebAdminRoleId
		AND swar.[EntityStatusId] = 1;
		
		SELECT  *
		FROM [dbo].[SystemWebAdminRole] AS swar
		WHERE swar.[SystemWebAdminRoleId] = @SystemWebAdminRoleId
		AND swar.[EntityStatusId] = 1;

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_getPaged]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		erwin
-- Create date: 2020-09-16
-- Description:	filter contract list by location
-- =============================================
CREATE PROCEDURE [dbo].[usp_systemwebadminrole_getPaged]
	@Search			NVARCHAR(50) = '',
	@PageNo			BIGINT = 1,
	@PageSize		BIGINT = 10,
	@OrderColumn	NVARCHAR(100),
	@OrderDir		NVARCHAR(5)
AS
BEGIN
	SET NOCOUNT ON;

	
		WITH DATA_CTE
		AS
		(
			Select tableSource.*, 
			(CASE @OrderDir
			 WHEN 'asc' THEN
				CASE @OrderColumn 
					WHEN 'SystemWebAdminRoleId' THEN ROW_NUMBER() OVER(ORDER BY [SystemWebAdminRoleId] ASC)
					WHEN 'RoleName' THEN ROW_NUMBER() OVER(ORDER BY [RoleName] ASC)
				END
			WHEN 'desc' THEN
				CASE @OrderColumn 
					WHEN 'SystemWebAdminRoleId' THEN ROW_NUMBER() OVER(ORDER BY [SystemWebAdminRoleId] DESC)
					WHEN 'RoleName' THEN ROW_NUMBER() OVER(ORDER BY [RoleName] DESC)
				END
			 END) AS row_num ,
			count(*) over() as TotalRows
			FROM (
			 SELECT 
			 swar.[SystemWebAdminRoleId],
			 MAX(swar.[RoleName])[RoleName]
			FROM [dbo].[SystemWebAdminRole] AS swar
			WHERE swar.EntityStatusId = 1
			AND (swar.[SystemWebAdminRoleId] like '%' + @Search + '%' OR swar.RoleName like '%' + @Search + '%')
			GROUP BY swar.[SystemWebAdminRoleId]
			) tableSource
		)
		SELECT 
		src.*
		 FROM DATA_CTE src
		WHERE src.row_num between ((@PageNo - 1) * @PageSize + 1 ) 
		and (@PageNo * @PageSize)
		ORDER BY src.row_num 

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_update]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminrole_update]
	@SystemWebAdminRoleId		NVARCHAR(100),
	@RoleName				NVARCHAR(100),
	@LastUpdatedBy			NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		-- UPDATE HERE
		UPDATE [dbo].[SystemWebAdminRole]
		SET 
		[RoleName] = @RoleName,
		[LastUpdatedBy] = @LastUpdatedBy,
		[LastUpdatedAt] = GETDATE()
		WHERE [SystemWebAdminRoleId] = @SystemWebAdminRoleId;

		SELECT @@ROWCOUNT;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminroleprivileges_add]
	@SystemWebAdminPrivilegeId	NVARCHAR(100),
	@IsAllowed				BIT,
	@SystemWebAdminRoleId	NVARCHAR(100),
	@CreatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @SystemWebAdminRolePrivilegeId nvarchar(100);
		
		IF EXISTS(SELECT * FROM [dbo].[SystemWebAdminRolePrivileges] WHERE [SystemWebAdminPrivilegeId] = @SystemWebAdminPrivilegeId AND [SystemWebAdminRoleId] = @SystemWebAdminRoleId)
		BEGIN
			SELECT @SystemWebAdminRolePrivilegeId = [SystemWebAdminRolePrivilegeId] FROM [dbo].[SystemWebAdminRolePrivileges] WHERE [SystemWebAdminPrivilegeId] = @SystemWebAdminPrivilegeId AND [SystemWebAdminRoleId] = @SystemWebAdminRoleId;
			UPDATE [dbo].[SystemWebAdminRolePrivileges] SET IsAllowed = 1, EntityStatusId = 1 WHERE [SystemWebAdminRolePrivilegeId] = @SystemWebAdminRolePrivilegeId;
		END
		ELSE
		BEGIN
		
			exec [dbo].[usp_sequence_getNextCode] 'SystemWebAdminRolePrivileges', @Id = @SystemWebAdminRolePrivilegeId OUTPUT

			INSERT INTO [dbo].[SystemWebAdminRolePrivileges](
				[SystemWebAdminRolePrivilegeId],
				[SystemWebAdminPrivilegeId],
				[SystemWebAdminRoleId],
				[IsAllowed],
				[CreatedBy],
				[CreatedAt],
				[EntityStatusId]
			)
			VALUES(
				@SystemWebAdminRolePrivilegeId,
				@SystemWebAdminPrivilegeId,
				@SystemWebAdminRoleId,
				@IsAllowed,
				@CreatedBy,
				GETDATE(),
				1
			);

		END

		SELECT @SystemWebAdminRolePrivilegeId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_delete]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminroleprivileges_delete]
	@SystemWebAdminRolePrivilegeId	NVARCHAR(100),
	@LastUpdatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		IF EXISTS(SELECT * FROM [dbo].[SystemWebAdminRolePrivileges] WHERE [SystemWebAdminRolePrivilegeId] = @SystemWebAdminRolePrivilegeId AND EntityStatusId = 1)
		BEGIN
			-- UPDATE HERE
			UPDATE [dbo].[SystemWebAdminRolePrivileges]
			SET 
			IsAllowed = 0,
			EntityStatusId = 2,
			LastUpdatedBy = @LastUpdatedBy,
			LastUpdatedAt = GETDATE()
			WHERE [SystemWebAdminRolePrivilegeId] = @SystemWebAdminRolePrivilegeId;

			SELECT @@ROWCOUNT;
		END
		ELSE
		BEGIN
			SELECT 1;
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_getBySystemUserId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminroleprivileges_getBySystemUserId]
	@SystemUserId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		SELECT 
		swap.[SystemWebAdminPrivilegeId],
		MAX(swap.[SystemWebAdminPrivilegeName])[SystemWebAdminPrivilegeName]
		FROM [dbo].[SystemWebAdminRolePrivileges] swarp
		LEFT JOIN [dbo].[SystemWebAdminUserRole] swaur ON swarp.[SystemWebAdminRoleId] = swaur.[SystemWebAdminRoleId]
		LEFT JOIN [dbo].[SystemWebAdminPrivileges] swap ON swarp.[SystemWebAdminPrivilegeId] = swap.[SystemWebAdminPrivilegeId]
		WHERE swaur.[SystemUserId] = @SystemUserId
		AND swarp.[IsAllowed] = 1
		AND swarp.[EntityStatusId] = 1
		GROUP BY swap.[SystemWebAdminPrivilegeId]

		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_getBySystemWebAdminPrivilegeIdAndSystemWebAdminRoleId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminroleprivileges_getBySystemWebAdminPrivilegeIdAndSystemWebAdminRoleId]
	@SystemWebAdminPrivilegeId	BIGINT,
	@SystemWebAdminRoleId		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @SystemWebAdminRolePrivilegeId	NVARCHAR(100);
		DECLARE @CreatedBy			NVARCHAR(100);
		DECLARE @CreatedAt			DATETIME;
		DECLARE @LastUpdatedBy		NVARCHAR(100);
		DECLARE @LastUpdatedAt		DATETIME;
		DECLARE @EntityStatusId		BIGINT;
		
		SELECT 
		@SystemWebAdminRolePrivilegeId = swarp.[SystemWebAdminRolePrivilegeId],
		@CreatedBy = swarp.[CreatedBy],
		@CreatedAt = swarp.[CreatedAt],
		@LastUpdatedBy = swarp.[LastUpdatedBy],
		@LastUpdatedAt = swarp.[LastUpdatedAt],
		@EntityStatusId = swarp.[EntityStatusId]
		FROM [dbo].[SystemWebAdminRolePrivileges] swarp
		WHERE swarp.[SystemWebAdminRoleId] = @SystemWebAdminRoleId
		AND swarp.[SystemWebAdminPrivilegeId] = @SystemWebAdminPrivilegeId
		AND swarp.[EntityStatusId] = 1;
		
		SELECT *
		FROM [dbo].[SystemWebAdminRolePrivileges] swarp
		WHERE swarp.[SystemWebAdminRolePrivilegeId] = @SystemWebAdminRolePrivilegeId
		AND swarp.[EntityStatusId] = 1;

		SELECT *
		FROM [dbo].[SystemWebAdminRole] AS swar
		WHERE swar.[SystemWebAdminRoleId] = @SystemWebAdminRoleId;
		
		SELECT *
		FROM [dbo].[SystemWebAdminPrivileges] AS swap
		WHERE swap.[SystemWebAdminPrivilegeId] = @SystemWebAdminPrivilegeId;

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_getBySystemWebAdminRoleId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminroleprivileges_getBySystemWebAdminRoleId]
	@SystemWebAdminRoleId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		SELECT 
		MAX(systemWebAdminRolePrivileges.[SystemWebAdminRolePrivilegeId])[SystemWebAdminRolePrivilegeId], 
		MAX(systemWebAdminRolePrivileges.[IsAllowed])[IsAllowed], 
		systemWebAdminRolePrivileges.[SystemWebAdminPrivilegeId], 
		MAX(systemWebAdminRolePrivileges.[SystemWebAdminPrivilegeName])[SystemWebAdminPrivilegeName],  
		MAX(systemWebAdminRolePrivileges.[SystemWebAdminRoleId])[SystemWebAdminRoleId], 
		MAX(systemWebAdminRolePrivileges.[RoleName])[RoleName]
		FROM (
			SELECT 
			swarp.[SystemWebAdminRolePrivilegeId],
			swarp.[IsAllowed],
			swap.[SystemWebAdminPrivilegeId],
			swap.[SystemWebAdminPrivilegeName],
			swar.[SystemWebAdminRoleId],
			swar.[RoleName]
			FROM [dbo].[SystemWebAdminRolePrivileges] swarp
			LEFT JOIN [dbo].[SystemWebAdminRole] swar ON swarp.[SystemWebAdminRoleId] = swar.[SystemWebAdminRoleId]
			LEFT JOIN [dbo].[SystemWebAdminPrivileges] swap ON swarp.[SystemWebAdminPrivilegeId] = swap.[SystemWebAdminPrivilegeId]
			WHERE swarp.[SystemWebAdminRoleId] = @SystemWebAdminRoleId
			AND swarp.[EntityStatusId] = 1

			UNION ALL

			SELECT 
			CAST(swap.[SystemWebAdminPrivilegeId] AS NVARCHAR(100)) AS [SystemWebAdminRolePrivilegeId],
			0 [IsAllowed],
			swap.[SystemWebAdminPrivilegeId],
			swap.[SystemWebAdminPrivilegeName],
			'' [SystemWebAdminRoleId],
			'' [RoleName]
			FROM [dbo].[SystemWebAdminPrivileges] swap
		) systemWebAdminRolePrivileges
		GROUP BY systemWebAdminRolePrivileges.SystemWebAdminPrivilegeId
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_update]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminroleprivileges_update]
	@SystemWebAdminRolePrivilegeId		NVARCHAR(100),
	@IsAllowed						BIT,
	@LastUpdatedBy					NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		-- UPDATE HERE
		UPDATE [dbo].[SystemWebAdminRolePrivileges]
		SET 
		[IsAllowed] = @IsAllowed,
		[LastUpdatedBy] = @LastUpdatedBy,
		[LastUpdatedAt] = GETDATE()
		WHERE [SystemWebAdminRolePrivilegeId] = @SystemWebAdminRolePrivilegeId;

		SELECT @@ROWCOUNT;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_add]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminuserroles_add]
	@SystemUserId			NVARCHAR(100),
	@SystemWebAdminRoleId	NVARCHAR(100),
	@CreatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @SystemWebAdminUserRoleId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'SystemWebAdminUserRole', @Id = @SystemWebAdminUserRoleId OUTPUT

		INSERT INTO [dbo].[SystemWebAdminUserRole](
			[SystemWebAdminUserRoleId],
			[SystemUserId],
			[SystemWebAdminRoleId],
			[CreatedBy],
			[CreatedAt],
			[EntityStatusId]
		)
		VALUES(
			@SystemWebAdminUserRoleId,
			@SystemUserId,
			@SystemWebAdminRoleId,
			@CreatedBy,
			GETDATE(),
			1
		);

		SELECT @SystemWebAdminUserRoleId;
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_delete]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminuserroles_delete]
	@SystemWebAdminUserRoleId	NVARCHAR(100),
	@LastUpdatedBy	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		IF EXISTS(SELECT * FROM [dbo].[SystemWebAdminUserRole] WHERE [SystemWebAdminUserRoleId] = @SystemWebAdminUserRoleId AND EntityStatusId = 1)
		BEGIN
			-- UPDATE HERE
			UPDATE [dbo].[SystemWebAdminUserRole]
			SET 
			EntityStatusId = 2,
			LastUpdatedBy = @LastUpdatedBy,
			LastUpdatedAt = GETDATE()
			WHERE [SystemWebAdminUserRoleId] = @SystemWebAdminUserRoleId;

			SELECT @@ROWCOUNT;
		END
		ELSE
		BEGIN
			SELECT 1;
		END
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END


GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_getBySystemUserId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminuserroles_getBySystemUserId]
	@SystemUserId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		SELECT 
		swaur.[SystemWebAdminUserRoleId],
		swar.*
		FROM [dbo].[SystemWebAdminUserRole] swaur
		LEFT JOIN [dbo].[SystemUser] su ON swaur.[SystemUserId] = su.[SystemUserId]
		LEFT JOIN [dbo].[SystemWebAdminRole] swar ON swaur.[SystemWebAdminRoleId] = swar.[SystemWebAdminRoleId]
		WHERE su.[SystemUserId] = @SystemUserId
		AND swaur.[EntityStatusId] = 1;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_getBySystemWebAdminRoleIdAndSystemUserId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminuserroles_getBySystemWebAdminRoleIdAndSystemUserId]
	@SystemWebAdminRoleId	NVARCHAR(100),
	@SystemUserId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @SystemWebAdminUserRoleId	NVARCHAR(100);
		DECLARE @CreatedBy			NVARCHAR(100);
		DECLARE @CreatedAt			DATETIME;
		DECLARE @LastUpdatedBy		NVARCHAR(100);
		DECLARE @LastUpdatedAt		DATETIME;
		DECLARE @EntityStatusId		BIGINT;
		
		SELECT 
		@SystemWebAdminUserRoleId = swaur.[SystemWebAdminUserRoleId],
		@SystemUserId = swaur.[SystemUserId],
		@SystemWebAdminRoleId = swaur.[SystemWebAdminRoleId],
		@CreatedBy = swaur.[CreatedBy],
		@CreatedAt = swaur.[CreatedAt],
		@LastUpdatedBy = swaur.[LastUpdatedBy],
		@LastUpdatedAt = swaur.[LastUpdatedAt],
		@EntityStatusId = swaur.[EntityStatusId]
		FROM [dbo].[SystemWebAdminUserRole] swaur
		WHERE swaur.[SystemUserId] = @SystemUserId
		AND swaur.[SystemWebAdminRoleId] = @SystemWebAdminRoleId
		AND swaur.[EntityStatusId] = 1;
		
		SELECT *
		FROM [dbo].[SystemWebAdminUserRole] swaur
		WHERE swaur.[SystemWebAdminUserRoleId] = @SystemWebAdminUserRoleId
		AND swaur.[EntityStatusId] = 1;

		SELECT *
		FROM [dbo].[SystemWebAdminRole] AS swar
		WHERE swar.[SystemWebAdminRoleId] = @SystemWebAdminRoleId;
		
		SELECT *
		FROM [dbo].[SystemUser] AS su
		WHERE su.[SystemUserId] = @SystemUserId;

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_getBySystemWebAdminUserRoleId]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_systemwebadminuserroles_getBySystemWebAdminUserRoleId]
	@SystemWebAdminUserRoleId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @SystemUsersId	NVARCHAR(100);
		DECLARE @SystemWebAdminRoleId	NVARCHAR(100);
		DECLARE @CreatedBy			NVARCHAR(100);
		DECLARE @CreatedAt			DATETIME;
		DECLARE @LastUpdatedBy		NVARCHAR(100);
		DECLARE @LastUpdatedAt		DATETIME;
		DECLARE @EntityStatusId		BIGINT;
		
		SELECT 
		@SystemUsersId = swaur.[SystemUserId],
		@SystemWebAdminRoleId = swaur.[SystemWebAdminRoleId],
		@CreatedBy = swaur.[CreatedBy],
		@CreatedAt = swaur.[CreatedAt],
		@LastUpdatedBy = swaur.[LastUpdatedBy],
		@LastUpdatedAt = swaur.[LastUpdatedAt],
		@EntityStatusId = swaur.[EntityStatusId]
		FROM [dbo].[SystemWebAdminUserRole] swaur
		WHERE swaur.[SystemWebAdminUserRoleId] = @SystemWebAdminUserRoleId
		AND swaur.[EntityStatusId] = 1;
		
		SELECT *
		FROM [dbo].[SystemWebAdminUserRole] swaur
		WHERE swaur.[SystemWebAdminUserRoleId] = @SystemWebAdminUserRoleId
		AND swaur.[EntityStatusId] = 1;

		SELECT *
		FROM [dbo].[SystemWebAdminRole] AS swar
		WHERE swar.[SystemWebAdminRoleId] = @SystemWebAdminRoleId;
		
		SELECT *
		FROM [dbo].[SystemUser] AS su
		WHERE su.[SystemUserId] = @SystemUsersId;

		SELECT 
		@CreatedBy AS CreatedBy,
		@CreatedAt AS CreatedAt,
		[dbo].[svf_getUserFullName](@CreatedBy) AS CreatedByFullName,
		@LastUpdatedBy AS LastUpdatedBy,
		@LastUpdatedAt AS LastUpdatedAt,
		[dbo].[svf_getUserFullName](@LastUpdatedBy) AS LastUpdatedByFullName

		SELECT  
		es.[EntityStatusId],
		es.[EntityStatusName]
		FROM [dbo].[EntityStatus] AS es
		WHERE es.[EntityStatusId] = @EntityStatusId;
		
		RETURN 0
        
    END TRY
    BEGIN CATCH

        SELECT
			'Error - ' + ERROR_MESSAGE()   AS ErrorMessage,
            'Error'           AS Status,
            ERROR_NUMBER()    AS ErrorNumber,
            ERROR_SEVERITY()  AS ErrorSeverity,
            ERROR_STATE()     AS ErrorState,
            ERROR_PROCEDURE() AS ErrorProcedure,
            ERROR_LINE()      AS ErrorLine,
            ERROR_MESSAGE()   AS ErrorMessage;

    END CATCH

END



GO
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_getPaged]    Script Date: 6/9/2022 10:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		erwin
-- Create date: 2020-09-16
-- Description:	filter contract list by location
-- =============================================
CREATE PROCEDURE [dbo].[usp_systemwebadminuserroles_getPaged]
	@SystemUserId	NVARCHAR(100) = '',
	@Search			NVARCHAR(50) = '',
	@PageNo			BIGINT = 1,
	@PageSize		BIGINT = 10,
	@OrderColumn	NVARCHAR(100),
	@OrderDir		NVARCHAR(5)
AS
BEGIN
	SET NOCOUNT ON;

	
		WITH DATA_CTE
		AS
		(
			Select tableSource.*, 
			(CASE @OrderDir
			 WHEN 'asc' THEN
				CASE @OrderColumn 
					WHEN 'SystemWebAdminUserRoleId' THEN ROW_NUMBER() OVER(ORDER BY [SystemWebAdminUserRoleId] ASC)
					WHEN 'SystemRole.RoleName' THEN ROW_NUMBER() OVER(ORDER BY [RoleName] ASC)
				END
			WHEN 'desc' THEN
				CASE @OrderColumn 
					WHEN 'SystemWebAdminUserRoleId' THEN ROW_NUMBER() OVER(ORDER BY [SystemWebAdminUserRoleId] DESC)
					WHEN 'SystemRole.RoleName' THEN ROW_NUMBER() OVER(ORDER BY [RoleName] DESC)
				END
			 END) AS row_num ,
			count(*) over() as TotalRows
			FROM (
			 SELECT 
			 swaur.[SystemWebAdminUserRoleId],
			 MAX(swar.[SystemWebAdminRoleId])[SystemWebAdminRoleId],
			 MAX(swar.RoleName)[RoleName]
			FROM [dbo].[SystemWebAdminUserRole] AS swaur
			LEFT JOIN [dbo].[SystemWebAdminRole] swar ON swaur.[SystemWebAdminRoleId] = swar.[SystemWebAdminRoleId]
			WHERE swaur.EntityStatusId = 1 AND swaur.SystemUserId = @SystemUserId
			AND (swar.RoleName like '%' + @Search + '%')
			GROUP BY swaur.[SystemWebAdminUserRoleId]
			) tableSource
		)
		SELECT 
		src.*
		 FROM DATA_CTE src
		WHERE src.row_num between ((@PageNo - 1) * @PageSize + 1 ) 
		and (@PageNo * @PageSize)
		ORDER BY src.row_num 

END



GO
