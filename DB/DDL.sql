-- ��Ʈw���c�w�q (DDL)
CREATE DATABASE FinancialProductsDB;
GO

USE FinancialProductsDB;
GO

-- �ϥΪ̸�T��
CREATE TABLE [User] (
    UserID NVARCHAR(50) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Account NVARCHAR(20) NOT NULL
);
GO

-- ���İӫ~��
CREATE TABLE Product (
    No INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FeeRate DECIMAL(5, 4) NOT NULL -- �O�v�s�x���p�� (0.1 = 10%, 0.01 = 1%)
);
GO

-- �ϥΪ̳ߦn���İӫ~��
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

-- ����
CREATE INDEX IX_LikeList_UserID ON LikeList(UserID);
CREATE INDEX IX_LikeList_ProductNo ON LikeList(ProductNo);
GO

-- ���J��l���
INSERT INTO [User] (UserID, UserName, Email, Account)
VALUES ('A1236456789', '���p��', 'test@email.com', '1111999666');

INSERT INTO Product (ProductName, Price, FeeRate)
VALUES 
('�x�n�qETF', 250.00, 0.01),
('���Ѯ�ETF', 32.50, 0.015),
('�����ު�ETF', 520.75, 0.02),
('���y�Ũ���', 15.25, 0.01),
('�s������ETF', 42.80, 0.025);
GO