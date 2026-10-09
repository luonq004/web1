using Lab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.ViewComponents
{
    public class LeftMenuViewComponent : ViewComponent
    {
        private List<MenuItem> menuItems = new List<MenuItem>
        {
            new MenuItem { Text = "Home", Url = "/" },
            new MenuItem { Text = "Students", Url = "/Student/Index" },
            new MenuItem { Text = "Contact", Url = "/contact" },
            new MenuItem { Text = "Services", Url = "/services" },
            new MenuItem { Text = "Products", Url = "/products" }
        };

        public IViewComponentResult Invoke()
        {
            return View("Menu", menuItems);
        }

        //public async Task<IViewComponentResult> InvokeAsync()
        //{
        //    return View("Menu", menuItems);
        //}
    }
}
