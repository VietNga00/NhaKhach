USE [TMS_Core_DEV]
GO
/****** Object:  Table [dbo].[GH_DanhSachKhach]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_DanhSachKhach](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeNghiId] [int] NOT NULL,
	[TenKhach] [nvarchar](150) NULL,
	[GioiTinh] [nvarchar](50) NULL,
	[ChucDanh] [nvarchar](100) NULL,
	[SoCccd] [varchar](50) NULL,
	[Sdt] [varchar](50) NULL,
	[TrangThai] [int] NULL,
	[NgayTao] [datetime] NULL,
	[NgayCapNhat] [datetime] NULL,
	[IsCancel] [bit] NULL,
 CONSTRAINT [PK__GH_DanhS__3214EC07899B4E1B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_DatPhong]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_DatPhong](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeNghiId] [int] NULL,
	[KhachId] [int] NULL,
	[TuNgay] [datetime] NULL,
	[DenNgay] [datetime] NULL,
	[AllDay] [bit] NULL,
	[PhongId] [int] NULL,
	[TinhTrangId] [int] NULL,
	[TrangThai] [bit] NULL,
	[GhiChu] [nvarchar](100) NULL,
	[NguoiDatId] [int] NULL,
	[NgayTao] [datetime] NULL,
	[NguoiHuyId] [int] NULL,
	[NgayHuy] [datetime] NULL,
	[NguoiThucHienId] [int] NULL,
	[NgayCapNhat] [datetime] NULL,
	[NguoiDuyetId] [int] NULL,
	[NgayDuyet] [datetime] NULL,
 CONSTRAINT [PK__GH_DatPh__3214EC07DD1B5147] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_DeNghi]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_DeNghi](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DonViId] [int] NULL,
	[NoiDung] [nvarchar](100) NULL,
	[SoLuongKhach] [int] NULL,
	[TinhTrangId] [int] NULL,
	[GhiChu] [nvarchar](100) NULL,
	[NguoiTaoId] [int] NULL,
	[NgayTao] [datetime] NULL,
	[NguoiDuyetId] [int] NULL,
	[NgayDuyet] [datetime] NULL,
	[NguoiThucHienId] [int] NULL,
	[NgayCapNhat] [datetime] NULL,
 CONSTRAINT [PK__GH_DeNgh__3214EC07A0F14F65] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_LoaiPhong]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_LoaiPhong](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MaLoai] [varchar](50) NOT NULL,
	[TenLoai] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](100) NULL,
	[SoLuongToiDa] [int] NOT NULL,
	[TrangThai] [bit] NOT NULL,
	[NgayTao] [datetime] NULL,
	[NgayCapNhat] [datetime] NULL,
 CONSTRAINT [PK__GH_LoaiP__3214EC07E52B292A] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_NhanPhong]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_NhanPhong](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DatPhongId] [int] NOT NULL,
	[GhiChu] [nvarchar](50) NULL,
	[NguoiThucHienId] [int] NOT NULL,
	[NgayNhan] [datetime] NULL,
 CONSTRAINT [PK__GH_NhanP__3214EC0708888113] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_Phong]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_Phong](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MaPhong] [varchar](50) NOT NULL,
	[TenPhong] [nvarchar](50) NOT NULL,
	[TrangThai] [bit] NOT NULL,
	[NgayTao] [datetime] NULL,
	[NgayCapNhat] [datetime] NULL,
	[LoaiId] [int] NOT NULL,
	[ToaNhaId] [int] NOT NULL,
 CONSTRAINT [PK__GH_Phong__3214EC07CB8CEC9D] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_ThongKe]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_ThongKe](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Thang] [int] NOT NULL,
	[Nam] [int] NOT NULL,
	[TongLichDat] [int] NOT NULL,
	[TongLichHuy] [int] NOT NULL,
	[TongLichQuaHan] [int] NOT NULL,
	[TongSoKhach] [int] NULL,
	[NgayThongKe] [datetime] NULL,
 CONSTRAINT [PK__GH_Thong__3214EC0704B98223] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_ThongKePhong]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_ThongKePhong](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Thang] [int] NOT NULL,
	[Nam] [int] NOT NULL,
	[PhongId] [int] NOT NULL,
	[SoLichDat] [int] NOT NULL,
	[SoLichHuy] [int] NOT NULL,
	[SoLichQuaHan] [int] NOT NULL,
	[NgayThongKe] [datetime] NULL,
 CONSTRAINT [PK__GH_Thong__3214EC07844CCCA9] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_TinhTrang]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_TinhTrang](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenTinhTrang] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](100) NULL,
	[NgayTao] [datetime] NULL,
	[NgayCapNhat] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GH_TraPhong]    Script Date: 16/5/25 8:28:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GH_TraPhong](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DatPhongId] [int] NULL,
	[TongThoiGian] [nvarchar](100) NOT NULL,
	[NgayTra] [datetime] NULL,
	[GhiChu] [nvarchar](50) NULL,
	[NguoiThucHienId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


SET IDENTITY_INSERT [dbo].[GH_LoaiPhong] ON 

INSERT [dbo].[GH_LoaiPhong] ([Id], [MaLoai], [TenLoai], [MoTa], [SoLuongToiDa], [TrangThai], [NgayTao], [NgayCapNhat]) VALUES (1, N'LP1', N'Phòng đơn ', N'Loại phòng dành cho 1 người ', 1, 0, CAST(N'2025-03-28T10:34:19.577' AS DateTime), CAST(N'2025-04-24T14:20:20.450' AS DateTime))
INSERT [dbo].[GH_LoaiPhong] ([Id], [MaLoai], [TenLoai], [MoTa], [SoLuongToiDa], [TrangThai], [NgayTao], [NgayCapNhat]) VALUES (2, N'LP2', N'Phòng đôi', N'Loại phòng cho 2 người', 2, 0, CAST(N'2025-03-31T16:47:06.497' AS DateTime), CAST(N'2025-04-03T14:36:09.837' AS DateTime))
SET IDENTITY_INSERT [dbo].[GH_LoaiPhong] OFF
GO

SET IDENTITY_INSERT [dbo].[GH_Phong] ON 

INSERT [dbo].[GH_Phong] ([Id], [MaPhong], [TenPhong], [TrangThai], [NgayTao], [NgayCapNhat], [LoaiId], [ToaNhaId]) VALUES (3, N'A11.312', N'Phòng nghỉ A1', 0, CAST(N'2025-03-28T10:37:39.310' AS DateTime), NULL, 1, 1)
INSERT [dbo].[GH_Phong] ([Id], [MaPhong], [TenPhong], [TrangThai], [NgayTao], [NgayCapNhat], [LoaiId], [ToaNhaId]) VALUES (4, N'A11.313', N'Phòng nghỉ A1', 0, CAST(N'2025-03-31T16:47:18.333' AS DateTime), NULL, 2, 1)
INSERT [dbo].[GH_Phong] ([Id], [MaPhong], [TenPhong], [TrangThai], [NgayTao], [NgayCapNhat], [LoaiId], [ToaNhaId]) VALUES (5, N'A21.110', N'Phòng nghỉ A2', 0, CAST(N'2025-03-31T16:47:32.057' AS DateTime), NULL, 1, 2)
INSERT [dbo].[GH_Phong] ([Id], [MaPhong], [TenPhong], [TrangThai], [NgayTao], [NgayCapNhat], [LoaiId], [ToaNhaId]) VALUES (6, N'A11.314', N'Phòng nghỉ A1', 0, CAST(N'2025-04-23T16:07:05.707' AS DateTime), NULL, 1, 1)
INSERT [dbo].[GH_Phong] ([Id], [MaPhong], [TenPhong], [TrangThai], [NgayTao], [NgayCapNhat], [LoaiId], [ToaNhaId]) VALUES (7, N'A11.311', N'Phòng nghỉ A1', 0, CAST(N'2025-04-23T16:07:35.273' AS DateTime), NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[GH_Phong] OFF
GO

SET IDENTITY_INSERT [dbo].[GH_TinhTrang] ON 

INSERT [dbo].[GH_TinhTrang] ([Id], [TenTinhTrang], [MoTa], [NgayTao], [NgayCapNhat]) VALUES (1, N'Khởi tạo ', N'Đề nghị mới được tạo', CAST(N'2025-03-28T00:00:00.000' AS DateTime), CAST(N'2025-04-04T08:11:15.007' AS DateTime))
INSERT [dbo].[GH_TinhTrang] ([Id], [TenTinhTrang], [MoTa], [NgayTao], [NgayCapNhat]) VALUES (2, N'Chờ duyệt', N'Đề nghị/ Lịch đang chờ duyệt', CAST(N'2025-03-28T08:54:20.433' AS DateTime), CAST(N'2025-04-04T08:11:42.983' AS DateTime))
INSERT [dbo].[GH_TinhTrang] ([Id], [TenTinhTrang], [MoTa], [NgayTao], [NgayCapNhat]) VALUES (3, N'Không duyệt', N'Lịch không được duyệt', CAST(N'2025-04-04T08:09:36.110' AS DateTime), CAST(N'2025-04-04T08:12:01.030' AS DateTime))
INSERT [dbo].[GH_TinhTrang] ([Id], [TenTinhTrang], [MoTa], [NgayTao], [NgayCapNhat]) VALUES (4, N'Đã duyệt', N'Lịch đã được duyệt', CAST(N'2025-04-04T08:12:02.970' AS DateTime), NULL)
INSERT [dbo].[GH_TinhTrang] ([Id], [TenTinhTrang], [MoTa], [NgayTao], [NgayCapNhat]) VALUES (5, N'Đã nhận phòng', N'Lịch đã nhận phòng', CAST(N'2025-04-04T08:12:19.403' AS DateTime), NULL)
INSERT [dbo].[GH_TinhTrang] ([Id], [TenTinhTrang], [MoTa], [NgayTao], [NgayCapNhat]) VALUES (6, N'Hoàn thành', N'Đề nghị/ Lịch đã hoàn thành', CAST(N'2025-04-11T08:26:25.833' AS DateTime), CAST(N'2025-04-14T10:12:36.277' AS DateTime))
INSERT [dbo].[GH_TinhTrang] ([Id], [TenTinhTrang], [MoTa], [NgayTao], [NgayCapNhat]) VALUES (7, N'Đã hủy', N'Lịch đã hủy', CAST(N'2025-04-16T15:40:55.310' AS DateTime), CAST(N'2025-05-13T07:34:55.237' AS DateTime))
SET IDENTITY_INSERT [dbo].[GH_TinhTrang] OFF
GO

ALTER TABLE [dbo].[GH_DanhSachKhach] ADD  CONSTRAINT [DF_GH_DanhSachKhach_IsCancel]  DEFAULT ((0)) FOR [IsCancel]
GO
ALTER TABLE [dbo].[GH_DatPhong] ADD  CONSTRAINT [DF_GH_DatPhong_AllDay]  DEFAULT ((0)) FOR [AllDay]
GO
ALTER TABLE [dbo].[GH_DatPhong] ADD  CONSTRAINT [DF_GH_DatPhong_TrangThai]  DEFAULT ((0)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[GH_LoaiPhong] ADD  CONSTRAINT [DF_GH_LoaiPhong_TrangThai]  DEFAULT ((0)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[GH_Phong] ADD  CONSTRAINT [DF_GH_Phong_TrangThai]  DEFAULT ((0)) FOR [TrangThai]
GO
ALTER TABLE [dbo].[GH_DanhSachKhach]  WITH CHECK ADD  CONSTRAINT [FK__GH_DanhSa__DatPh__5D39EB3A] FOREIGN KEY([DeNghiId])
REFERENCES [dbo].[GH_DeNghi] ([Id])
GO
ALTER TABLE [dbo].[GH_DanhSachKhach] CHECK CONSTRAINT [FK__GH_DanhSa__DatPh__5D39EB3A]
GO
ALTER TABLE [dbo].[GH_DatPhong]  WITH CHECK ADD  CONSTRAINT [FK__DatPhog__Phong__568CEDAB] FOREIGN KEY([PhongId])
REFERENCES [dbo].[GH_Phong] ([Id])
GO
ALTER TABLE [dbo].[GH_DatPhong] CHECK CONSTRAINT [FK__DatPhog__Phong__568CEDAB]
GO
ALTER TABLE [dbo].[GH_DatPhong]  WITH CHECK ADD  CONSTRAINT [FK__GH_DatPho__Nguoi__5875361D] FOREIGN KEY([NguoiDatId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
ALTER TABLE [dbo].[GH_DatPhong] CHECK CONSTRAINT [FK__GH_DatPho__Nguoi__5875361D]
GO
ALTER TABLE [dbo].[GH_DatPhong]  WITH CHECK ADD  CONSTRAINT [FK__GH_DatPho__Nguoi__5A5D7E8F] FOREIGN KEY([NguoiThucHienId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
ALTER TABLE [dbo].[GH_DatPhong] CHECK CONSTRAINT [FK__GH_DatPho__Nguoi__5A5D7E8F]
GO
ALTER TABLE [dbo].[GH_DatPhong]  WITH CHECK ADD  CONSTRAINT [FK__GH_DatPho__TinhT__578111E4] FOREIGN KEY([TinhTrangId])
REFERENCES [dbo].[GH_TinhTrang] ([Id])
GO
ALTER TABLE [dbo].[GH_DatPhong] CHECK CONSTRAINT [FK__GH_DatPho__TinhT__578111E4]
GO
ALTER TABLE [dbo].[GH_DatPhong]  WITH CHECK ADD  CONSTRAINT [FK_GH_DatPhong_GH_DanhSachKhach] FOREIGN KEY([KhachId])
REFERENCES [dbo].[GH_DanhSachKhach] ([Id])
GO
ALTER TABLE [dbo].[GH_DatPhong] CHECK CONSTRAINT [FK_GH_DatPhong_GH_DanhSachKhach]
GO
ALTER TABLE [dbo].[GH_DatPhong]  WITH CHECK ADD  CONSTRAINT [FK_GH_DatPhong_GH_DeNghi] FOREIGN KEY([DeNghiId])
REFERENCES [dbo].[GH_DeNghi] ([Id])
GO
ALTER TABLE [dbo].[GH_DatPhong] CHECK CONSTRAINT [FK_GH_DatPhong_GH_DeNghi]
GO
ALTER TABLE [dbo].[GH_DatPhong]  WITH CHECK ADD  CONSTRAINT [FK_GH_DatPhong_VienChuc] FOREIGN KEY([NguoiDuyetId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
ALTER TABLE [dbo].[GH_DatPhong] CHECK CONSTRAINT [FK_GH_DatPhong_VienChuc]
GO
ALTER TABLE [dbo].[GH_DatPhong]  WITH CHECK ADD  CONSTRAINT [FK_GH_DatPhong_VienChuc1] FOREIGN KEY([NguoiHuyId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
ALTER TABLE [dbo].[GH_DatPhong] CHECK CONSTRAINT [FK_GH_DatPhong_VienChuc1]
GO
ALTER TABLE [dbo].[GH_DeNghi]  WITH CHECK ADD  CONSTRAINT [FK__GH_DeNghi__DonVi__31E578E1] FOREIGN KEY([DonViId])
REFERENCES [dbo].[DonVi] ([ID])
GO
ALTER TABLE [dbo].[GH_DeNghi] CHECK CONSTRAINT [FK__GH_DeNghi__DonVi__31E578E1]
GO
ALTER TABLE [dbo].[GH_DeNghi]  WITH CHECK ADD  CONSTRAINT [FK__GH_DeNghi__Nguoi__33CDC153] FOREIGN KEY([NguoiTaoId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
ALTER TABLE [dbo].[GH_DeNghi] CHECK CONSTRAINT [FK__GH_DeNghi__Nguoi__33CDC153]
GO
ALTER TABLE [dbo].[GH_DeNghi]  WITH CHECK ADD  CONSTRAINT [FK_GH_DeNghi_GH_TinhTrang] FOREIGN KEY([TinhTrangId])
REFERENCES [dbo].[GH_TinhTrang] ([Id])
GO
ALTER TABLE [dbo].[GH_DeNghi] CHECK CONSTRAINT [FK_GH_DeNghi_GH_TinhTrang]
GO
ALTER TABLE [dbo].[GH_DeNghi]  WITH CHECK ADD  CONSTRAINT [FK_GH_DeNghi_NguoiThuHien] FOREIGN KEY([NguoiThucHienId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
ALTER TABLE [dbo].[GH_DeNghi] CHECK CONSTRAINT [FK_GH_DeNghi_NguoiThuHien]
GO
ALTER TABLE [dbo].[GH_DeNghi]  WITH CHECK ADD  CONSTRAINT [FK_GH_DeNghi_VienChuc] FOREIGN KEY([NguoiDuyetId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
ALTER TABLE [dbo].[GH_DeNghi] CHECK CONSTRAINT [FK_GH_DeNghi_VienChuc]
GO
ALTER TABLE [dbo].[GH_NhanPhong]  WITH CHECK ADD  CONSTRAINT [FK__GH_NhanPh__DatPh__610A7C1E] FOREIGN KEY([DatPhongId])
REFERENCES [dbo].[GH_DatPhong] ([Id])
GO
ALTER TABLE [dbo].[GH_NhanPhong] CHECK CONSTRAINT [FK__GH_NhanPh__DatPh__610A7C1E]
GO
ALTER TABLE [dbo].[GH_NhanPhong]  WITH CHECK ADD  CONSTRAINT [FK__GH_NhanPh__Nguoi__61FEA057] FOREIGN KEY([NguoiThucHienId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
ALTER TABLE [dbo].[GH_NhanPhong] CHECK CONSTRAINT [FK__GH_NhanPh__Nguoi__61FEA057]
GO
ALTER TABLE [dbo].[GH_Phong]  WITH CHECK ADD  CONSTRAINT [FK__GH_Phong__LoaiId__51C8388E] FOREIGN KEY([LoaiId])
REFERENCES [dbo].[GH_LoaiPhong] ([Id])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[GH_Phong] CHECK CONSTRAINT [FK__GH_Phong__LoaiId__51C8388E]
GO
ALTER TABLE [dbo].[GH_Phong]  WITH CHECK ADD  CONSTRAINT [FK_GH_Phong_ToaNha] FOREIGN KEY([ToaNhaId])
REFERENCES [dbo].[ToaNha] ([ID])
GO
ALTER TABLE [dbo].[GH_Phong] CHECK CONSTRAINT [FK_GH_Phong_ToaNha]
GO
ALTER TABLE [dbo].[GH_ThongKePhong]  WITH CHECK ADD  CONSTRAINT [FK__GH_ThongK__Phong__3F747E29] FOREIGN KEY([PhongId])
REFERENCES [dbo].[GH_Phong] ([Id])
GO
ALTER TABLE [dbo].[GH_ThongKePhong] CHECK CONSTRAINT [FK__GH_ThongK__Phong__3F747E29]
GO
ALTER TABLE [dbo].[GH_TraPhong]  WITH CHECK ADD  CONSTRAINT [FK__GH_TraPho__DatPh__6FE2AB5A] FOREIGN KEY([DatPhongId])
REFERENCES [dbo].[GH_DatPhong] ([Id])
GO
ALTER TABLE [dbo].[GH_TraPhong] CHECK CONSTRAINT [FK__GH_TraPho__DatPh__6FE2AB5A]
GO
ALTER TABLE [dbo].[GH_TraPhong]  WITH CHECK ADD FOREIGN KEY([NguoiThucHienId])
REFERENCES [dbo].[VienChuc] ([ID])
GO
