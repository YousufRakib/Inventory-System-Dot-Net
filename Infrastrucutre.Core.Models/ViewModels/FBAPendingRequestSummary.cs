using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class FBAPendingRequestSummary
    {
        public int PendingList { get; set; }
        public string SellerID { get; set; }
        public string FBARoot { get; set; }
        public decimal MasterCartonQty { get; set; }
        public decimal RequestQty { get; set; }
    }
}
