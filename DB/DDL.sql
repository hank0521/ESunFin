-- 資料庫結構定義 (DDL)
CREATE DATABASE FinancialProductsDB;
GO

USE FinancialProductsDB;
GO

-- 使用者資訊表
CREATE TABLE [User] (
    UserID NVARCHAR(50) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Account NVARCHAR(20) NOT NULL
);
GO

-- 金融商品表
CREATE TABLE Product (
    No INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FeeRate DECIMAL(5, 4) NOT NULL -- 費率存儲為小數 (0.1 = 10%, 0.01 = 1%)
);
GO

-- 使用者喜好金融商品表
CREATE TABLE LikeList (
    SN INT IDENTITY(1,1) PRIMARY KEY,
    UserID NVARCHAR(50) NOT NULL,
    ProductNo INT NOT NULL,
    OrderQty INT NOT NULL,
    Account NVARCHAR(20) NOT NULL,
    TotalFee DECIMAL(18, 2) NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES [User](UserID),
    FOREIGN KEY (ProductNo) REFERENCES Product(No)
);
GO

-- 索引
CREATE INDEX IX_LikeList_UserID ON LikeList(UserID);
CREATE INDEX IX_LikeList_ProductNo ON LikeList(ProductNo);
GO

-- 插入初始資料
INSERT INTO [User] (UserID, UserName, Email, Account)
VALUES ('A1236456789', '王小明', 'test@email.com', '1111999666');

INSERT INTO Product (ProductName, Price, FeeRate)
VALUES 
('台積電ETF', 250.00, 0.01),
('高股息ETF', 32.50, 0.015),
('美國科技股ETF', 520.75, 0.02),
('全球債券基金', 15.25, 0.01),
('新興市場ETF', 42.80, 0.025);
GO