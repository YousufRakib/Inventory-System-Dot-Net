using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.Models
{
    public class FDBStockQueryResult
    {
        public int ItemMasterID { get; set; }
        public string SKU { get; set; }
        public string Location { get; set; }
        public string Seller { get; set; }
        public int AfnTotalQuantity { get; set; }
    }
}
