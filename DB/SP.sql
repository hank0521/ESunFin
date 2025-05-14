-- 存儲過程 (Stored Procedures)
USE FinancialProductsDB;
GO

-- 1. 取得所有使用者
CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    SELECT UserID, UserName, Email, Account 
    FROM [User]
    ORDER BY UserName;
END
GO

-- 2. 根據UserID取得使用者
CREATE PROCEDURE sp_GetUserById
    @UserID NVARCHAR(50)
AS
BEGIN
    SELECT UserID, UserName, Email, Account 
    FROM [User]
    WHERE UserID = @UserID;
END
GO

-- 3. 新增使用者
CREATE PROCEDURE sp_InsertUser
    @UserID NVARCHAR(50),
    @UserName NVARCHAR(50),
    @Email NVARCHAR(100),
    @Account NVARCHAR(20)
AS
BEGIN
    INSERT INTO [User] (UserID, UserName, Email, Account)
    VALUES (@UserID, @UserName, @Email, @Account);
END
GO

-- 4. 取得所有金融商品
CREATE PROCEDURE sp_GetAllProducts
AS
BEGIN
    SELECT No, ProductName, Price, FeeRate
    FROM Product
    ORDER BY ProductName;
END
GO

-- 5. 根據No取得金融商品
CREATE PROCEDURE sp_GetProductByNo
    @No INT
AS
BEGIN
    SELECT No, ProductName, Price, FeeRate
    FROM Product
    WHERE No = @No;
END
GO

-- 6. 新增金融商品
CREATE PROCEDURE sp_InsertProduct
    @ProductName NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @FeeRate DECIMAL(5, 4),
    @NewProductNo INT OUTPUT
AS
BEGIN
    INSERT INTO Product (ProductName, Price, FeeRate)
    VALUES (@ProductName, @Price, @FeeRate);
    
    SET @NewProductNo = SCOPE_IDENTITY();
END
GO

-- 7. 更新金融商品
CREATE PROCEDURE sp_UpdateProduct
    @No INT,
    @ProductName NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @FeeRate DECIMAL(5, 4)
AS
BEGIN
    UPDATE Product 
    SET ProductName = @ProductName, 
        Price = @Price, 
        FeeRate = @FeeRate
    WHERE No = @No;
END
GO

-- 8. 刪除金融商品
CREATE PROCEDURE sp_DeleteProduct
    @No INT
AS
BEGIN
    -- 檢查是否有相關的喜好清單項目
    IF EXISTS (SELECT 1 FROM LikeList WHERE ProductNo = @No)
    BEGIN
        RAISERROR('無法刪除，此產品已存在於喜好清單中', 16, 1);
        RETURN;
    END
    
    DELETE FROM Product 
    WHERE No = @No;
END
GO

-- 9. 取得使用者所有喜好金融商品
CREATE PROCEDURE sp_GetUserLikeList
    @UserID NVARCHAR(50)
AS
BEGIN
    SELECT 
        L.SN, 
        L.UserID, 
        L.ProductNo, 
        L.OrderQty, 
        L.Account, 
        L.TotalFee, 
        L.TotalAmount,
        P.ProductName, 
        P.Price, 
        P.FeeRate,
        U.Email
    FROM LikeList L
    INNER JOIN Product P ON L.ProductNo = P.No
    INNER JOIN [User] U ON L.UserID = U.UserID
    WHERE L.UserID = @UserID
    ORDER BY L.SN DESC;
END
GO

-- 10. 根據SN取得喜好金融商品
CREATE PROCEDURE sp_GetLikeListBySN
    @SN INT
AS
BEGIN
    SELECT 
        L.SN, 
        L.UserID, 
        L.ProductNo, 
        L.OrderQty, 
        L.Account, 
        L.TotalFee, 
        L.TotalAmount,
        P.ProductName, 
        P.Price, 
        P.FeeRate,
        U.Email
    FROM LikeList L
    INNER JOIN Product P ON L.ProductNo = P.No
    INNER JOIN [User] U ON L.UserID = U.UserID
    WHERE L.SN = @SN;
END
GO

-- 11. 新增喜好金融商品
CREATE PROCEDURE sp_InsertLikeList
    @UserID NVARCHAR(50),
    @ProductNo INT,
    @OrderQty INT,
    @Account NVARCHAR(20),
    @NewSN INT OUTPUT
AS
BEGIN
    -- 計算總金額和手續費
    DECLARE @Price DECIMAL(18, 2);
    DECLARE @FeeRate DECIMAL(5, 4);
    DECLARE @TotalAmount DECIMAL(18, 2);
    DECLARE @TotalFee DECIMAL(18, 2);
    
    SELECT @Price = Price, @FeeRate = FeeRate 
    FROM Product 
    WHERE No = @ProductNo;
    
    SET @TotalAmount = @Price * @OrderQty;
    SET @TotalFee = @TotalAmount * @FeeRate;
    
    INSERT INTO LikeList (UserID, ProductNo, OrderQty, Account, TotalFee, TotalAmount)
    VALUES (@UserID, @ProductNo, @OrderQty, @Account, @TotalFee, @TotalAmount);
    
    SET @NewSN = SCOPE_IDENTITY();
END
GO

-- 12. 更新喜好金融商品
CREATE PROCEDURE sp_UpdateLikeList
    @SN INT,
    @ProductNo INT,
    @OrderQty INT,
    @Account NVARCHAR(20)
AS
BEGIN
    -- 計算總金額和手續費
    DECLARE @Price DECIMAL(18, 2);
    DECLARE @FeeRate DECIMAL(5, 4);
    DECLARE @TotalAmount DECIMAL(18, 2);
    DECLARE @TotalFee DECIMAL(18, 2);
    
    SELECT @Price = Price, @FeeRate = FeeRate 
    FROM Product 
    WHERE No = @ProductNo;
    
    SET @TotalAmount = @Price * @OrderQty;
    SET @TotalFee = @TotalAmount * @FeeRate;
    
    UPDATE LikeList 
    SET ProductNo = @ProductNo,
        OrderQty = @OrderQty,
        Account = @Account,
        TotalFee = @TotalFee,
        TotalAmount = @TotalAmount
    WHERE SN = @SN;
END
GO

-- 13. 刪除喜好金融商品
CREATE PROCEDURE sp_DeleteLikeList
    @SN INT
AS
BEGIN
    DELETE FROM LikeList 
    WHERE SN = @SN;
END
GO