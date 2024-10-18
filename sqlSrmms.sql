
USE [SRMMS]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[acc_id] [int] IDENTITY(1,1) NOT NULL,
	[cus_id] [int] NOT NULL,
	[emp_id] [int] NOT NULL,
 CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED 
(
	[acc_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Booking_table]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking_table](
	[book_id] [int] IDENTITY(1,1) NOT NULL,
	[table_id] [int] NULL,
	[acc_id] [int] NULL,
	[time_booking] [date] NULL,
	[time_out] [date] NULL,
 CONSTRAINT [PK_Booking_table] PRIMARY KEY CLUSTERED 
(
	[book_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[cat_id] [int] IDENTITY(1,1) NOT NULL,
	[cat_name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[cat_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Combo]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Combo](
	[combo_id] [int] IDENTITY(1,1) NOT NULL,
	[combo_name] [nvarchar](200) NOT NULL,
	[combo_discription] [nvarchar](max) NULL,
	[combo_img] [nvarchar](max) NULL,
	[combo_money] [money] NOT NULL,
	[combo_status] [bit] NOT NULL,
 CONSTRAINT [PK_Combo] PRIMARY KEY CLUSTERED 
(
	[combo_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Combo_Detail]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Combo_Detail](
	[combo_id] [int] NOT NULL,
	[pro_id] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[cus_id] [int] IDENTITY(1,1) NOT NULL,
	[cus_fullname] [nvarchar](50) NOT NULL,
	[cus_phone] [int] NOT NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[cus_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Discount_code]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discount_code](
	[code_id] [int] NOT NULL,
	[order_id] [int] NULL,
	[code_detail] [nvarchar](250) NULL,
	[discount_value] [float] NULL,
	[start_date] [date] NULL,
	[end_date] [date] NULL,
	[status] [bit] NULL,
 CONSTRAINT [PK_Discount_code] PRIMARY KEY CLUSTERED 
(
	[code_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[emp_id] [int] IDENTITY(1,1) NOT NULL,
	[emp_first_name] [nvarchar](50) NOT NULL,
	[emp_last_name] [nvarchar](50) NOT NULL,
	[emp_dob] [date] NULL,
	[emp_gender] [bit] NOT NULL,
	[emp_phoneNumber] [int] NOT NULL,
	[emp_email] [nvarchar](50) NOT NULL,
	[emp_password] [nvarchar](30) NOT NULL,
	[emp_address] [nvarchar](150) NULL,
	[emp_ward] [nvarchar](50) NULL,
	[emp_startDate] [date] NOT NULL,
	[emp_endDate] [date] NULL,
	[emp_role_id] [int] NOT NULL,
	[emp_status] [bit] NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[emp_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[pro_id] [int] NOT NULL,
	[feedback] [nvarchar](max) NOT NULL,
	[ratestar] [int] NOT NULL,
	[acc_id] [int] NOT NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[order_id] [int] IDENTITY(1,1) NOT NULL,
	[order_date] [date] NOT NULL,
	[acc_id] [int] NOT NULL,
	[totalMoney] [money] NOT NULL,
	[order_status_id] [int] NOT NULL,
	[staus] [bit] NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[order_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order_Details]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order_Details](
	[order_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[order_id] [int] NOT NULL,
	[pro_id] [int] NULL,
	[quantiity] [int] NOT NULL,
	[price] [money] NOT NULL,
 CONSTRAINT [PK_Order_Details] PRIMARY KEY CLUSTERED 
(
	[order_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[pro_id] [int] IDENTITY(1,1) NOT NULL,
	[pro_name] [nvarchar](200) NOT NULL,
	[pro_discription] [nvarchar](max) NOT NULL,
	[pro_warning] [nvarchar](max) NOT NULL,
	[pro_price] [money] NOT NULL,
	[cat_id] [int] NOT NULL,
	[ing_id] [int] NULL,
	[pro_img] [nvarchar](max) NOT NULL,
	[pro_calories] [nvarchar](250) NOT NULL,
	[pro_cooking_time] [int] NOT NULL,
	[pro_status] [bit] NOT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[pro_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Restaurant_Information]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Restaurant_Information](
	[res_name] [nvarchar](max) NOT NULL,
	[res_adress] [nvarchar](200) NOT NULL,
	[res_facebook] [nvarchar](max) NOT NULL,
	[res_email] [nvarchar](max) NOT NULL,
	[res_phone] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[role_id] [int] IDENTITY(1,1) NOT NULL,
	[role_name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](150) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Table]    Script Date: 10/15/2024 12:03:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Table](
	[table_id] [int] NOT NULL,
	[table_name] [nvarchar](50) NOT NULL,
	[QR_code] [nvarchar](50) NOT NULL,
	[status] [bit] NOT NULL,
 CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED 
(
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Employees] ON 

INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (2, N'Giang', N'Nguyen', CAST(N'2002-02-12' AS Date), 1, 127263273, N'giang@gmail.com', N'giang123', N'HY', N'Hy', CAST(N'2024-12-01' AS Date), CAST(N'2024-12-02' AS Date), 1, 0)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (4, N'Ha 123', N'Nguyen', CAST(N'2002-10-05' AS Date), 1, 182617218, N'ha@gmail.com', N'ha123', N'HY', N'Hy', CAST(N'2024-11-25' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (6, N'Nguyen', N'Tran', CAST(N'2002-12-02' AS Date), 1, 183837, N'nguyen@gmail.com', N'123', N'Hy', N'hy', CAST(N'2023-12-01' AS Date), NULL, 1, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (7, N'Dung', N'tran', CAST(N'2023-12-01' AS Date), 1, 129872193, N'dung@gmail.com', N'123', N'HY', N'Hy', CAST(N'2023-12-01' AS Date), NULL, 1, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (8, N'Khanh', N'nguyen', CAST(N'2023-12-01' AS Date), 1, 821839721, N'khanh@gmail.com', N'123', N'Hy', N'hy', CAST(N'2023-12-01' AS Date), NULL, 1, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (9, N'hai', N'nguyen', CAST(N'2023-12-01' AS Date), 1, 162381288, N'hai@gmail.com', N'123', N'Hy', N'Hy', CAST(N'2023-12-01' AS Date), NULL, 1, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (10, N'duong', N'nguyen', CAST(N'2023-12-01' AS Date), 1, 139871291, N'duong@gmail.com', N'123', N'Hy', N'Hy', CAST(N'2023-12-01' AS Date), NULL, 1, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (11, N'giang', N'Nguyen', CAST(N'2023-12-01' AS Date), 1, 133313166, N'giang@gmail.com', N'123', N'Hy', N'Hy', CAST(N'2023-12-01' AS Date), NULL, 1, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (13, N'Nguyen', N'Nguyen', CAST(N'2023-12-01' AS Date), 1, 729739199, N'giang@gmail.com', N'1331', NULL, NULL, CAST(N'2023-12-01' AS Date), NULL, 1, 0)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (15, N'chin', N'ngga', CAST(N'2023-12-01' AS Date), 1, 13333, N'giang@gmail.com', N'1313', N'hy', N'hy', CAST(N'2023-12-01' AS Date), NULL, 1, NULL)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (17, N'haha', N'nahah', CAST(N'2023-12-01' AS Date), 1, 861739291, N'giang@gmail.com', N'11421', N'Hy', N'Hy', CAST(N'2023-12-01' AS Date), NULL, 1, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (18, N'edsa', N'sadsada', CAST(N'2024-10-08' AS Date), 0, 0, N'dsadsa', N'd231', NULL, NULL, CAST(N'2024-10-08' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (19, N'edsa sda', N'sadsada', CAST(N'2024-10-08' AS Date), 0, 0, N'dsadsa ffasf', N'd231', NULL, NULL, CAST(N'2024-10-08' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (20, N'sdsadsa', N'dsadas', CAST(N'2024-10-23' AS Date), 0, 2141351, N'giang12344@gmail.com', N'sadsadsa', NULL, NULL, CAST(N'2024-10-09' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (21, N'sdsadsa', N'dsadas', CAST(N'2024-10-22' AS Date), 0, 1222324214, N'giang1111@gmail.com', N'giang213', NULL, NULL, CAST(N'2024-10-23' AS Date), NULL, 2, 0)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (22, N'sdsadsa dsad', N'dsadas', CAST(N'2024-10-09' AS Date), 0, 12332132, N'giang441@gmail.com', N'giang123', NULL, NULL, CAST(N'2024-10-08' AS Date), NULL, 2, 0)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (23, N'sdsadsa sadsa', N'dsadsasadsa', CAST(N'2024-10-16' AS Date), 0, 12332132, N'giang123213@gmail.com', N'sadsadsa', NULL, NULL, CAST(N'2024-10-08' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (24, N'sdasds', N'dsadas', CAST(N'2024-10-09' AS Date), 0, 12332132, N'giang213213@gmail.com', N'sadsadsa', N'HY', NULL, CAST(N'2024-10-08' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (25, N'HUng', N'Nguyen', CAST(N'2024-10-08' AS Date), 1, 192736261, N'hung123@gmail.com', N'hung123', N'HN', NULL, CAST(N'2024-10-08' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (26, N'Khanh', N'Nha', CAST(N'2024-10-23' AS Date), 0, 182873872, N'giang5555@gmail.com', N'Khanh123', N'HY', NULL, CAST(N'2024-10-08' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (27, N'sdsadsa dsad', N'adsadsadd', CAST(N'2024-10-30' AS Date), 1, 12332132, N'giang6868@gmail.com', N'sadsadsa', N'dasdsadsad', NULL, CAST(N'2024-10-08' AS Date), NULL, 2, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (28, N'Dung', N'Tuan', CAST(N'2024-10-23' AS Date), 0, 182873612, N'tuan@gmail.com', N'tuan123', N'hahahha', NULL, CAST(N'2024-10-09' AS Date), NULL, 3, 1)
INSERT [dbo].[Employees] ([emp_id], [emp_first_name], [emp_last_name], [emp_dob], [emp_gender], [emp_phoneNumber], [emp_email], [emp_password], [emp_address], [emp_ward], [emp_startDate], [emp_endDate], [emp_role_id], [emp_status]) VALUES (29, N'Nguyen', N'nguen', CAST(N'2024-10-09' AS Date), 1, 1921739872, N'giang8787@gmail.com', N'nguyen1232', N'Ha Noi', NULL, CAST(N'2024-10-09' AS Date), NULL, 2, 1)
SET IDENTITY_INSERT [dbo].[Employees] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([role_id], [role_name], [description]) VALUES (1, N'Admin', NULL)
INSERT [dbo].[Role] ([role_id], [role_name], [description]) VALUES (2, N'Staff', NULL)
INSERT [dbo].[Role] ([role_id], [role_name], [description]) VALUES (3, N'Kitchen', NULL)
INSERT [dbo].[Role] ([role_id], [role_name], [description]) VALUES (4, N'Manager', NULL)
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
ALTER TABLE [dbo].[Feedback] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Feedback] ADD  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_Customers] FOREIGN KEY([cus_id])
REFERENCES [dbo].[Customers] ([cus_id])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_Customers]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_Employees] FOREIGN KEY([emp_id])
REFERENCES [dbo].[Employees] ([emp_id])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_Employees]
GO
ALTER TABLE [dbo].[Booking_table]  WITH CHECK ADD  CONSTRAINT [FK_Booking_table_Accounts] FOREIGN KEY([acc_id])
REFERENCES [dbo].[Accounts] ([acc_id])
GO
ALTER TABLE [dbo].[Booking_table] CHECK CONSTRAINT [FK_Booking_table_Accounts]
GO
ALTER TABLE [dbo].[Booking_table]  WITH CHECK ADD  CONSTRAINT [FK_Booking_table_Table] FOREIGN KEY([table_id])
REFERENCES [dbo].[Table] ([table_id])
GO
ALTER TABLE [dbo].[Booking_table] CHECK CONSTRAINT [FK_Booking_table_Table]
GO
ALTER TABLE [dbo].[Combo_Detail]  WITH CHECK ADD  CONSTRAINT [FK_Combo Detail_Combo] FOREIGN KEY([combo_id])
REFERENCES [dbo].[Combo] ([combo_id])
GO
ALTER TABLE [dbo].[Combo_Detail] CHECK CONSTRAINT [FK_Combo Detail_Combo]
GO
ALTER TABLE [dbo].[Combo_Detail]  WITH CHECK ADD  CONSTRAINT [FK_Combo Detail_Menu] FOREIGN KEY([pro_id])
REFERENCES [dbo].[Products] ([pro_id])
GO
ALTER TABLE [dbo].[Combo_Detail] CHECK CONSTRAINT [FK_Combo Detail_Menu]
GO
ALTER TABLE [dbo].[Discount_code]  WITH CHECK ADD  CONSTRAINT [FK_Discount_code_Order] FOREIGN KEY([order_id])
REFERENCES [dbo].[Order] ([order_id])
GO
ALTER TABLE [dbo].[Discount_code] CHECK CONSTRAINT [FK_Discount_code_Order]
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_Employees_Role] FOREIGN KEY([emp_role_id])
REFERENCES [dbo].[Role] ([role_id])
GO
ALTER TABLE [dbo].[Employees] CHECK CONSTRAINT [FK_Employees_Role]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Accounts] FOREIGN KEY([acc_id])
REFERENCES [dbo].[Accounts] ([acc_id])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Accounts]
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Menu] FOREIGN KEY([pro_id])
REFERENCES [dbo].[Products] ([pro_id])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Menu]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Accounts] FOREIGN KEY([acc_id])
REFERENCES [dbo].[Accounts] ([acc_id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Accounts]
GO
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [FK_Order_Details_Order] FOREIGN KEY([order_id])
REFERENCES [dbo].[Order] ([order_id])
GO
ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [FK_Order_Details_Order]
GO
ALTER TABLE [dbo].[Order_Details]  WITH CHECK ADD  CONSTRAINT [FK_Order_Details_Products] FOREIGN KEY([pro_id])
REFERENCES [dbo].[Products] ([pro_id])
GO
ALTER TABLE [dbo].[Order_Details] CHECK CONSTRAINT [FK_Order_Details_Products]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Menu_Categories] FOREIGN KEY([cat_id])
REFERENCES [dbo].[Categories] ([cat_id])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Menu_Categories]
GO
