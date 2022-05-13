CREATE TRIGGER TRG_PROD_INV_STK_UPDATE 
ON ProdInventory 
AFTER INSERT, UPDATE, DELETE   
AS  
BEGIN 
	DECLARE @WH_Location_id uniqueidentifier, @ProductMasterId uniqueidentifier, @ExpiryDate date;
	DECLARE @CreatedBy uniqueidentifier, @CreatedDate smalldatetime
	DECLARE @ShelveNo varchar(20);
	DECLARE @stock_balance NUMERIC(18,6);

	SELECT  @WH_Location_id = WareHouseLocationId, @ProductMasterId = ProductMasterId, @ExpiryDate = ExpiryDate,
			@ShelveNo = ShelveNo,
			@CreatedBy = CreatedBy, @CreatedDate = CreatedDate
	FROM 
	(SELECT top 1 WareHouseLocationId, ProductMasterId, ExpiryDate, ShelveNo,CreatedBy, CreatedDate from 
			(
				SELECT   i.WareHouseLocationId, i.ProductMasterId, i.ExpiryDate, ShelveNo, CreatedBy, CreatedDate
					   FROM inserted AS i 
				union 
					SELECT    d.WareHouseLocationId, d.ProductMasterId, d.ExpiryDate, ShelveNo, CreatedBy, CreatedDate FROM deleted AS d 
			) as modified
	) as tu

	SELECT @stock_balance = ISNULL(sum(StockIn - StockOut),0) FROM ProdInventory inv
	WHERE inv.WareHouseLocationId = @WH_Location_id 
	AND inv.ProductMasterId = @ProductMasterId
	AND inv.ShelveNo = @ShelveNo
	AND  ((inv.ExpiryDate IS NULL AND @ExpiryDate IS NULL) OR inv.ExpiryDate = @ExpiryDate)

	IF EXISTS (SELECT 1  
           FROM ProdInventoryBalance bal 
           WHERE bal.WareHouseLocationId = @WH_Location_id 
		   AND bal.ProductMasterId = @ProductMasterId
		   AND ShelveNo = @ShelveNo
		   AND  (bal.ExpiryDate IS NULL OR @ExpiryDate IS NULL OR bal.ExpiryDate = @ExpiryDate)
          )
	BEGIN 

		UPDATE ProdInventoryBalance set AvlQuantity = @stock_balance where 
		   WareHouseLocationId = @WH_Location_id 
		   AND ProductMasterId = @ProductMasterId
		   AND ShelveNo = @ShelveNo
		   AND  (ExpiryDate IS NULL OR @ExpiryDate IS NULL OR ExpiryDate = @ExpiryDate)
	END
	ELSE 
	BEGIN
		INSERT INTO ProdInventoryBalance (WareHouseLocationId, ProductMasterId,ExpiryDate, ShelveNo,AvlQuantity,Active,CreatedBy,CreatedDate)
			VALUES (@WH_Location_id, @ProductMasterId, @ExpiryDate, @ShelveNo, @stock_balance, 'Y', @CreatedBy, @CreatedDate)
	END;

END;
GO  
 