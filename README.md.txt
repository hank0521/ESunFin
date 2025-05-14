# 金融商品管理系統

這是一個使用ASP.NET Core MVC開發的金融商品管理系統，可以管理使用者喜好的金融商品。

## 功能特色

- 新增、查詢、刪除和更新喜好金融商品
- 使用者可管理金融商品資訊（產品名稱、產品價格、手續費率）
- 計算預計扣款總金額和總手續費
- 支援RWD響應式設計

## 技術實現

- ASP.NET Core 6 MVC架構
- 使用三層式架構設計（MVC模式）
- 使用SQL Server資料庫和Stored Procedure
- Bootstrap實現RWD介面
- 實作資料庫交易(Transaction)，確保資料一致性
- 防止SQL Injection及XSS攻擊