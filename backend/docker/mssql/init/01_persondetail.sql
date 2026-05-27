USE [HRIS]
GO

/****** Object:  Table [dbo].[PersonDetail]    Script Date: 27/05/2569 11:37:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PersonDetail](
	[PersonID] [numeric](18, 0) NOT NULL,
	[PersonCode] [varchar](50) NULL,
	[PersonCardID] [varchar](20) NULL,
	[InitialID] [numeric](18, 0) NULL,
	[FnameT] [varchar](50) NULL,
	[LnameT] [varchar](50) NULL,
	[FnameE] [varchar](50) NULL,
	[LnameE] [varchar](50) NULL,
	[Pws] [varchar](20) NULL,
	[Cmb1ID] [numeric](18, 0) NOT NULL,
	[Cmb1Code] [varchar](20) NULL,
	[Cmb1NameT] [varchar](100) NULL,
	[Cmb1NameE] [varchar](100) NULL,
	[Cmb2ID] [numeric](18, 0) NOT NULL,
	[Cmb2Code] [varchar](20) NULL,
	[Cmb2NameT] [varchar](100) NULL,
	[Cmb2NameE] [varchar](100) NULL,
	[Cmb3ID] [numeric](18, 0) NOT NULL,
	[Cmb3Code] [varchar](20) NULL,
	[Cmb3NameT] [varchar](100) NULL,
	[Cmb3NameE] [varchar](100) NULL,
	[Cmb4ID] [numeric](18, 0) NOT NULL,
	[Cmb4Code] [varchar](20) NULL,
	[Cmb4NameT] [varchar](100) NULL,
	[Cmb4NameE] [varchar](100) NULL,
	[Cmb4NameOther] [nvarchar](200) NULL,
	[Cmb5ID] [numeric](18, 0) NOT NULL,
	[Cmb5Code] [varchar](20) NULL,
	[Cmb5NameT] [varchar](100) NULL,
	[Cmb5NameE] [varchar](100) NULL,
	[cmb6ID] [numeric](18, 0) NOT NULL,
	[Cmb6Code] [varchar](20) NULL,
	[Cmb6NameT] [varchar](100) NULL,
	[Cmb6NameE] [varchar](100) NULL,
	[PositionID] [numeric](18, 0) NOT NULL,
	[PositionCode] [varchar](100) NULL,
	[PositionNameT] [varchar](500) NULL,
	[EmpTypeID] [numeric](18, 0) NOT NULL,
	[Age] [tinyint] NULL,
	[BirthDate] [datetime] NULL,
	[StartDate] [datetime] NULL,
	[PassDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[IdentityID] [varchar](50) NULL,
	[SexID] [numeric](18, 0) NOT NULL,
	[LevelID] [numeric](18, 0) NULL,
	[LevelCode] [varchar](20) NULL,
	[LevelT] [varchar](50) NULL,
	[CompanyID] [numeric](18, 0) NOT NULL,
	[Company_Code] [varchar](20) NULL,
	[Company_NameT] [varchar](200) NULL,
	[Company_NameE] [varchar](200) NULL,
	[PersonPic] [image] NULL,
	[TypeProcessSalary] [varchar](1) NULL,
	[Email] [varchar](150) NULL,
	[NickName] [varchar](50) NULL,
	[LastChang_Date] [datetime] NULL,
	[UserName] [nvarchar](100) NULL,
	[JGID] [numeric](18, 0) NULL,
	[JG] [varchar](20) NULL,
 CONSTRAINT [PK_PersonDetail] PRIMARY KEY CLUSTERED 
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_Cmb1ID]  DEFAULT ((1)) FOR [Cmb1ID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_Cmb2ID]  DEFAULT ((1)) FOR [Cmb2ID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_Cmb3ID]  DEFAULT ((1)) FOR [Cmb3ID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_Cmb4ID]  DEFAULT ((1)) FOR [Cmb4ID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_Cmb5ID]  DEFAULT ((1)) FOR [Cmb5ID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_cmb6ID]  DEFAULT ((1)) FOR [cmb6ID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_PositionID]  DEFAULT ((1)) FOR [PositionID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_EmpTypeID]  DEFAULT ((1)) FOR [EmpTypeID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_SexID]  DEFAULT ((1)) FOR [SexID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_CompanyID]  DEFAULT ((1)) FOR [CompanyID]
GO

ALTER TABLE [dbo].[PersonDetail] ADD  CONSTRAINT [DF_PNT_Person_TypeProcessSalary]  DEFAULT ('M') FOR [TypeProcessSalary]
GO

INSERT INTO [dbo].[PersonDetail] (
	[PersonID], [PersonCode], [PersonCardID], [InitialID], [FnameT], [LnameT], [FnameE], [LnameE], [Pws],
	[Cmb1ID], [Cmb1Code], [Cmb1NameT], [Cmb1NameE], [Cmb2ID], [Cmb2Code], [Cmb2NameT], [Cmb2NameE],
	[Cmb3ID], [Cmb3Code], [Cmb3NameT], [Cmb3NameE], [Cmb4ID], [Cmb4Code], [Cmb4NameT], [Cmb4NameE], [Cmb4NameOther],
	[Cmb5ID], [Cmb5Code], [Cmb5NameT], [Cmb5NameE], [cmb6ID], [Cmb6Code], [Cmb6NameT], [Cmb6NameE],
	[PositionID], [PositionCode], [PositionNameT], [EmpTypeID], [Age], [BirthDate], [StartDate], [PassDate], [EndDate],
	[IdentityID], [SexID], [LevelID], [LevelCode], [LevelT], [CompanyID], [Company_Code], [Company_NameT], [Company_NameE],
	[PersonPic], [TypeProcessSalary], [Email], [NickName], [LastChang_Date], [UserName], [JGID], [JG]
)
VALUES
(501, '16810031', '16810031', 4, N'สุพัตรา', N'แก้วประเสริฐ', 'Supattra', 'Kaewprasert', '1031', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 19, 'OP02-04', 'HF - Tuna Trimming', 'HF-Tuna', 1, '-', '-', '-', NULL, 90, '06', '2401', 'TUNA P/D , P/D - Freshfish , P/D - Packing , P/D Salmon Raw Packing', 250, '226', 'Staff - HF - Tuna Trimming', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1980-02-11 00:00:00.000', '2001-03-05 00:00:00.000', '2001-07-02 00:00:00.000', NULL, '3990200145831', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-04-28 14:46:15.217', N'KYS_A', 14, 'Asst.Foreperson'),
(502, '16810045', '16810045', 4, N'มาลินี', N'ทองนาค', 'Malinee', 'Thongnak', '1045', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 47, 'OP02-02', 'HF - Tuna Front End ', 'Freshfish', 1, '-', '-', '-', NULL, 90, '06', '2401', 'TUNA P/D , P/D - Freshfish , P/D - Packing , P/D Salmon Raw Packing', 256, '232', 'Staff - HF - Tuna Front End ', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1979-09-18 00:00:00.000', '2000-06-12 00:00:00.000', '2000-10-09 00:00:00.000', NULL, '3990600217342', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-04-28 14:46:16.040', N'KYS_A', 14, 'Asst.Foreperson'),
(503, '16710028', '16710028', 4, N'วรรณา', N'ศรีสุข', 'Wanna', 'Srisuk', '0028', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 19, 'OP02-04', 'HF - Tuna Trimming', 'HF-Tuna', 1, '-', '-', '-', NULL, 90, '06', '2401', 'TUNA P/D , P/D - Freshfish , P/D - Packing , P/D Salmon Raw Packing', 250, '226', 'Staff - HF - Tuna Trimming', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1978-12-03 00:00:00.000', '1999-11-22 00:00:00.000', '2000-03-20 00:00:00.000', NULL, '3990900441287', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-03-11 11:53:41.370', N'KYS_ST', 14, 'Asst.Foreperson'),
(504, '16610019', '16610019', 4, N'จันทร์เพ็ญ', N'บุญมา', 'Janpen', 'Boonma', '0619', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 19, 'OP02-04', 'HF - Tuna Trimming', 'HF-Tuna', 1, '-', '-', '-', NULL, 90, '06', '2401', 'TUNA P/D , P/D - Freshfish , P/D - Packing , P/D Salmon Raw Packing', 250, '226', 'Staff - HF - Tuna Trimming', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1976-06-25 00:00:00.000', '1998-08-17 00:00:00.000', '1998-12-14 00:00:00.000', NULL, '3991100523194', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-04-28 14:46:21.107', N'KYS_A', 14, 'Asst.Foreperson'),
(505, '16510052', '16510052', 4, N'รัตนา', N'มีชัย', 'Rattana', 'Meechai', '9252', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 47, 'OP02-02', 'HF - Tuna Front End ', 'Freshfish', 1, '-', '-', '-', NULL, 90, '06', '2401', 'TUNA P/D , P/D - Freshfish , P/D - Packing , P/D Salmon Raw Packing', 256, '232', 'Staff - HF - Tuna Front End ', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1982-04-14 00:00:00.000', '2003-02-10 00:00:00.000', '2003-06-09 00:00:00.000', NULL, '3991400678456', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-04-28 14:46:18.980', N'KYS_A', 14, 'Asst.Foreperson'),
(506, '16410017', '16410017', 4, N'เบญจมาศ', N'อินทร์ทอง', 'Benjamas', 'Inthong', '0017', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 19, 'OP02-04', 'HF - Tuna Trimming', 'HF-Tuna', 1, '-', '-', '-', NULL, 90, '06', '2401', 'TUNA P/D , P/D - Freshfish , P/D - Packing , P/D Salmon Raw Packing', 250, '226', 'Staff - HF - Tuna Trimming', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1977-03-30 00:00:00.000', '1997-10-06 00:00:00.000', '1998-02-02 00:00:00.000', NULL, '3991700184520', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-04-28 14:46:23.507', N'KYS_A', 14, 'Asst.Foreperson'),
(507, '16310064', '16310064', 4, N'นฤมล', N'แสงแก้ว', 'Narumon', 'Saengkaew', '1064', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 43, 'OP02-05', 'HF - VAP Can', NULL, 1, '-', '-', '-', NULL, 105, '08', '2403', 'P/D - HF, Vap & Pouch HF , P/D VAP Pouch', 261, '237', 'Asst.FL-VAP', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1984-01-09 00:00:00.000', '2011-05-16 00:00:00.000', '2011-09-12 00:00:00.000', NULL, '5990200763185', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-04-28 14:46:25.743', N'KYS_A', 14, 'Asst.Foreperson'),
(508, '16210039', '16210039', 4, N'ศิริลักษณ์', N'คำจันทร์', 'Sirilak', 'Khamchan', '0039', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 19, 'OP02-04', 'HF - Tuna Trimming', 'HF-Tuna', 1, '-', '-', '-', NULL, 90, '06', '2401', 'TUNA P/D , P/D - Freshfish , P/D - Packing , P/D Salmon Raw Packing', 250, '226', 'Staff - HF - Tuna Trimming', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1975-08-21 00:00:00.000', '1996-12-02 00:00:00.000', '1997-03-31 00:00:00.000', NULL, '3992100359106', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-04-28 14:46:12.237', N'KYS_A', 14, 'Asst.Foreperson'),
(509, '16110073', '16110073', 4, N'อำพร', N'จันทร์หอม', 'Amporn', 'Janhom', '2573', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 47, 'OP02-02', 'HF - Tuna Front End ', 'Freshfish', 1, '-', '-', '-', NULL, 90, '06', '2401', 'TUNA P/D , P/D - Freshfish , P/D - Packing , P/D Salmon Raw Packing', 256, '232', 'Staff - HF - Tuna Front End ', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1981-10-07 00:00:00.000', '2002-01-14 00:00:00.000', '2002-05-13 00:00:00.000', NULL, '3992400916372', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-04-28 14:46:10.057', N'KYS_A', 14, 'Asst.Foreperson'),
(510, '16010088', '16010088', 4, N'กัลยา', N'พรหมรักษ์', 'Kanya', 'Promrak', '0088', 2, 'PFI01', 'Operations', NULL, 2, 'OP02', 'Production', 'Operations', 43, 'OP02-05', 'HF - VAP Can', NULL, 1, '-', '-', '-', NULL, 105, '08', '2403', 'P/D - HF, Vap & Pouch HF , P/D VAP Pouch', 261, '237', 'Asst.FL-VAP', NULL, 11083, 'PD-07', 'Assistant Foreperson', 4, NULL, '1983-05-28 00:00:00.000', '2012-09-03 00:00:00.000', '2012-12-31 00:00:00.000', NULL, '5992600482059', 2, 6, '06', 'Staff', 2, 'PFT', N'บริษัท พัทยาฟู้ดอินดัสตรี จำกัด', 'PATAYA FOOD INDUSTRIES LTD.', 0xFFD8FFE000104A46494600010101006000600000FFD9, 'M', NULL, NULL, '2026-03-11 11:53:39.370', N'KYS_ST', 14, 'Asst.Foreperson');
GO


