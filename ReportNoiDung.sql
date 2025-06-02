<<<<<<< HEAD
USE TMS_Core_DEV;
GO


CREATE PROCEDURE GH_DeNghi_NoiDung
	@denghiID INT
	
AS
BEGIN	
	SET NOCOUNT ON;  

	SELECT 
		GH_DeNghi.Id,
		DonVi.TenDonVi,
		GH_DeNghi.NoiDung,
		GH_DeNghi.SoLuongKhach,
		GH_DeNghi.NgayTao
	FROM GH_DeNghi
	INNER JOIN DonVi ON DonVi.ID = GH_DeNghi.DonViId
	WHERE GH_DeNghi.Id = @denghiID

END
GO
=======
USE TMS_Core_DEV;
GO


CREATE PROCEDURE GH_DeNghi_NoiDung
	@denghiID INT
	
AS
BEGIN	
	SET NOCOUNT ON;  

	SELECT 
		GH_DeNghi.Id,
		DonVi.TenDonVi,
		GH_DeNghi.NoiDung,
		GH_DeNghi.SoLuongKhach,
		GH_DeNghi.NgayTao
	FROM GH_DeNghi
	INNER JOIN DonVi ON DonVi.ID = GH_DeNghi.DonViId
	WHERE GH_DeNghi.Id = @denghiID

END
GO
>>>>>>> f533bc1 (Update)
