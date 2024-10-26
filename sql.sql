USE [SRMMS]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[acc_id] [int] IDENTITY(1,1) NOT NULL,
	[full_name] [nvarchar](50) NULL,
	[email] [nvarchar](50) NULL,
	[password] [nvarchar](20) NULL,
	[phone] [nvarchar](12) NULL,
	[role_id] [int] NULL,
 CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED 
(
	[acc_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[cat_id] [int] IDENTITY(1,1) NOT NULL,
	[cat_name] [nvarchar](50) NULL,
	[description] [nvarchar](150) NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[cat_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Combo]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Combo](
	[combo_id] [int] IDENTITY(1,1) NOT NULL,
	[combo_name] [nvarchar](200) NULL,
	[combo_discription] [nvarchar](max) NULL,
	[combo_img] [nvarchar](max) NULL,
	[combo_money] [money] NULL,
	[combo_status] [bit] NULL,
 CONSTRAINT [PK_Combo] PRIMARY KEY CLUSTERED 
(
	[combo_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Combo_Detail]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Combo_Detail](
	[combo_id] [int] NOT NULL,
	[pro_id] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Discount_code]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discount_code](
	[code_id] [int] NOT NULL,
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
/****** Object:  Table [dbo].[Feedback]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[feedback_id] [int] NOT NULL,
	[feedback] [nvarchar](300) NULL,
	[rate_star] [int] NULL,
	[acc_id] [int] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order](
	[order_id] [int] IDENTITY(1,1) NOT NULL,
	[order_date] [date] NULL,
	[acc_id] [int] NULL,
	[totalMoney] [money] NULL,
	[order_status_id] [int] NULL,
	[staus] [bit] NULL,
	[code_id] [int] NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[order_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Order_Details]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Order_Details](
	[order_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[order_id] [int] NULL,
	[pro_id] [int] NULL,
	[quantiity] [int] NULL,
	[price] [money] NULL,
 CONSTRAINT [PK_Order_Details] PRIMARY KEY CLUSTERED 
(
	[order_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Point_List]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Point_List](
	[point_id] [int] NOT NULL,
	[acc_id] [int] NULL,
	[number_ponit] [float] NULL,
 CONSTRAINT [PK_Point_List] PRIMARY KEY CLUSTERED 
(
	[point_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[pro_id] [int] IDENTITY(1,1) NOT NULL,
	[pro_name] [nvarchar](200) NULL,
	[pro_discription] [nvarchar](max) NULL,
	[pro_price] [money] NULL,
	[cat_id] [int] NULL,
	[pro_img] [nvarchar](max) NULL,
	[pro_calories] [nvarchar](250) NULL,
	[pro_status] [bit] NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[pro_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[role_id] [int] IDENTITY(1,1) NOT NULL,
	[role_name] [nvarchar](50) NULL,
	[description] [nvarchar](150) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Table]    Script Date: 10/25/2024 10:58:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Table](
	[table_id] [int] NOT NULL,
	[table_name] [nvarchar](50) NULL,
	[QR_code] [nvarchar](50) NULL,
	[acc_id] [int] NULL,
	[time_booking] [date] NULL,
	[status] [bit] NULL,
 CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED 
(
	[table_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Feedback] ADD  CONSTRAINT [DF__Feedback__create__619B8048]  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[Feedback] ADD  CONSTRAINT [DF__Feedback__update__628FA481]  DEFAULT (getdate()) FOR [updated_at]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_Accounts_Role] FOREIGN KEY([role_id])
REFERENCES [dbo].[Role] ([role_id])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_Accounts_Role]
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
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD  CONSTRAINT [FK_Feedback_Accounts] FOREIGN KEY([acc_id])
REFERENCES [dbo].[Accounts] ([acc_id])
GO
ALTER TABLE [dbo].[Feedback] CHECK CONSTRAINT [FK_Feedback_Accounts]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Accounts] FOREIGN KEY([acc_id])
REFERENCES [dbo].[Accounts] ([acc_id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Accounts]
GO
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Discount_code] FOREIGN KEY([code_id])
REFERENCES [dbo].[Discount_code] ([code_id])
GO
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Discount_code]
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
ALTER TABLE [dbo].[Point_List]  WITH CHECK ADD  CONSTRAINT [FK_Point_List_Accounts] FOREIGN KEY([acc_id])
REFERENCES [dbo].[Accounts] ([acc_id])
GO
ALTER TABLE [dbo].[Point_List] CHECK CONSTRAINT [FK_Point_List_Accounts]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Menu_Categories] FOREIGN KEY([cat_id])
REFERENCES [dbo].[Categories] ([cat_id])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Menu_Categories]
GO
ALTER TABLE [dbo].[Table]  WITH CHECK ADD  CONSTRAINT [FK_Table_Accounts] FOREIGN KEY([acc_id])
REFERENCES [dbo].[Accounts] ([acc_id])
GO
ALTER TABLE [dbo].[Table] CHECK CONSTRAINT [FK_Table_Accounts]
GO
