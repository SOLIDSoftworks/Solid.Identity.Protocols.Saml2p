using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Utilities.Razor.Tests.Pages
{
    public class TestPageModel : PageModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public void OnGet()
        {
        }
    }
}