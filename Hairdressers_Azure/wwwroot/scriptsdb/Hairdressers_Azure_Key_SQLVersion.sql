CREATE TABLE [dbo].[ADMINS](
	[hairdresser_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
	[role] [tinyint] NOT NULL,
 CONSTRAINT [PK_ADMINS] PRIMARY KEY CLUSTERED 
(
	[hairdresser_id] ASC,
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[APPOINTMENT_SERVICES]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[APPOINTMENT_SERVICES](
	[appointment_id] [int] NOT NULL,
	[service_id] [int] NOT NULL,
 CONSTRAINT [PK_APPOINTMENT_SERVICES] PRIMARY KEY CLUSTERED 
(
	[appointment_id] ASC,
	[service_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[APPOINTMENTS]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[APPOINTMENTS](
	[appointment_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
	[hairdresser_id] [int] NOT NULL,
	[date] [date] NOT NULL,
	[time] [time](0) NOT NULL,
 CONSTRAINT [PK_APPOINTMENTS] PRIMARY KEY CLUSTERED 
(
	[appointment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FILES]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FILES](
	[hairdresser_id] [int] NOT NULL,
	[hairdresser_file_item] [tinyint] NOT NULL,
	[path_name] [nvarchar](50) NOT NULL,
	[alternate_text] [nvarchar](50) NULL,
	[logo_image] [bit] NULL,
 CONSTRAINT [PK_FILES] PRIMARY KEY CLUSTERED 
(
	[hairdresser_id] ASC,
	[hairdresser_file_item] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HAIRDRESSERS]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HAIRDRESSERS](
	[hairdresser_id] [int] NOT NULL,
	[name] [nvarchar](60) NOT NULL,
	[number_phone] [nvarchar](20) NULL,
	[address] [nvarchar](200) NOT NULL,
	[postal_code] [int] NOT NULL,
	[token] [nvarchar](100) NULL,
 CONSTRAINT [PK_HAIRDRESSERS] PRIMARY KEY CLUSTERED 
(
	[hairdresser_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SCHEDULE_ROWS]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SCHEDULE_ROWS](
	[schedule_row_id] [int] NOT NULL,
	[schedule_id] [int] NOT NULL,
	[start_time] [time](0) NOT NULL,
	[end_time] [time](0) NOT NULL,
	[monday] [bit] NOT NULL,
	[tuesday] [bit] NOT NULL,
	[wednesday] [bit] NOT NULL,
	[thursday] [bit] NOT NULL,
	[friday] [bit] NOT NULL,
	[saturday] [bit] NOT NULL,
	[sunday] [bit] NOT NULL,
 CONSTRAINT [PK_SCHEDULE_ROWS] PRIMARY KEY CLUSTERED 
(
	[schedule_row_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SCHEDULES]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SCHEDULES](
	[schedule_id] [int] NOT NULL,
	[hairdresser_id] [int] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK_SCHEDULES] PRIMARY KEY CLUSTERED 
(
	[schedule_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SERVICES]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SERVICES](
	[service_id] [int] NOT NULL,
	[hairdresser_id] [int] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[price] [decimal](5, 2) NOT NULL,
	[daprox] [tinyint] NOT NULL,
 CONSTRAINT [PK_SERVICES] PRIMARY KEY CLUSTERED 
(
	[service_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[USERS]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[USERS](
	[user_id] [int] NOT NULL,
	[password] [varbinary](max) NOT NULL,
	[password_read] [nvarchar](50) NOT NULL,
	[salt] [nvarchar](50) NOT NULL,
	[email] [nvarchar](50) NOT NULL,
	[email_validated] [bit] NOT NULL,
	[last_name] [nvarchar](50) NULL,
	[name] [nvarchar](50) NOT NULL,
	[number_phone] [nvarchar](20) NULL,
	[temp_token] [nvarchar](100) NULL,
 CONSTRAINT [PK_USERS] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[HAIRDRESSERS] ([hairdresser_id], [name], [number_phone], [address], [postal_code], [token]) VALUES (1, N'Peluqueria Janis', N'917 85 62 44', N'C. Ibor, 19', 28053, N'6mPARwFWCyEmpr3M5wDuE3nErP0tQbjkxR4IwusJ4F0vn64V9x')
INSERT [dbo].[HAIRDRESSERS] ([hairdresser_id], [name], [number_phone], [address], [postal_code], [token]) VALUES (2, N'El Haddadi - Peluquería Caballeros', N'918 51 34 56', N'C/. Travesía de la Venta 1 Local Nº 2', 28400, N'UOQQfd1fQ1hl4vUrg4WfGemFgX9dCPIJfuYTxJ9tOWEN62efME')
GO
INSERT [dbo].[SCHEDULE_ROWS] ([schedule_row_id], [schedule_id], [start_time], [end_time], [monday], [tuesday], [wednesday], [thursday], [friday], [saturday], [sunday]) VALUES (1, 1, CAST(N'08:30:00' AS Time), CAST(N'14:30:00' AS Time), 1, 1, 1, 1, 1, 0, 0)
INSERT [dbo].[SCHEDULE_ROWS] ([schedule_row_id], [schedule_id], [start_time], [end_time], [monday], [tuesday], [wednesday], [thursday], [friday], [saturday], [sunday]) VALUES (2, 1, CAST(N'17:00:00' AS Time), CAST(N'21:00:00' AS Time), 1, 1, 1, 1, 1, 0, 0)
INSERT [dbo].[SCHEDULE_ROWS] ([schedule_row_id], [schedule_id], [start_time], [end_time], [monday], [tuesday], [wednesday], [thursday], [friday], [saturday], [sunday]) VALUES (3, 1, CAST(N'09:00:00' AS Time), CAST(N'12:00:00' AS Time), 0, 0, 0, 0, 0, 1, 1)
INSERT [dbo].[SCHEDULE_ROWS] ([schedule_row_id], [schedule_id], [start_time], [end_time], [monday], [tuesday], [wednesday], [thursday], [friday], [saturday], [sunday]) VALUES (4, 2, CAST(N'08:30:00' AS Time), CAST(N'14:30:00' AS Time), 1, 1, 1, 1, 1, 0, 0)
INSERT [dbo].[SCHEDULE_ROWS] ([schedule_row_id], [schedule_id], [start_time], [end_time], [monday], [tuesday], [wednesday], [thursday], [friday], [saturday], [sunday]) VALUES (5, 2, CAST(N'17:00:00' AS Time), CAST(N'21:00:00' AS Time), 1, 1, 1, 1, 0, 0, 0)
INSERT [dbo].[SCHEDULE_ROWS] ([schedule_row_id], [schedule_id], [start_time], [end_time], [monday], [tuesday], [wednesday], [thursday], [friday], [saturday], [sunday]) VALUES (6, 2, CAST(N'17:00:00' AS Time), CAST(N'22:30:00' AS Time), 0, 0, 0, 0, 1, 0, 0)
INSERT [dbo].[SCHEDULE_ROWS] ([schedule_row_id], [schedule_id], [start_time], [end_time], [monday], [tuesday], [wednesday], [thursday], [friday], [saturday], [sunday]) VALUES (7, 2, CAST(N'10:00:00' AS Time), CAST(N'12:00:00' AS Time), 0, 0, 0, 0, 0, 1, 1)
GO
INSERT [dbo].[SCHEDULES] ([schedule_id], [hairdresser_id], [name], [active]) VALUES (1, 1, N'Horario General', 1)
INSERT [dbo].[SCHEDULES] ([schedule_id], [hairdresser_id], [name], [active]) VALUES (2, 2, N'Horario General', 1)
GO
INSERT [dbo].[SERVICES] ([service_id], [hairdresser_id], [name], [price], [daprox]) VALUES (1, 1, N'Corte de Caballero (simple)', CAST(9.00 AS Decimal(5, 2)), 30)
INSERT [dbo].[SERVICES] ([service_id], [hairdresser_id], [name], [price], [daprox]) VALUES (2, 1, N'Corte de Caballero (complejo)', CAST(10.00 AS Decimal(5, 2)), 40)
INSERT [dbo].[SERVICES] ([service_id], [hairdresser_id], [name], [price], [daprox]) VALUES (3, 1, N'Teñido y marcado', CAST(45.00 AS Decimal(5, 2)), 120)
INSERT [dbo].[SERVICES] ([service_id], [hairdresser_id], [name], [price], [daprox]) VALUES (4, 1, N'Afeitado completo', CAST(25.00 AS Decimal(5, 2)), 150)
GO
INSERT [dbo].[USERS] ([user_id], [password], [password_read], [salt], [email], [email_validated], [last_name], [name], [number_phone], [temp_token]) VALUES (1, 0xB4F0B6C3BF8359747E7B9023F61E40A9D25AD11C91CEDF3FC4A98703CDA002C9646D8C46DFEF9E0D48939062EAC9F159E0278969C210198F77CBEB415875E72B, N'123', N'Ât£eÆJç·~"@m¡UñC²¦ÃaåW"¦Kpðw×$!ÚÐb<Ã#õöy', N'giovannyandresch@gmail.com', 0, N'Cortés Hernández', N'Giovanny Andrés', N'628 638 560', N'')
GO
/****** Object:  StoredProcedure [dbo].[SP_ASSIGN_TOKEN]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

    CREATE PROCEDURE [dbo].[SP_ASSIGN_TOKEN](@USER_ID INT, @TOKEN NVARCHAR(100))
    AS
	    UPDATE USERS SET TEMP_TOKEN = @TOKEN WHERE USER_ID = @USER_ID;
GO
/****** Object:  StoredProcedure [dbo].[SP_COMPARE_ROLE]    Script Date: 12/04/2023 23:36:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
    CREATE PROCEDURE [dbo].[SP_COMPARE_ROLE](@HAIRDRESSER_ID INT, @USER_ID1 INT, @USER_ID2 INT, @RES BIT OUT)
    AS
	    DECLARE @ROLE_1 INT, @ROLE_2 INT;
	    SELECT @ROLE_1 = ADMINS.role FROM ADMINS WHERE ADMINS.[user_id] = @USER_ID1 AND ADMINS.hairdresser_id = @HAIRDRESSER_ID;
	    SELECT @ROLE_2 = ADMINS.role FROM ADMINS WHERE ADMINS.[user_id] = @USER_ID2 AND ADMINS.hairdresser_id = @HAIRDRESSER_ID;
	    IF(@ROLE_1 <= @ROLE_2)
		    SET @RES = 1;
	    ELSE
		    SET @RES = 0;
GO
USE [master]
GO
ALTER DATABASE [PELUQUERIAS] SET  READ_WRITE 
GO
