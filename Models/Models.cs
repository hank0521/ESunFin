using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialProducts.Models
{
    // 基本使用者模型
    public class User
    {
        [Display(Name = "使用者ID")]
        public string UserID { get; set; }

        [Display(Name = "使用者名稱")]
        public string UserName { get; set; }

        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Display(Name = "帳號")]
        public string Account { get; set; }
    }

    // 基本產品模型
    public class Product
    {
        [Display(Name = "編號")]
        public int No { get; set; }

        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }

        [Display(Name = "產品價格")]
        public decimal Price { get; set; }

        [Display(Name = "手續費率")]
        public decimal FeeRate { get; set; }
    }

    // 基本喜好清單模型
    public class LikeList
    {
        [Display(Name = "序號")]
        public int SN { get; set; }

        [Display(Name = "使用者ID")]
        public string UserID { get; set; }

        [Display(Name = "產品編號")]
        public int ProductNo { get; set; }

        [Display(Name = "購買數量")]
        public int OrderQty { get; set; }

        [Display(Name = "扣款帳號")]
        public string Account { get; set; }

        [Display(Name = "總手續費")]
        public decimal TotalFee { get; set; }

        [Display(Name = "預計扣款總金額")]
        public decimal TotalAmount { get; set; }
    }

    // 喜好清單項目模型（用於顯示，包含更多資訊）
    public class LikeListItem : LikeList
    {
        [Display(Name = "產品名稱")]
        public string ProductName { get; set; }

        [Display(Name = "產品價格")]
        public decimal Price { get; set; }

        [Display(Name = "手續費率")]
        public decimal FeeRate { get; set; }

        [Display(Name = "電子郵件")]
        public string Email { get; set; }
    }

    // 產品視圖模型（用於表單處理）
    public class ProductViewModel
    {
        public int No { get; set; }

        [Required(ErrorMessage = "請輸入產品名稱")]
        [Display(Name = "產品名稱")]
        [StringLength(100, ErrorMessage = "產品名稱最多100個字元")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "請輸入產品價格")]
        [Display(Name = "產品價格")]
        [Range(0.01, 1000000, ErrorMessage = "產品價格必須大於0")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "請輸入手續費率")]
        [Display(Name = "手續費率(%)")]
        [Range(0, 100, ErrorMessage = "手續費率必須在0到100之間")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal FeeRatePercent { get; set; }

        // 從 FeeRatePercent 轉換到 FeeRate (例：3.5% -> 0.035)
        public decimal FeeRate => FeeRatePercent / 100;

        // 從 Product 轉換到 ProductViewModel
        public static ProductViewModel FromProduct(Product product)
        {
            if (product == null)
                return null;

            return new ProductViewModel
            {
                No = product.No,
                ProductName = product.ProductName,
                Price = product.Price,
                FeeRatePercent = product.FeeRate * 100 // 轉為百分比 (例：0.035 -> 3.5)
            };
        }

        // 從 ProductViewModel 轉換到 Product
        public Product ToProduct()
        {
            return new Product
            {
                No = this.No,
                ProductName = this.ProductName,
                Price = this.Price,
                FeeRate = this.FeeRate // 使用屬性轉換為小數
            };
        }
    }

    // 喜好清單視圖模型（用於表單處理）
    public class LikeListViewModel
    {
        public int SN { get; set; }

        [Required(ErrorMessage = "請選擇使用者")]
        [Display(Name = "使用者ID")]
        public string UserID { get; set; }

        [Required(ErrorMessage = "請選擇產品")]
        [Display(Name = "產品")]
        public int ProductNo { get; set; }

        [Required(ErrorMessage = "請輸入購買數量")]
        [Display(Name = "購買數量")]
        [Range(1, 1000000, ErrorMessage = "購買數量必須大於0")]
        public int OrderQty { get; set; }

        [Required(ErrorMessage = "請輸入扣款帳號")]
        [Display(Name = "扣款帳號")]
        [StringLength(20, ErrorMessage = "帳號最多20個字元")]
        public string Account { get; set; }

        // 產品相關資料 (僅用於顯示)
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal FeeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalFee { get; set; }

        // 使用者相關資料 (僅用於顯示)
        public string UserName { get; set; }
        public string Email { get; set; }

        // 從 LikeListItem 轉換到 LikeListViewModel
        public static LikeListViewModel FromLikeListItem(LikeListItem item)
        {
            if (item == null)
                return null;

            return new LikeListViewModel
            {
                SN = item.SN,
                UserID = item.UserID,
                ProductNo = item.ProductNo,
                OrderQty = item.OrderQty,
                Account = item.Account,
                ProductName = item.ProductName,
                Price = item.Price,
                FeeRate = item.FeeRate,
                TotalAmount = item.TotalAmount,
                TotalFee = item.TotalFee,
                Email = item.Email
            };
        }

        // 從 LikeListViewModel 轉換到 LikeList
        public LikeList ToLikeList()
        {
            return new LikeList
            {
                SN = this.SN,
                UserID = this.UserID,
                ProductNo = this.ProductNo,
                OrderQty = this.OrderQty,
                Account = this.Account
            };
        }
    }
}