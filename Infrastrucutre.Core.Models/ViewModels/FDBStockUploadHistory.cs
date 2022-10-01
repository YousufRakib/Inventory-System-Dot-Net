using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Core.Models.ViewModels
{
    public class FDBStockUploadHistory
    {
        public string FNSKU { get; set; }
        public int ItemMasterID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedDateString
        {
            get
            {
                return string.Format("{0:dd/MM/yyyy}", this.CreatedDate);
            }
        }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedDateString
        {
            get
            {
                return string.Format("{0:dd/MM/yyyy}", this.UpdatedDate);
            }
        }
        public string FBARootName { get; set; }
        public string FBASellerName { get; set; }
    }
}
