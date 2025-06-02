<<<<<<< HEAD
USE TMS_Core_DEV
GO


CREATE PROCEDURE GH_DeNghi_ChiTiet
	@denghiID INT
	
AS
BEGIN	
	SET NOCOUNT ON;  

	SELECT 
		GH_DeNghi.Id,
		GH_DanhSachKhach.DeNghiId,
		GH_DanhSachKhach.TenKhach,
		GH_DanhSachKhach.ChucDanh,
		GH_DanhSachKhach.TrangThai,
		GH_DatPhong.DeNghiId AS GH_DatPhong_DeNghiId,
		GH_DatPhong.KhachId,
		GH_DatPhong.GhiChu,
		GH_DatPhong.AllDay,
		GH_DatPhong.TuNgay,
		GH_DatPhong.DenNgay,
		GH_Phong.MaPhong,
		GH_Phong.TenPhong
	FROM GH_DeNghi
	INNER JOIN GH_DanhSachKhach ON GH_DanhSachKhach.DeNghiId = GH_DeNghi.Id
	INNER JOIN GH_DatPhong ON GH_DatPhong.KhachId = GH_DanhSachKhach.Id AND GH_DatPhong.DeNghiId = GH_DeNghi.Id
	INNER JOIN GH_Phong ON GH_Phong.Id = GH_DatPhong.PhongId
	WHERE GH_DeNghi.Id = @denghiID

END
GO
=======
USE TMS_Core_DEV
GO


CREATE PROCEDURE GH_DeNghi_ChiTiet
	@denghiID INT
	
AS
BEGIN	
	SET NOCOUNT ON;  

	SELECT 
		GH_DeNghi.Id,
		GH_DanhSachKhach.DeNghiId,
		GH_DanhSachKhach.TenKhach,
		GH_DanhSachKhach.ChucDanh,
		GH_DanhSachKhach.TrangThai,
		GH_DatPhong.DeNghiId AS GH_DatPhong_DeNghiId,
		GH_DatPhong.KhachId,
		GH_DatPhong.GhiChu,
		GH_DatPhong.AllDay,
		GH_DatPhong.TuNgay,
		GH_DatPhong.DenNgay,
		GH_Phong.MaPhong,
		GH_Phong.TenPhong
	FROM GH_DeNghi
	INNER JOIN GH_DanhSachKhach ON GH_DanhSachKhach.DeNghiId = GH_DeNghi.Id
	INNER JOIN GH_DatPhong ON GH_DatPhong.KhachId = GH_DanhSachKhach.Id AND GH_DatPhong.DeNghiId = GH_DeNghi.Id
	INNER JOIN GH_Phong ON GH_Phong.Id = GH_DatPhong.PhongId
	WHERE GH_DeNghi.Id = @denghiID

END
GO
>>>>>>> f533bc1 (Update)
