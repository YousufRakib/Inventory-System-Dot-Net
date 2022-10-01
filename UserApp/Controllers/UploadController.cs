using AkraTechFramework.Controllers;
using ExcelDataReader;
using Infrastructure.Core.Provider;
using Infrastrucutre.Core.DataAccess;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader; 

namespace InventoryManager.Controllers
{
    public class UploadController : BaseController
    {
        IItemProvider _itemProvider;
        IListingProvider _listingProvider; 


        public UploadController(IItemProvider itemProvider, IListingProvider listingProvider)
        {
            this._itemProvider = itemProvider;
            this._listingProvider = listingProvider;
        }

        public UploadController()
            : this(new ItemProvider(new ItemRepository()), new ListingProvider(new ListingRepository(), new ItemRepository(), new ItemPriceRepository() ))
        {

        }

        [HttpGet]
        public ActionResult UploadFBAStock()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFBAStock(HttpPostedFileBase fileUpload)
        {
            try
            {
                if (fileUpload.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileUpload.FileName);
                    //string _path = Path.Combine(Server.MapPath("~/UploadedFiles/"), _FileName);
                    //string _path = Server.MapPath("/UploadedFiles/" + _FileName);
                    //C:\inetpub\wwwroot\ArsukInventory\UploadedFiles
                   
                    fileUpload.SaveAs(@"C:\inetpub\wwwroot\ArsukInventory\UploadedFiles\" + _FileName);
                    //fileUpload.SaveAs(@"E:\Projects\Freelancing Projects\ArsukInventory-Develop\02-02-2022\ArsukInventory\UserApp\UploadedFiles\" + _FileName);
                    //fileUpload.SaveAs(Server.MapPath("/UploadedFiles/" + _FileName));
                    //fileUpload.SaveAs(_path);
                }

                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    Stream stream = fileUpload.InputStream;
                    IExcelDataReader reader = null;

                    if (fileUpload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (fileUpload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else if (fileUpload.FileName.EndsWith(".csv"))
                    {
                        reader = ExcelReaderFactory.CreateCsvReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    DataSet vmOrderSheet = reader.AsDataSet();

                    reader.Close();
                    var result = _listingProvider.UploadFDBStockFile(vmOrderSheet);
                    if (result == "1")
                    {
                        ViewBag.Message = "File Uploaded Successfully!!";
                    }
                    else if (result == "2")
                    {
                        ViewBag.Message = "Something went wrong!!";
                    }
                    else
                    {
                        ViewBag.Message = "Make sure the 1st column of this file is seller and 2nd column is location!";
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "File upload failed!! Check the error: (" + ex.Message.ToString() + ")";
                return View();
            }
        }

        [HttpGet]
        public ActionResult OrderSheet()
        {

            return View();
        }

        [HttpPost]
        public ActionResult OrderSheet(HttpPostedFileBase fileUpload)
        {
            try
            {
                if (fileUpload.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileUpload.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                    fileUpload.SaveAs(_path);
                } 

                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = fileUpload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (fileUpload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (fileUpload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else if (fileUpload.FileName.EndsWith(".csv"))
                    {
                        reader = ExcelReaderFactory.CreateCsvReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    //reader.IsFirstRowAsColumnNames = true;

                    DataSet vmOrderSheet = reader.AsDataSet();
                    reader.Close();
                    _listingProvider.UpdateOrderItemsByOrderSheetUpload(vmOrderSheet);  
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }

                ViewBag.Message = "File Uploaded Successfully!!";
                return View();
            }
            catch(Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            } 
        }

    }
}
