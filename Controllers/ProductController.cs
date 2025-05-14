namespace ESunFin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using global::FinancialProducts.Models;

    namespace FinancialProducts.Controllers
    {
        public class ProductController : Controller
        {
            private readonly DataAccess _dataAccess;

            public ProductController(DataAccess dataAccess)
            {
                _dataAccess = dataAccess;
            }

            // GET: Product
            public async Task<IActionResult> Index()
            {
                var products = await _dataAccess.GetAllProductsAsync();
                return View(products);
            }

            // GET: Product/Details/5
            public async Task<IActionResult> Details(int id)
            {
                if (id <= 0)
                {
                    return BadRequest("無效的產品編號");
                }

                var product = await _dataAccess.GetProductByNoAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var viewModel = ProductViewModel.FromProduct(product);
                return View(viewModel);
            }

            // GET: Product/Create
            public IActionResult Create()
            {
                return View(new ProductViewModel());
            }

            // POST: Product/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(ProductViewModel viewModel)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var product = viewModel.ToProduct();
                        await _dataAccess.InsertProductAsync(product);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"新增金融商品失敗: {ex.Message}");
                    }
                }
                return View(viewModel);
            }

            // GET: Product/Edit/5
            public async Task<IActionResult> Edit(int id)
            {
                if (id <= 0)
                {
                    return BadRequest("無效的產品編號");
                }

                var product = await _dataAccess.GetProductByNoAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var viewModel = ProductViewModel.FromProduct(product);
                return View(viewModel);
            }

            // POST: Product/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
            {
                if (id != viewModel.No)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var product = viewModel.ToProduct();
                        await _dataAccess.UpdateProductAsync(product);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"更新金融商品失敗: {ex.Message}");
                    }
                }
                return View(viewModel);
            }

            // GET: Product/Delete/5
            public async Task<IActionResult> Delete(int id)
            {
                if (id <= 0)
                {
                    return BadRequest("無效的產品編號");
                }

                var product = await _dataAccess.GetProductByNoAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var viewModel = ProductViewModel.FromProduct(product);
                return View(viewModel);
            }

            // POST: Product/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                try
                {
                    await _dataAccess.DeleteProductAsync(id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var product = await _dataAccess.GetProductByNoAsync(id);
                    var viewModel = ProductViewModel.FromProduct(product);

                    ModelState.AddModelError("", $"刪除金融商品失敗: {ex.Message}");
                    return View(viewModel);
                }
            }
        }
    }
}
