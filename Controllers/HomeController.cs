using Microsoft.AspNetCore.Mvc;
using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.IO;
using System.Web;

namespace LeitorCSV.Controllers
{
    public class HomeController : Controller
    {
        //GET: Home
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload (IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.Length > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        Stream stream = upload.OpenReadStream();
                        DataTable csvTable = new DataTable();
                        using (CsvReader csvReader =
                            new CsvReader(new StreamReader(stream), true))
                        {
                            csvTable.Load(csvReader);
                        }
                        return View(csvTable);
                    }
                    else
                    {
                        ModelState.AddModelError("Arquivo", "Formato do arquivo não é suportado");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("Arquivo", "Faça o Upload do arquivo");
                }
            }
            return View();
        }

    }
}
