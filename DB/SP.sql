-- �s�x�L�{ (Stored Procedures)
USE FinancialProductsDB;
GO

-- 1. ���o�Ҧ��ϥΪ�
CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    SELECT UserID, UserName, Email, Account 
    FROM [User]
    ORDER BY UserName;
END
GO

-- 2. �ھ�UserID���o�ϥΪ�
CREATE PROCEDURE sp_GetUserById
    @UserID NVARCHAR(50)
AS
BEGIN
    SELECT UserID, UserName, Email, Account 
    FROM [User]
    WHERE UserID = @UserID;
END
GO

-- 3. �s�W�ϥΪ�
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

-- 4. ���o�Ҧ����İӫ~
CREATE PROCEDURE sp_GetAllProducts
AS
BEGIN
    SELECT No, ProductName, Price, FeeRate
    FROM Product
    ORDER BY ProductName;
END
GO

-- 5. �ھ�No���o���İӫ~
CREATE PROCEDURE sp_GetProductByNo
    @No INT
AS
BEGIN
    SELECT No, ProductName, Price, FeeRate
    FROM Product
    WHERE No = @No;
END
GO

-- 6. �s�W���İӫ~
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

-- 7. ��s���İӫ~
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

-- 8. �R�����İӫ~
CREATE PROCEDURE sp_DeleteProduct
    @No INT
AS
BEGIN
    -- �ˬd�O�_���������ߦn�M�涵��
    IF EXISTS (SELECT 1 FROM LikeList WHERE ProductNo = @No)
    BEGIN
        RAISERROR('�L�k�R���A�����~�w�s�b��ߦn�M�椤', 16, 1);
        RETURN;
    END
    
    DELETE FROM Product 
    WHERE No = @No;
END
GO

-- 9. ���o�ϥΪ̩Ҧ��ߦn���İӫ~
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

-- 10. �ھ�SN���o�ߦn���İӫ~
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

-- 11. �s�W�ߦn���İӫ~
CREATE PROCEDURE sp_InsertLikeList
    @UserID NVARCHAR(50),
    @ProductNo INT,
    @OrderQty INT,
    @Account NVARCHAR(20),
    @NewSN INT OUTPUT
AS
BEGIN
    -- �p���`���B�M����O
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

-- 12. ��s�ߦn���İӫ~
CREATE PROCEDURE sp_UpdateLikeList
    @SN INT,
    @ProductNo INT,
    @OrderQty INT,
    @Account NVARCHAR(20)
AS
BEGIN
    -- �p���`���B�M����O
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

-- 13. �R���ߦn���İӫ~
CREATE PROCEDURE sp_DeleteLikeList
    @SN INT
AS
BEGIN
    DELETE FROM LikeList 
    WHERE SN = @SN;
END
GO