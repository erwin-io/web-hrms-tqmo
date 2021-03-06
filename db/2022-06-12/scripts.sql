USE [hrms]
GO
/****** Object:  UserDefinedFunction [dbo].[LPAD]    Script Date: 6/12/2022 9:32:04 PM ******/
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
/****** Object:  UserDefinedFunction [dbo].[svf_getUserFullName]    Script Date: 6/12/2022 9:32:04 PM ******/
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
/****** Object:  UserDefinedFunction [dbo].[tvf_SplitString]    Script Date: 6/12/2022 9:32:04 PM ******/
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
/****** Object:  Table [dbo].[Appointment]    Script Date: 6/12/2022 9:32:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appointment](
	[AppointmentId] [nvarchar](100) NOT NULL,
	[PatientId] [nvarchar](100) NOT NULL,
	[AppointmentDate] [datetime] NOT NULL,
	[AppointmentTime] [time](7) NOT NULL,
	[PrimaryReason] [nvarchar](max) NOT NULL,
	[DateSymtomsFirstNoted] [datetime] NOT NULL,
	[DescOfCharOfSymtoms] [nvarchar](max) NOT NULL,
	[HasPrevMedTreatment] [bit] NOT NULL,
	[IsTakingBloodThinningDrugs] [bit] NOT NULL,
	[PatientGuardian] [nvarchar](500) NULL,
	[PatientGuardianMobileNumber] [bigint] NULL,
	[PatientRelative] [nvarchar](500) NULL,
	[PatientRelativeMobileNumber] [bigint] NULL,
	[AppointmentStatusId] [bigint] NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NOT NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_Appointment] PRIMARY KEY CLUSTERED 
(
	[AppointmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AppointmentStatus]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[CivilStatus]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CivilStatus](
	[CivilStatusId] [bigint] NOT NULL,
	[CivilStatusName] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_CivilStatus] PRIMARY KEY CLUSTERED 
(
	[CivilStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Doctor]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Doctor](
	[DoctorId] [nvarchar](100) NOT NULL,
	[LegalEntityId] [nvarchar](100) NOT NULL,
	[CompleteAddress] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_Doctor] PRIMARY KEY CLUSTERED 
(
	[DoctorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EntityApprovalStatus]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[EntityGender]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[EntityStatus]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[ExistingPastDiseases]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExistingPastDiseases](
	[ExistingPastDiseasesId] [nvarchar](100) NOT NULL,
	[AppointmentId] [nvarchar](100) NOT NULL,
	[ExistingPastDiseasesName] [nvarchar](500) NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_ExistingPastDiseases] PRIMARY KEY CLUSTERED 
(
	[ExistingPastDiseasesId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[File]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[LegalEntity]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LegalEntity](
	[LegalEntityId] [nvarchar](100) NOT NULL,
	[FirstName] [nvarchar](200) NOT NULL,
	[MiddleName] [nvarchar](200) NULL,
	[LastName] [nvarchar](200) NOT NULL,
	[GenderId] [bigint] NOT NULL,
	[BirthDate] [date] NOT NULL,
	[Age] [bigint] NULL,
	[EmailAddress] [nvarchar](100) NOT NULL,
	[MobileNumber] [bigint] NOT NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_LegalEntity] PRIMARY KEY CLUSTERED 
(
	[LegalEntityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LegalEntityAddress]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[Patient]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Patient](
	[PatientId] [nvarchar](100) NOT NULL,
	[LegalEntityId] [nvarchar](100) NOT NULL,
	[IsNew] [bit] NOT NULL,
	[CivilStatusId] [bigint] NOT NULL,
	[Occupation] [nvarchar](200) NOT NULL,
	[CompleteAddress] [nvarchar](max) NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED 
(
	[PatientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PrevMedicationTaken]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PrevMedicationTaken](
	[PrevMedicationTakenId] [nvarchar](100) NOT NULL,
	[AppointmentId] [nvarchar](100) NOT NULL,
	[PrevMedicationTakenName] [nvarchar](500) NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_PrevMedicationTaken] PRIMARY KEY CLUSTERED 
(
	[PrevMedicationTakenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReferedPerson]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReferedPerson](
	[ReferedPersonId] [nvarchar](100) NOT NULL,
	[AppointmentId] [nvarchar](100) NOT NULL,
	[IsReferedPersonInRecord] [bit] NOT NULL,
	[ReferedPersonName] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[LastUpdatedBy] [nvarchar](100) NULL,
	[LastUpdatedAt] [datetime] NULL,
	[EntityStatusId] [bigint] NOT NULL,
 CONSTRAINT [PK_ReferedPerson] PRIMARY KEY CLUSTERED 
(
	[ReferedPersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sequence]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemConfig]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemConfigType]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemToken]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemUser]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemUserConfig]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemUserContact]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemUserType]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemUserVerification]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemUserVerificationType]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemWebAdminMenu]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemWebAdminMenuRole]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemWebAdminModule]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemWebAdminPrivileges]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemWebAdminRole]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemWebAdminRolePrivileges]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Table [dbo].[SystemWebAdminUserRole]    Script Date: 6/12/2022 9:32:05 PM ******/
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
INSERT [dbo].[AppointmentStatus] ([AppointmentStatusId], [AppointmentStatusName]) VALUES (1, N'Pending')
GO
INSERT [dbo].[AppointmentStatus] ([AppointmentStatusId], [AppointmentStatusName]) VALUES (2, N'Processed')
GO
INSERT [dbo].[AppointmentStatus] ([AppointmentStatusId], [AppointmentStatusName]) VALUES (3, N'Completed')
GO
INSERT [dbo].[AppointmentStatus] ([AppointmentStatusId], [AppointmentStatusName]) VALUES (4, N'Canceled')
GO
INSERT [dbo].[AppointmentStatus] ([AppointmentStatusId], [AppointmentStatusName]) VALUES (5, N'Declined')
GO
INSERT [dbo].[CivilStatus] ([CivilStatusId], [CivilStatusName]) VALUES (1, N'Single')
GO
INSERT [dbo].[CivilStatus] ([CivilStatusId], [CivilStatusName]) VALUES (2, N'Married')
GO
INSERT [dbo].[CivilStatus] ([CivilStatusId], [CivilStatusName]) VALUES (3, N'Divorced')
GO
INSERT [dbo].[CivilStatus] ([CivilStatusId], [CivilStatusName]) VALUES (4, N'Widowed ')
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
INSERT [dbo].[EntityGender] ([GenderId], [GenderName]) VALUES (3, N'Rather not say')
GO
INSERT [dbo].[EntityStatus] ([EntityStatusId], [EntityStatusName]) VALUES (1, N'Active')
GO
INSERT [dbo].[EntityStatus] ([EntityStatusId], [EntityStatusName]) VALUES (2, N'Deleted')
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (1, N'SystemUser', N'SU-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (2, N'SystemUserConfig', N'SUC-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (3, N'SystemUserContact', N'SUCON-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (4, N'SystemWebAdminUserRole', N'SWAUR-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (5, N'SystemWebAdminRole', N'SWAR-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (6, N'LegalEntity', N'LE-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (7, N'LegalGeoAddress', N'LGA-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (8, N'SystemWebAdminMenuRole', N'SWAMR-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (9, N'File', N'F-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (10, N'LegalEntityAddress', N'LEA-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (11, N'SystemWebAdminRolePrivileges', N'SWARP-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (12, N'Patient', N'P-', 10, 0)
GO
INSERT [dbo].[Sequence] ([SequenceId], [TableName], [Prefix], [Length], [LastNumber]) VALUES (13, N'Doctor', N'D-', 10, 0)
GO
INSERT [dbo].[SystemConfig] ([SystemConfigId], [ConfigName], [ConfigGroup], [ConfigKey], [ConfigValue], [SystemConfigTypeId], [IsUserConfigurable]) VALUES (1, N'System Version', N'System Version', N'SYSTEM_VERSION', N'1', 2, 0)
GO
INSERT [dbo].[SystemConfig] ([SystemConfigId], [ConfigName], [ConfigGroup], [ConfigKey], [ConfigValue], [SystemConfigTypeId], [IsUserConfigurable]) VALUES (2, N'API Version', N'System Version', N'API_VERSION', N'1', 2, 0)
GO
INSERT [dbo].[SystemConfigType] ([SystemConfigTypeId], [ValueType]) VALUES (1, N'BOOLEAN')
GO
INSERT [dbo].[SystemConfigType] ([SystemConfigTypeId], [ValueType]) VALUES (2, N'TEXT')
GO
INSERT [dbo].[SystemUserType] ([SystemUserTypeId], [SystemUserTypeName]) VALUES (1, N'WebAppAdmin')
GO
INSERT [dbo].[SystemUserType] ([SystemUserTypeId], [SystemUserTypeName]) VALUES (2, N'MobileAppUser')
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
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (7, 4, N'Appointments', 1)
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (8, 4, N'Medical Records', 1)
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (9, 3, N'Patients', 1)
GO
INSERT [dbo].[SystemWebAdminMenu] ([SystemWebAdminMenuId], [SystemWebAdminModuleId], [SystemWebAdminMenuName], [EntityStatusId]) VALUES (10, 3, N'Doctors', 1)
GO
INSERT [dbo].[SystemWebAdminModule] ([SystemWebAdminModuleId], [SystemWebAdminModuleName]) VALUES (1, N'Dashboard')
GO
INSERT [dbo].[SystemWebAdminModule] ([SystemWebAdminModuleId], [SystemWebAdminModuleName]) VALUES (4, N'Health Records')
GO
INSERT [dbo].[SystemWebAdminModule] ([SystemWebAdminModuleId], [SystemWebAdminModuleName]) VALUES (2, N'System Admin Security')
GO
INSERT [dbo].[SystemWebAdminModule] ([SystemWebAdminModuleId], [SystemWebAdminModuleName]) VALUES (3, N'System Setup')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (1, N'Allowed to processed appointment')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (2, N'Allowed to decline appointment')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (3, N'Allowed to view appointment details')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (4, N'Allowed to add user')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (5, N'Allowed to update user')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (6, N'Allowed to delete user')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (7, N'Allowed to add system web admin role')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (8, N'Allowed to update system web admin role')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (9, N'Allowed to delete system web admin role')
GO
INSERT [dbo].[SystemWebAdminPrivileges] ([SystemWebAdminPrivilegeId], [SystemWebAdminPrivilegeName]) VALUES (10, N'Allowed to update system web admin menu role')
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [U_Sequence]    Script Date: 6/12/2022 9:32:05 PM ******/
ALTER TABLE [dbo].[Sequence] ADD  CONSTRAINT [U_Sequence] UNIQUE NONCLUSTERED 
(
	[TableName] ASC,
	[Prefix] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemUser]    Script Date: 6/12/2022 9:32:05 PM ******/
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [UK_SystemUser] UNIQUE NONCLUSTERED 
(
	[UserName] ASC,
	[EntityStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemWebAdminMenu]    Script Date: 6/12/2022 9:32:05 PM ******/
ALTER TABLE [dbo].[SystemWebAdminMenu] ADD  CONSTRAINT [UK_SystemWebAdminMenu] UNIQUE NONCLUSTERED 
(
	[SystemWebAdminModuleId] ASC,
	[SystemWebAdminMenuName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemWebAdminMenuRole]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  Index [UK_SystemWebAdminModule]    Script Date: 6/12/2022 9:32:05 PM ******/
ALTER TABLE [dbo].[SystemWebAdminModule] ADD  CONSTRAINT [UK_SystemWebAdminModule] UNIQUE NONCLUSTERED 
(
	[SystemWebAdminModuleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_SystemRole]    Script Date: 6/12/2022 9:32:05 PM ******/
ALTER TABLE [dbo].[SystemWebAdminRole] ADD  CONSTRAINT [UK_SystemRole] UNIQUE NONCLUSTERED 
(
	[RoleName] ASC,
	[EntityStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Appointment] ADD  CONSTRAINT [DF_Appointment_HasPrevMedTreatment]  DEFAULT ((0)) FOR [HasPrevMedTreatment]
GO
ALTER TABLE [dbo].[Appointment] ADD  CONSTRAINT [DF_Appointment_IsTakingBloodThinningDrugs]  DEFAULT ((0)) FOR [IsTakingBloodThinningDrugs]
GO
ALTER TABLE [dbo].[Appointment] ADD  CONSTRAINT [DF_Appointment_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[Doctor] ADD  CONSTRAINT [DF_Doctor_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[ExistingPastDiseases] ADD  CONSTRAINT [DF_ExistingPastDiseases_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_FileSize]  DEFAULT ((0)) FOR [FileSize]
GO
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_IsFromStorage]  DEFAULT ((0)) FOR [IsFromStorage]
GO
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[LegalEntity] ADD  CONSTRAINT [DF_LegalEntity_GenderId]  DEFAULT ((1)) FOR [GenderId]
GO
ALTER TABLE [dbo].[LegalEntity] ADD  CONSTRAINT [DF_LegalEntity_Age]  DEFAULT ((0)) FOR [Age]
GO
ALTER TABLE [dbo].[LegalEntity] ADD  CONSTRAINT [DF_LegalEntity_MobileNumber]  DEFAULT ((0)) FOR [MobileNumber]
GO
ALTER TABLE [dbo].[LegalEntity] ADD  CONSTRAINT [DF_LegalEntity_EntityStatusId_1]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[LegalEntityAddress] ADD  CONSTRAINT [DF_LegalGeoAddress_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[Patient] ADD  CONSTRAINT [DF_Patient_IsNew]  DEFAULT ((1)) FOR [IsNew]
GO
ALTER TABLE [dbo].[Patient] ADD  CONSTRAINT [DF_Patient_CivilStatusId]  DEFAULT ((1)) FOR [CivilStatusId]
GO
ALTER TABLE [dbo].[Patient] ADD  CONSTRAINT [DF_Patient_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[PrevMedicationTaken] ADD  CONSTRAINT [DF_PrevMedicationTaken_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
GO
ALTER TABLE [dbo].[ReferedPerson] ADD  CONSTRAINT [DF_ReferedPerson_IsReferedPersonInRecord]  DEFAULT ((0)) FOR [IsReferedPersonInRecord]
GO
ALTER TABLE [dbo].[ReferedPerson] ADD  CONSTRAINT [DF_ReferedPerson_EntityStatusId]  DEFAULT ((1)) FOR [EntityStatusId]
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
ALTER TABLE [dbo].[LegalEntity]  WITH CHECK ADD  CONSTRAINT [FK_LegalEntity_LegalEntity] FOREIGN KEY([LegalEntityId])
REFERENCES [dbo].[LegalEntity] ([LegalEntityId])
GO
ALTER TABLE [dbo].[LegalEntity] CHECK CONSTRAINT [FK_LegalEntity_LegalEntity]
GO
ALTER TABLE [dbo].[SystemUser]  WITH CHECK ADD  CONSTRAINT [FK_SystemUser_LegalEntity] FOREIGN KEY([LegalEntityId])
REFERENCES [dbo].[LegalEntity] ([LegalEntityId])
GO
ALTER TABLE [dbo].[SystemUser] CHECK CONSTRAINT [FK_SystemUser_LegalEntity]
GO
/****** Object:  StoredProcedure [dbo].[usp_doctor_add]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_doctor_add]
	@LegalEntityId			NVARCHAR(100),
	@CompleteAddress		NVARCHAR(MAX),
	@CreatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @DoctorId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'Doctor', @Id = @DoctorId OUTPUT

		INSERT INTO [dbo].[Doctor](
			[DoctorId],
			[LegalEntityId],
			[CompleteAddress],
			[CreatedBy],
			[CreatedAt],
			[EntityStatusId]
		)
		VALUES(
			@DoctorId,
			@LegalEntityId,
			@CompleteAddress,
			@CreatedBy,
			GETDATE(),
			1
		);

		SELECT @DoctorId;
        
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
/****** Object:  StoredProcedure [dbo].[usp_doctor_delete]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_doctor_delete]
	@DoctorId			NVARCHAR(100),
	@LastUpdatedBy		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @LegalEntityId NVARCHAR(100);

		IF EXISTS(SELECT * FROM [dbo].[Doctor] WHERE [DoctorId] = @DoctorId AND EntityStatusId = 1)
		BEGIN
			
			SELECT @LegalEntityId = [LegalEntityId] FROM [dbo].[Doctor] WHERE [DoctorId] = @DoctorId AND EntityStatusId = 1
			-- UPDATE HERE
			UPDATE [dbo].[LegalEntity]
			SET 
			[FirstName] = @LegalEntityId + '-' + [FirstName] + '-' + '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			[LastName] = @LegalEntityId + '-' + [LastName] + '-' + '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			EntityStatusId = 2
			WHERE [LegalEntityId] = @LegalEntityId;

			UPDATE [dbo].[Doctor]
			SET 
			[LegalEntityId] = '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			EntityStatusId = 2,
			[LastUpdatedBy] = @LastUpdatedBy,
			[LastUpdatedAt] = GETDATE()
			WHERE [DoctorId] = @DoctorId;

			SELECT 1;
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
/****** Object:  StoredProcedure [dbo].[usp_doctor_getByID]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_doctor_getByID]
	@DoctorId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @LegalEntityId			NVARCHAR(100);
		DECLARE @GenderId				BIGINT;
		DECLARE @CreatedBy				NVARCHAR(100);
		DECLARE @CreatedAt				DATETIME;
		DECLARE @LastUpdatedBy			NVARCHAR(100);
		DECLARE @LastUpdatedAt			DATETIME;
		DECLARE @EntityStatusId			BIGINT;
		
		SELECT 
		@LegalEntityId = d.[LegalEntityId],
		@CreatedBy = d.[CreatedBy],
		@CreatedAt = d.[CreatedAt],
		@LastUpdatedBy = d.[LastUpdatedBy],
		@LastUpdatedAt = d.[LastUpdatedAt],
		@EntityStatusId = d.[EntityStatusId]
		FROM [dbo].[Doctor] d
		WHERE d.[DoctorId] = @DoctorId
		AND d.[EntityStatusId] = 1;
		
		SELECT  *
		FROM [dbo].[Doctor] AS d
		WHERE d.[DoctorId] = @DoctorId
		AND d.[EntityStatusId] = 1;

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
/****** Object:  StoredProcedure [dbo].[usp_doctor_getPaged]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		erwin
-- Create date: 2020-09-16
-- Description:	filter contract list by location
-- =============================================
CREATE PROCEDURE [dbo].[usp_doctor_getPaged]
	@Search					NVARCHAR(50) = '',
	@PageNo					BIGINT = 1,
	@PageSize				BIGINT = 10,
	@OrderColumn			NVARCHAR(100) = 'DoctorId',
	@OrderDir				NVARCHAR(5) = 'asc'
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
					WHEN 'DoctorId' THEN ROW_NUMBER() OVER(ORDER BY [DoctorId] ASC)
					WHEN 'CompleteAddress' THEN ROW_NUMBER() OVER(ORDER BY [CompleteAddress] ASC)
					WHEN 'LegalEntity.FullName' THEN ROW_NUMBER() OVER(ORDER BY [FullName] ASC)
					WHEN 'LegalEntity.Gender.GenderName' THEN ROW_NUMBER() OVER(ORDER BY [GenderName] ASC)
					WHEN 'LegalEntity.Age' THEN ROW_NUMBER() OVER(ORDER BY [Age] ASC)
					WHEN 'LegalEntity.EmailAddress' THEN ROW_NUMBER() OVER(ORDER BY [EmailAddress] ASC)
					WHEN 'LegalEntity.MobileNumber' THEN ROW_NUMBER() OVER(ORDER BY [MobileNumber] ASC)
				END
			WHEN 'desc' THEN
				CASE @OrderColumn 
					WHEN 'DoctorId' THEN ROW_NUMBER() OVER(ORDER BY [DoctorId] DESC)
					WHEN 'CompleteAddress' THEN ROW_NUMBER() OVER(ORDER BY [CompleteAddress] DESC)
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
			 d.[DoctorId],
			 MAX(d.[CompleteAddress])[CompleteAddress],
			 MAX(le.[LegalEntityId])[LegalEntityId],
			 MAX(le.[FullName])[FullName],
			 MAX(le.[BirthDate])[BirthDate],
			 MAX(le.[Age])[Age],
			 MAX(le.[EmailAddress])[EmailAddress],
			 MAX(le.[MobileNumber])[MobileNumber],
			 MAX(eg.[GenderId])[GenderId],
			 MAX(eg.[GenderName])[GenderName]
			FROM [dbo].[Doctor] AS d
			LEFT JOIN (SELECT *,ISNULL([FirstName],'') + ' ' + ISNULL([MiddleName],'') + ' ' + ISNULL([LastName],'') AS [FullName] FROM [dbo].[LegalEntity] WHERE EntityStatusId = 1) AS le ON d.LegalEntityId = le.LegalEntityId
			LEFT JOIN [dbo].[EntityGender] AS eg ON le.GenderId = eg.GenderId
			WHERE d.EntityStatusId = 1
			AND (d.[DoctorId] like '%' + @Search + '%' 
			OR le.[FullName] like '%' + @Search + '%' 
			OR eg.[GenderName] like '%' + @Search + '%'
			OR le.[Age] like '%' + @Search + '%' 
			OR le.[EmailAddress] like '%' + @Search + '%'
			OR le.[MobileNumber] like '%' + @Search + '%' )
			GROUP BY d.[DoctorId]
			) tableSource
		)
		SELECT 
		src.*,
		src.row_num,
		src.TotalRows
		FROM DATA_CTE src
		WHERE src.row_num between ((@PageNo - 1) * @PageSize + 1 ) 
		and (@PageNo * @PageSize)
		ORDER BY src.row_num 

END
GO
/****** Object:  StoredProcedure [dbo].[usp_doctor_update]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_doctor_update]
	@DoctorId			NVARCHAR(100),
	@CompleteAddress	NVARCHAR(200),
	@LastUpdatedBy		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		-- UPDATE HERE
		UPDATE [dbo].[Doctor]
		SET 
		[CompleteAddress] = @CompleteAddress,
		[LastUpdatedBy] = @LastUpdatedBy,
		[LastUpdatedAt] = GETDATE()
		WHERE [DoctorId] = @DoctorId;

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
/****** Object:  StoredProcedure [dbo].[usp_file_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_file_getById]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_file_update]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_legalentity_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
	@MiddleName			NVARCHAR(100) = '',
	@LastName			NVARCHAR(100) = '',
	@GenderId			BIGINT = 1,
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
			IIF(@GenderId <> 1 AND @GenderId <> 2, 3,@GenderId),
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
/****** Object:  StoredProcedure [dbo].[usp_legalentity_update]    Script Date: 6/12/2022 9:32:05 PM ******/
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
	@MiddleName			NVARCHAR(100),
	@LastName			NVARCHAR(100),
	@GenderId			BIGINT = 1,
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
			[GenderId] = IIF(@GenderId <> 1 AND @GenderId <> 2, 3,@GenderId),
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
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_delete]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_getById]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_getByLegalEntityId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_getBySystemUserId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_legalentityaddress_update]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_lookuptable_getByTableNames]    Script Date: 6/12/2022 9:32:05 PM ******/
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

		SELECT 'CivilStatus' AS LookupName,CAST([CivilStatusId] AS nvarchar(100)) AS Id,[CivilStatusName] AS Name FROM [dbo].[CivilStatus]
		WHERE 'CivilStatus' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )

		UNION ALL

		SELECT 'AppointmentStatus' AS LookupName,CAST([AppointmentStatusId] AS nvarchar(100)) AS Id,[AppointmentStatusName] AS Name FROM [dbo].[AppointmentStatus]
		WHERE 'AppointmentStatus' IN (select Item from [dbo].[tvf_SplitString](@TableNames,',') )

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
/****** Object:  StoredProcedure [dbo].[usp_patient_add]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_patient_add]
	@LegalEntityId			NVARCHAR(100),
	@CivilStatusId			BIGINT,
	@Occupation				NVARCHAR(200),
	@CompleteAddress		NVARCHAR(MAX),
	@CreatedBy				NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;

		DECLARE @PatientId nvarchar(100);
		
		exec [dbo].[usp_sequence_getNextCode] 'Patient', @Id = @PatientId OUTPUT

		INSERT INTO [dbo].[Patient](
			[PatientId],
			[LegalEntityId],
			[IsNew],
			[CivilStatusId],
			[Occupation],
			[CompleteAddress],
			[CreatedBy],
			[CreatedAt],
			[EntityStatusId]
		)
		VALUES(
			@PatientId,
			@LegalEntityId,
			1,
			@CivilStatusId,
			@Occupation,
			@CompleteAddress,
			@CreatedBy,
			GETDATE(),
			1
		);

		SELECT @PatientId;
        
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
/****** Object:  StoredProcedure [dbo].[usp_patient_delete]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_patient_delete]
	@PatientId			NVARCHAR(100),
	@LastUpdatedBy		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @LegalEntityId NVARCHAR(100);

		IF EXISTS(SELECT * FROM [dbo].[Patient] WHERE [PatientId] = @PatientId AND EntityStatusId = 1)
		BEGIN
			
			SELECT @LegalEntityId = [LegalEntityId] FROM [dbo].[Patient] WHERE [PatientId] = @PatientId AND EntityStatusId = 1
			-- UPDATE HERE
			UPDATE [dbo].[LegalEntity]
			SET 
			[FirstName] = @LegalEntityId + '-' + [FirstName] + '-' + '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			[LastName] = @LegalEntityId + '-' + [LastName] + '-' + '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			EntityStatusId = 2
			WHERE [LegalEntityId] = @LegalEntityId;

			UPDATE [dbo].[Patient]
			SET 
			[LegalEntityId] = '(DELETED - ' + CONVERT(VARCHAR(50),GETDATE())+ ')',
			EntityStatusId = 2,
			[LastUpdatedBy] = @LastUpdatedBy,
			[LastUpdatedAt] = GETDATE()
			WHERE [PatientId] = @PatientId;

			SELECT 1;
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
/****** Object:  StoredProcedure [dbo].[usp_patient_getByID]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_patient_getByID]
	@PatientId	NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		
		DECLARE @LegalEntityId			NVARCHAR(100);
		DECLARE @CivilStatusId			BIGINT;
		DECLARE @GenderId				BIGINT;
		DECLARE @CreatedBy				NVARCHAR(100);
		DECLARE @CreatedAt				DATETIME;
		DECLARE @LastUpdatedBy			NVARCHAR(100);
		DECLARE @LastUpdatedAt			DATETIME;
		DECLARE @EntityStatusId			BIGINT;
		
		SELECT 
		@LegalEntityId = p.[LegalEntityId],
		@CivilStatusId = p.[CivilStatusId],
		@CreatedBy = p.[CreatedBy],
		@CreatedAt = p.[CreatedAt],
		@LastUpdatedBy = p.[LastUpdatedBy],
		@LastUpdatedAt = p.[LastUpdatedAt],
		@EntityStatusId = p.[EntityStatusId]
		FROM [dbo].[Patient] p
		WHERE p.[PatientId] = @PatientId
		AND p.[EntityStatusId] = 1;
		
		SELECT  *
		FROM [dbo].[Patient] AS p
		WHERE p.[PatientId] = @PatientId
		AND p.[EntityStatusId] = 1;

		SELECT  *
		FROM [dbo].[CivilStatus] AS cs
		WHERE cs.[CivilStatusId] = @CivilStatusId;

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
/****** Object:  StoredProcedure [dbo].[usp_patient_getPaged]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		erwin
-- Create date: 2020-09-16
-- Description:	filter contract list by location
-- =============================================
CREATE PROCEDURE [dbo].[usp_patient_getPaged]
	@Search					NVARCHAR(50) = '',
	@PageNo					BIGINT = 1,
	@PageSize				BIGINT = 10,
	@OrderColumn			NVARCHAR(100) = 'PatientId',
	@OrderDir				NVARCHAR(5) = 'asc'
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
					WHEN 'PatientId' THEN ROW_NUMBER() OVER(ORDER BY [PatientId] ASC)
					WHEN 'Occupation' THEN ROW_NUMBER() OVER(ORDER BY [Occupation] ASC)
					WHEN 'CompleteAddress' THEN ROW_NUMBER() OVER(ORDER BY [CompleteAddress] ASC)
					WHEN 'LegalEntity.FullName' THEN ROW_NUMBER() OVER(ORDER BY [FullName] ASC)
					WHEN 'LegalEntity.Gender.GenderName' THEN ROW_NUMBER() OVER(ORDER BY [GenderName] ASC)
					WHEN 'LegalEntity.Age' THEN ROW_NUMBER() OVER(ORDER BY [Age] ASC)
					WHEN 'LegalEntity.EmailAddress' THEN ROW_NUMBER() OVER(ORDER BY [EmailAddress] ASC)
					WHEN 'LegalEntity.MobileNumber' THEN ROW_NUMBER() OVER(ORDER BY [MobileNumber] ASC)
				END
			WHEN 'desc' THEN
				CASE @OrderColumn 
					WHEN 'PatientId' THEN ROW_NUMBER() OVER(ORDER BY [PatientId] DESC)
					WHEN 'Occupation' THEN ROW_NUMBER() OVER(ORDER BY [Occupation] DESC)
					WHEN 'CompleteAddress' THEN ROW_NUMBER() OVER(ORDER BY [CompleteAddress] DESC)
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
			 p.[PatientId],
			 MAX(p.[Occupation])[Occupation],
			 MAX(p.[CompleteAddress])[CompleteAddress],
			 MAX(le.[LegalEntityId])[LegalEntityId],
			 MAX(le.[FullName])[FullName],
			 MAX(le.[BirthDate])[BirthDate],
			 MAX(le.[Age])[Age],
			 MAX(le.[EmailAddress])[EmailAddress],
			 MAX(le.[MobileNumber])[MobileNumber],
			 MAX(eg.[GenderId])[GenderId],
			 MAX(eg.[GenderName])[GenderName]
			FROM [dbo].[Patient] AS p
			LEFT JOIN (SELECT *,ISNULL([FirstName],'') + ' ' + ISNULL([MiddleName],'') + ' ' + ISNULL([LastName],'') AS [FullName] FROM [dbo].[LegalEntity] WHERE EntityStatusId = 1) AS le ON p.LegalEntityId = le.LegalEntityId
			LEFT JOIN [dbo].[EntityGender] AS eg ON le.GenderId = eg.GenderId
			WHERE p.EntityStatusId = 1
			AND (p.[PatientId] like '%' + @Search + '%' 
			OR le.[FullName] like '%' + @Search + '%' 
			OR eg.[GenderName] like '%' + @Search + '%'
			OR le.[Age] like '%' + @Search + '%' 
			OR le.[EmailAddress] like '%' + @Search + '%'
			OR le.[MobileNumber] like '%' + @Search + '%' )
			GROUP BY p.[PatientId]
			) tableSource
		)
		SELECT 
		src.*,
		src.TotalRows
		FROM DATA_CTE src
		WHERE src.row_num between ((@PageNo - 1) * @PageSize + 1 ) 
		and (@PageNo * @PageSize)
		ORDER BY src.row_num 

END
GO
/****** Object:  StoredProcedure [dbo].[usp_patient_update]    Script Date: 6/12/2022 9:32:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ====================================================================
-- Created date: Sept 25, 2020
-- Author: 
-- ====================================================================
CREATE PROCEDURE [dbo].[usp_patient_update]
	@PatientId			NVARCHAR(100),
	@CivilStatusId		NVARCHAR(100),
	@Occupation			NVARCHAR(200),
	@CompleteAddress	NVARCHAR(200),
	@LastUpdatedBy		NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY

		SET NOCOUNT ON;
		-- UPDATE HERE
		UPDATE [dbo].[Patient]
		SET 
		[CivilStatusId] = @CivilStatusId,
		[Occupation] = @Occupation,
		[CompleteAddress] = @CompleteAddress,
		[LastUpdatedBy] = @LastUpdatedBy,
		[LastUpdatedAt] = GETDATE()
		WHERE [PatientId] = @PatientId;

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
/****** Object:  StoredProcedure [dbo].[usp_Reset]    Script Date: 6/12/2022 9:32:05 PM ******/
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
		
		DELETE FROM [dbo].[Patient];
		DELETE FROM [dbo].[Doctor];
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
		
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'Patient';
		Update [dbo].[Sequence] set [LastNumber] = 0 WHERE [TableName] = 'Doctor';
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
/****** Object:  StoredProcedure [dbo].[usp_Reset_Activity]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_sequence_getNextCode]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemtoken_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemtoken_getById]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_changePassword]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_changeUsername]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_createAccount]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_delete]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getByCredentials]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getByID]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getByUsername]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getPaged]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_getTrackerStatus]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuser_update]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuserconfig_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuserconfig_getBySystemUserId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuserconfig_update]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuserverification_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuserverification_getByID]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuserverification_getBySender]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemuserverification_verifyUser]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenumodule_getBySystemWebAdminMenuId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_delete]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_getBySystemUserId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminMenuIdAndSystemWebAdminRoleId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminRoleId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_getBySystemWebAdminRoleIdandSystemWebAdminModuleId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminmenuroles_update]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_delete]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_getByID]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_getPaged]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminrole_update]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_delete]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_getBySystemUserId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_getBySystemWebAdminPrivilegeIdAndSystemWebAdminRoleId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_getBySystemWebAdminRoleId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminroleprivileges_update]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_add]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_delete]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_getBySystemUserId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_getBySystemWebAdminRoleIdAndSystemUserId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_getBySystemWebAdminUserRoleId]    Script Date: 6/12/2022 9:32:05 PM ******/
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
/****** Object:  StoredProcedure [dbo].[usp_systemwebadminuserroles_getPaged]    Script Date: 6/12/2022 9:32:05 PM ******/
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
