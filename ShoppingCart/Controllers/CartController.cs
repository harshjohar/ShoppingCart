﻿using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Infrastructure;
using ShoppingCart.Models;
using ShoppingCart.Models.ViewModels;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartViewModel cartVM = new()
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Quantity * x.Price),
            };

            return View(cartVM);
        }

        public async Task<IActionResult> Add(long id)
        {
            Product product = await _dataContext.Products.FindAsync(id);
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();
            if (cartItem == null)
            {
                cart.Add(new CartItem(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);

            TempData["Success"] = "The product has been added";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Decrease(long id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartItem cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == id);
            }
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["Success"] = "The product has been removed";
            return RedirectToAction("Index");
        }
        public IActionResult Remove(long id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            cart.RemoveAll(p => p.ProductId == id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["Success"] = "The product has been removed";
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            
            return RedirectToAction("Index");
        }
    }
}
