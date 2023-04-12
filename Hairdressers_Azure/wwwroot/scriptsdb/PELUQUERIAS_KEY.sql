CREATE TABLE ADMINS(
	hairdresser_id int NOT NULL,
	user_id int NOT NULL,
	role tinyint NOT NULL,
 CONSTRAINT PK_ADMINS PRIMARY KEY CLUSTERED 
(
	hairdresser_id ASC,
	user_id ASC
))

GO
/****** Object:  Table APPOINTMENT_SERVICES    Script Date: 27/03/2023 13:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE APPOINTMENT_SERVICES(
	appointment_id int NOT NULL,
	service_id int NOT NULL,
 CONSTRAINT PK_APPOINTMENT_SERVICES PRIMARY KEY CLUSTERED 
(
	appointment_id ASC,
	service_id ASC
))

GO
/****** Object:  Table APPOINTMENTS    Script Date: 27/03/2023 13:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE APPOINTMENTS(
	appointment_id int NOT NULL,
	user_id int NOT NULL,
	hairdresser_id int NOT NULL,
	date date NOT NULL,
	time time(0) NOT NULL,
 CONSTRAINT PK_APPOINTMENTS PRIMARY KEY CLUSTERED 
(
	appointment_id ASC
))

GO
/****** Object:  Table FILES    Script Date: 27/03/2023 13:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE FILES(
	hairdresser_id int NOT NULL,
	hairdresser_file_item tinyint NOT NULL,
	path_name nvarchar(50) NOT NULL,
	alternate_text nvarchar(50) NULL,
	logo_image bit NULL,
 CONSTRAINT PK_FILES PRIMARY KEY CLUSTERED 
(
	hairdresser_id ASC,
	hairdresser_file_item ASC
))

GO
/****** Object:  Table HAIRDRESSERS    Script Date: 27/03/2023 13:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE HAIRDRESSERS(
	hairdresser_id int NOT NULL,
	name nvarchar(60) NOT NULL,
	number_phone nvarchar(20) NULL,
	address nvarchar(200) NOT NULL,
	postal_code int NOT NULL,
	token nvarchar(100) NULL,
 CONSTRAINT PK_HAIRDRESSERS PRIMARY KEY CLUSTERED 
(
	hairdresser_id ASC
))

GO
/****** Object:  Table SCHEDULE_ROWS    Script Date: 27/03/2023 13:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE SCHEDULE_ROWS(
	schedule_row_id int NOT NULL,
	schedule_id int NOT NULL,
	start_time time(0) NOT NULL,
	end_time time(0) NOT NULL,
	monday bit NOT NULL,
	tuesday bit NOT NULL,
	wednesday bit NOT NULL,
	thursday bit NOT NULL,
	friday bit NOT NULL,
	saturday bit NOT NULL,
	sunday bit NOT NULL,
 CONSTRAINT PK_SCHEDULE_ROWS PRIMARY KEY CLUSTERED 
(
	schedule_row_id ASC
))

GO
/****** Object:  Table SCHEDULES    Script Date: 27/03/2023 13:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE SCHEDULES(
	schedule_id int NOT NULL,
	hairdresser_id int NOT NULL,
	name nvarchar(50) NOT NULL,
	active bit NOT NULL,
 CONSTRAINT PK_SCHEDULES PRIMARY KEY CLUSTERED 
(
	schedule_id ASC
))

GO
/****** Object:  Table SERVICES    Script Date: 27/03/2023 13:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE SERVICES(
	service_id int NOT NULL,
	hairdresser_id int NOT NULL,
	name nvarchar(50) NOT NULL,
	price decimal(5, 2) NOT NULL,
	daprox tinyint NOT NULL,
 CONSTRAINT PK_SERVICES PRIMARY KEY CLUSTERED 
(
	service_id ASC
))

GO
/****** Object:  Table USERS    Script Date: 27/03/2023 13:44:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE USERS(
	user_id int NOT NULL,
	password varbinary(max) NOT NULL,
	password_read nvarchar(50) NOT NULL,
	salt nvarchar(50) NOT NULL,
	email nvarchar(50) NOT NULL,
	email_validated bit NOT NULL,
	last_name nvarchar(50) NULL,
	name nvarchar(50) NOT NULL,
	number_phone nvarchar(20) NULL,
	temp_token nvarchar(100) NULL,
 CONSTRAINT PK_USERS PRIMARY KEY CLUSTERED 
(
	user_id ASC
))

GO
INSERT ADMINS (hairdresser_id, user_id, role) VALUES (1, 1, 1)
INSERT ADMINS (hairdresser_id, user_id, role) VALUES (2, 1, 1)
GO
INSERT HAIRDRESSERS (hairdresser_id, name, number_phone, address, postal_code, token) VALUES (1, N'Peluqueria Janis', N'917 85 62 44', N'C. Ibor, 19', 28053, N'6mPARwFWCyEmpr3M5wDuE3nErP0tQbjkxR4IwusJ4F0vn64V9x')
INSERT HAIRDRESSERS (hairdresser_id, name, number_phone, address, postal_code, token) VALUES (2, N'El Haddadi - Peluquería Caballeros', N'918 51 34 56', N'C/. Travesía de la Venta 1 Local Nº 2', 28400, N'UOQQfd1fQ1hl4vUrg4WfGemFgX9dCPIJfuYTxJ9tOWEN62efME')
GO
INSERT SCHEDULE_ROWS (schedule_row_id, schedule_id, start_time, end_time, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (1, 1, CAST(N'08:30:00' AS Time), CAST(N'14:30:00' AS Time), 1, 1, 1, 1, 1, 0, 0)
INSERT SCHEDULE_ROWS (schedule_row_id, schedule_id, start_time, end_time, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (2, 1, CAST(N'17:00:00' AS Time), CAST(N'21:00:00' AS Time), 1, 1, 1, 1, 1, 0, 0)
INSERT SCHEDULE_ROWS (schedule_row_id, schedule_id, start_time, end_time, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (3, 1, CAST(N'09:00:00' AS Time), CAST(N'12:00:00' AS Time), 0, 0, 0, 0, 0, 1, 1)
INSERT SCHEDULE_ROWS (schedule_row_id, schedule_id, start_time, end_time, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (4, 2, CAST(N'08:30:00' AS Time), CAST(N'14:30:00' AS Time), 1, 1, 1, 1, 1, 0, 0)
INSERT SCHEDULE_ROWS (schedule_row_id, schedule_id, start_time, end_time, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (5, 2, CAST(N'17:00:00' AS Time), CAST(N'21:00:00' AS Time), 1, 1, 1, 1, 0, 0, 0)
INSERT SCHEDULE_ROWS (schedule_row_id, schedule_id, start_time, end_time, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (6, 2, CAST(N'17:00:00' AS Time), CAST(N'22:30:00' AS Time), 0, 0, 0, 0, 1, 0, 0)
INSERT SCHEDULE_ROWS (schedule_row_id, schedule_id, start_time, end_time, monday, tuesday, wednesday, thursday, friday, saturday, sunday) VALUES (7, 2, CAST(N'10:00:00' AS Time), CAST(N'12:00:00' AS Time), 0, 0, 0, 0, 0, 1, 1)
GO
INSERT SCHEDULES (schedule_id, hairdresser_id, name, active) VALUES (1, 1, N'Horario General', 1)
INSERT SCHEDULES (schedule_id, hairdresser_id, name, active) VALUES (2, 2, N'Horario General', 1)
GO
INSERT SERVICES (service_id, hairdresser_id, name, price, daprox) VALUES (1, 1, N'Corte de Caballero (simple)', CAST(9.00 AS Decimal(5, 2)), 30)
INSERT SERVICES (service_id, hairdresser_id, name, price, daprox) VALUES (2, 1, N'Corte de Caballero (complejo)', CAST(10.00 AS Decimal(5, 2)), 40)
INSERT SERVICES (service_id, hairdresser_id, name, price, daprox) VALUES (3, 1, N'Teñido y marcado', CAST(45.00 AS Decimal(5, 2)), 120)
INSERT SERVICES (service_id, hairdresser_id, name, price, daprox) VALUES (4, 1, N'Afeitado completo', CAST(25.00 AS Decimal(5, 2)), 150)
GO
INSERT USERS (user_id, password, password_read, salt, email, email_validated, last_name, name, number_phone, temp_token) VALUES (1, 0xE0AB21FC7DBB3355FB68FF55E5539869ABE5CCB49BF95550227493A5B213BF9F50C2EBAD7FE7B6488990FE5A363D1CCF7D5EE5BDEFE19378D1AF24F5BD3AC148, N'123', N'¢êò{È²`|o?&RÈqãAín?¦h¿uË8n_DÏ;°HfÆÁmo	û,a¢', N'paco.garcia.serrano@tajamar365.com', 0, N'García Serrano', N'Paco', N'999 888 777', N'')
INSERT USERS (user_id, password, password_read, salt, email, email_validated, last_name, name, number_phone, temp_token) VALUES (2, 0x94A85F4B1FC5D2B8C6AE06285838ECB0419F4D6774F798F77DBF62ADFA8BB7DDC787A0A6A503948F38A5AE6F6C88A42DAD5783CFBD9588F7FF00CFB50F4383D9, N'123', N'ýqØMí DþùE?¦¬kâ&ïÓ«Õ-?8døJßaÈ¢¡¸', N'cutandgo.app@gmail.com', 0, N'Cut&Go', N'Administrador', N'628 638 560', N'')
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index Unique_Email    Script Date: 27/03/2023 13:44:18 ******/
ALTER TABLE USERS ADD  CONSTRAINT Unique_Email UNIQUE NONCLUSTERED 
(
	email ASC
)
GO
ALTER TABLE USERS ADD  CONSTRAINT DF_USERS_password_read  DEFAULT (N'MCSD2023') FOR password_read
GO
ALTER TABLE USERS ADD  CONSTRAINT DF_USERS_salt  DEFAULT (N'tajamar') FOR salt
GO
ALTER TABLE USERS ADD  CONSTRAINT DF_USERS_email_validated  DEFAULT ((0)) FOR email_validated
GO
ALTER TABLE ADMINS  WITH CHECK ADD  CONSTRAINT FK_ADMINS_HAIRDRESSERS1 FOREIGN KEY(hairdresser_id)
REFERENCES HAIRDRESSERS (hairdresser_id)
GO
ALTER TABLE ADMINS CHECK CONSTRAINT FK_ADMINS_HAIRDRESSERS1
GO
ALTER TABLE ADMINS  WITH CHECK ADD  CONSTRAINT FK_ADMINS_USERS FOREIGN KEY(user_id)
REFERENCES USERS (user_id)
GO
ALTER TABLE ADMINS CHECK CONSTRAINT FK_ADMINS_USERS
GO
ALTER TABLE APPOINTMENT_SERVICES  WITH CHECK ADD  CONSTRAINT FK_APPOINTMENT_SERVICES_APPOINTMENTS FOREIGN KEY(appointment_id)
REFERENCES APPOINTMENTS (appointment_id)
GO
ALTER TABLE APPOINTMENT_SERVICES CHECK CONSTRAINT FK_APPOINTMENT_SERVICES_APPOINTMENTS
GO
ALTER TABLE APPOINTMENT_SERVICES  WITH CHECK ADD  CONSTRAINT FK_APPOINTMENT_SERVICES_SERVICES1 FOREIGN KEY(service_id)
REFERENCES SERVICES (service_id)
GO
ALTER TABLE APPOINTMENT_SERVICES CHECK CONSTRAINT FK_APPOINTMENT_SERVICES_SERVICES1
GO
ALTER TABLE APPOINTMENTS  WITH CHECK ADD  CONSTRAINT FK_APPOINTMENTS_HAIRDRESSERS FOREIGN KEY(hairdresser_id)
REFERENCES HAIRDRESSERS (hairdresser_id)
GO
ALTER TABLE APPOINTMENTS CHECK CONSTRAINT FK_APPOINTMENTS_HAIRDRESSERS
GO
ALTER TABLE APPOINTMENTS  WITH CHECK ADD  CONSTRAINT FK_APPOINTMENTS_USERS FOREIGN KEY(user_id)
REFERENCES USERS (user_id)
GO
ALTER TABLE APPOINTMENTS CHECK CONSTRAINT FK_APPOINTMENTS_USERS
GO
ALTER TABLE FILES  WITH CHECK ADD  CONSTRAINT FK_FILES_HAIRDRESSERS FOREIGN KEY(hairdresser_id)
REFERENCES HAIRDRESSERS (hairdresser_id)
GO
ALTER TABLE FILES CHECK CONSTRAINT FK_FILES_HAIRDRESSERS
GO
ALTER TABLE SCHEDULE_ROWS  WITH CHECK ADD  CONSTRAINT FK_SCHEDULE_ROWS_SCHEDULES FOREIGN KEY(schedule_id)
REFERENCES SCHEDULES (schedule_id)
GO
ALTER TABLE SCHEDULE_ROWS CHECK CONSTRAINT FK_SCHEDULE_ROWS_SCHEDULES
GO
ALTER TABLE SCHEDULES  WITH CHECK ADD  CONSTRAINT FK_SCHEDULES_HAIRDRESSERS FOREIGN KEY(hairdresser_id)
REFERENCES HAIRDRESSERS (hairdresser_id)
GO
ALTER TABLE SCHEDULES CHECK CONSTRAINT FK_SCHEDULES_HAIRDRESSERS
GO
ALTER TABLE SERVICES  WITH CHECK ADD  CONSTRAINT FK_SERVICES_HAIRDRESSERS FOREIGN KEY(hairdresser_id)
REFERENCES HAIRDRESSERS (hairdresser_id)
GO
ALTER TABLE SERVICES CHECK CONSTRAINT FK_SERVICES_HAIRDRESSERS
GO
/****** Object:  StoredProcedure SP_ASSIGN_TOKEN    Script Date: 27/03/2023 13:44:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE SP_ASSIGN_TOKEN(@USER_ID INT, @TOKEN NVARCHAR(100))
AS
	UPDATE USERS SET TEMP_TOKEN = @TOKEN WHERE USER_ID = @USER_ID;
GO
/****** Object:  StoredProcedure SP_COMPARE_ROLE    Script Date: 27/03/2023 13:44:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
    CREATE PROCEDURE SP_COMPARE_ROLE(@HAIRDRESSER_ID INT, @USER_ID1 INT, @USER_ID2 INT, @RES BIT OUT)
    AS
	    DECLARE @ROLE_1 INT, @ROLE_2 INT;
	    SELECT @ROLE_1 = ADMINS.role FROM ADMINS WHERE ADMINS.user_id = @USER_ID1 AND ADMINS.hairdresser_id = @HAIRDRESSER_ID;
	    SELECT @ROLE_2 = ADMINS.role FROM ADMINS WHERE ADMINS.user_id = @USER_ID2 AND ADMINS.hairdresser_id = @HAIRDRESSER_ID;
	    IF(@ROLE_1 <= @ROLE_2)
		    SET @RES = 1;
	    ELSE
		    SET @RES = 0;
GO
/****** Object:  StoredProcedure SP_GET_HAIRDRESSER_EMAILS    Script Date: 27/03/2023 13:44:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE SP_GET_HAIRDRESSER_EMAILS (@HAIRDRESSER_ID INT)
AS
	SELECT EMAIL 
	FROM USERS INNER JOIN ADMINS ON ADMINS.user_id = USERS.user_id
	WHERE ADMINS.hairdresser_id = 1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Propietario = 1, Gerente = 2, Supervisor = 3, Empleado = 4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ADMINS', @level2type=N'COLUMN',@level2name=N'role'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Approximate duration of the service' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SERVICES', @level2type=N'COLUMN',@level2name=N'daprox'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Password para Paco' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'USERS', @level2type=N'COLUMN',@level2name=N'password_read'
GO
