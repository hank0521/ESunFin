using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinancialProducts.Models;

namespace FinancialProducts.Controllers
{
    public class LikeListController : Controller
    {
        private readonly DataAccess _dataAccess;

        public LikeListController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        // GET: LikeList
        public async Task<IActionResult> Index(string userId = null)
        {
            // 如果沒有提供userId，顯示一個使用者選擇介面
            if (string.IsNullOrEmpty(userId))
            {
                var users = await _dataAccess.GetAllUsersAsync();
                ViewBag.Users = new SelectList(users, "UserID", "UserName");
                return View("SelectUser");
            }

            // 獲取指定使用者的喜好清單
            var likeListItems = await _dataAccess.GetUserLikeListAsync(userId);

            // 獲取使用者資訊
            var user = await _dataAccess.GetUserByIdAsync(userId);
            ViewBag.User = user;

            return View(likeListItems);
        }

        // POST: LikeList/SelectUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index), new { userId });
        }

        // GET: LikeList/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var likeListItem = await _dataAccess.GetLikeListBySnAsync(id);
            if (likeListItem == null)
            {
                return NotFound();
            }

            var viewModel = LikeListViewModel.FromLikeListItem(likeListItem);
            return View(viewModel);
        }

        // GET: LikeList/Create?userId=xxx
        public async Task<IActionResult> Create(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Index));
            }

            var users = await _dataAccess.GetAllUsersAsync();
            var products = await _dataAccess.GetAllProductsAsync();

            var viewModel = new LikeListViewModel
            {
                UserID = userId
            };

            // 設置下拉選單
            ViewBag.Users = new SelectList(users, "UserID", "UserName", userId);
            ViewBag.Products = new SelectList(products, "No", "ProductName");

            return View(viewModel);
        }

        // POST: LikeList/Create
        [HttpPost]
        public async Task<IActionResult> Create(LikeListViewModel viewModel)
        {
            
          
                try
                {
                    var likeList = viewModel.ToLikeList();
                    await _dataAccess.InsertLikeListAsync(likeList);
                    return RedirectToAction(nameof(Index), new { userId = viewModel.UserID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"新增喜好金融商品失敗: {ex.Message}");
                }
            

            // 如果有錯誤，重新載入下拉選單
            var users = await _dataAccess.GetAllUsersAsync();
            var products = await _dataAccess.GetAllProductsAsync();

            ViewBag.Users = new SelectList(users, "UserID", "UserName", viewModel.UserID);
            ViewBag.Products = new SelectList(products, "No", "ProductName", viewModel.ProductNo);

            return View(viewModel);
        }

        // GET: LikeList/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var likeListItem = await _dataAccess.GetLikeListBySnAsync(id);
            if (likeListItem == null)
            {
                return NotFound();
            }

            var users = await _dataAccess.GetAllUsersAsync();
            var products = await _dataAccess.GetAllProductsAsync();

            var viewModel = LikeListViewModel.FromLikeListItem(likeListItem);

            // 設置下拉選單
            ViewBag.Users = new SelectList(users, "UserID", "UserName", viewModel.UserID);
            ViewBag.Products = new SelectList(products, "No", "ProductName", viewModel.ProductNo);

            return View(viewModel);
        }

        // POST: LikeList/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, LikeListViewModel viewModel)
        {
            if (id != viewModel.SN)
            {
                return NotFound();
            }

                try
                {
                    var likeList = viewModel.ToLikeList();
                    await _dataAccess.UpdateLikeListAsync(likeList);
                    return RedirectToAction(nameof(Index), new { userId = viewModel.UserID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"更新喜好金融商品失敗: {ex.Message}");
                }
            

            // 如果有錯誤，重新載入下拉選單
            var users = await _dataAccess.GetAllUsersAsync();
            var products = await _dataAccess.GetAllProductsAsync();

            ViewBag.Users = new SelectList(users, "UserID", "UserName", viewModel.UserID);
            ViewBag.Products = new SelectList(products, "No", "ProductName", viewModel.ProductNo);

            return View(viewModel);
        }

        // GET: LikeList/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var likeListItem = await _dataAccess.GetLikeListBySnAsync(id);
            if (likeListItem == null)
            {
                return NotFound();
            }

            var viewModel = LikeListViewModel.FromLikeListItem(likeListItem);
            return View(viewModel);
        }

        // POST: LikeList/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var likeListItem = await _dataAccess.GetLikeListBySnAsync(id);
            if (likeListItem == null)
            {
                return NotFound();
            }

            try
            {
                await _dataAccess.DeleteLikeListAsync(id);
                return RedirectToAction(nameof(Index), new { userId = likeListItem.UserID });
            }
            catch (Exception ex)
            {
                var viewModel = LikeListViewModel.FromLikeListItem(likeListItem);
                ModelState.AddModelError("", $"刪除喜好金融商品失敗: {ex.Message}");
                return View(viewModel);
            }
        }
    }
}