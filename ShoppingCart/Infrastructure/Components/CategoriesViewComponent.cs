﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Infrastructure.Components
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public CategoriesViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Categories.ToListAsync());

    }
}
