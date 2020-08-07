using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class CategoryAndSubCategoryViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategories { get; set; }
        public string StatusMessage { get; set; }
    }
}
