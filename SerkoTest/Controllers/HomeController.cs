
using SerkoTestRepository.Models;
using SerkoTest.BusinessRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SerkoTest.Models.DomainToModel;

namespace SerkoTestRepository.Controllers
{
    public class HomeController : Controller
    {
        private IParseTextRepository _parseText;

        public HomeController(IParseTextRepository parseText)
        {
            _parseText = parseText;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(HomeModel homeModel)
        {
            var returnedData = _parseText.ParseMessage(homeModel.Message);

            var convertedObject = new ExpenseModel
            {
                CostCentre = returnedData.CostCentre,
                Date = returnedData.Date,
                Description = returnedData.Description,
                GstCalculated = returnedData.GstCalculated,
                PaymentMethod = returnedData.PaymentMethod,
                Total = returnedData.Total,
                TotaWithoutGST = returnedData.TotaWithoutGST,
                Vendor = returnedData.Vendor,
                HasError = returnedData.HasError
                
            };

            return RedirectToAction("ViewExpense", convertedObject);
        }

        public ActionResult ViewExpense(ExpenseModel expenseModel)
        {
            return View(expenseModel);
        }

        public ActionResult AjaxCallView()
        {
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult FetchClientData(string xmlData)
        {
            var returnedData = _parseText.ParseMessage(xmlData);

            var convertedObject = new ExpenseModel
            {
                CostCentre = returnedData.CostCentre,
                Date = returnedData.Date,
                Description = returnedData.Description,
                GstCalculated = returnedData.GstCalculated,
                PaymentMethod = returnedData.PaymentMethod,
                Total = returnedData.Total,
                TotaWithoutGST = returnedData.TotaWithoutGST,
                Vendor = returnedData.Vendor
            };

            return Json(new { clientData = convertedObject });
        }
    }
}