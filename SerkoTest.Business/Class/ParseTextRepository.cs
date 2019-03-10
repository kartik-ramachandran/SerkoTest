using SerkoTest.BusinessRepository.Interface;
using SerkoTest.BusinessRepository.Objects;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SerkoTest.BusinessRepository.Class
{

    public class ParseTextRepository : IParseTextRepository
    {
        //get from configs if possible
        private readonly List<string> _KeyWords = new List<string> { "cost_centre", "total", "payment_method", "vendor", "description", "date" };

        public ExpenseClaim ParseMessage(string message)
        {

            var expenseClaim = new ExpenseClaim();

            if (IsValidXml(message))
            {
                ExtractDataFromValidXml(message, expenseClaim);
            }
            else
            {
                ExtractXmlDataFromMessage(message, expenseClaim);
            }

            return expenseClaim;
        }

        private void ExtractDataFromValidXml(string message, ExpenseClaim expenseClaim)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(message);

            // Get elements
            expenseClaim.CostCentre = GetInnerText(xmlDoc, "cost_centre");
            expenseClaim.Total = GetInnerText(xmlDoc, "total");
            expenseClaim.PaymentMethod = GetInnerText(xmlDoc, "payment_method");
            expenseClaim.Vendor = GetInnerText(xmlDoc, "vendor");
            expenseClaim.Description = GetInnerText(xmlDoc, "description");
            expenseClaim.Date = GetInnerText(xmlDoc, "date");

            expenseClaim.HasError = HasError(xmlDoc, "total");

            if (expenseClaim.HasError)
            {
                return; //no point calculating GST and other things if total is missing
            }

            CalculateAdditionalData(expenseClaim);
        }

        private string GetInnerText(XmlDocument xmlDoc, string tagName)
        {
            if (xmlDoc.GetElementsByTagName(tagName)[0] != null)
            {
                return xmlDoc.GetElementsByTagName(tagName)[0].InnerText;
            }

            if (xmlDoc.GetElementsByTagName(tagName)[0] == null && tagName.Equals("cost_centre"))
            {
                return "UNKNOWN";
            }          

            return string.Empty;
        }

        private bool HasError(XmlDocument xmlDoc, string tagName)
        {
            if (xmlDoc.GetElementsByTagName(tagName)[0] == null && tagName.Equals("total"))
            {
                return true;
            }

            return false;
        }

        private void ExtractXmlDataFromMessage(string message, ExpenseClaim expenseClaim)
        {
            _KeyWords.ForEach((item) =>
            {
                var pattern = "<" + item + ">(.*?)</" + item + ">";

                var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                var m = regex.Match(message);

                if (m.Success)
                {
                    ExtractDataFrommessage(item, expenseClaim, m);

                    CalculateAdditionalData(expenseClaim);
                }
                else
                {
                    if (item.Equals("cost_centre"))
                    {
                        expenseClaim.CostCentre = "UNKNOWN";
                    }
                    else
                    {
                        expenseClaim.HasError = true;
                    }
                }
            });
        }

        private void ExtractDataFrommessage(string item, ExpenseClaim expenseClaim, Match m)
        {
            if (item.Equals("cost_centre"))
            {
                expenseClaim.CostCentre = m.Groups[1].Value;
            }

            if (item.Equals("total"))
            {
                expenseClaim.Total = m.Groups[1].Value;
            }

            if (item.Equals("payment_method"))
            {
                expenseClaim.PaymentMethod = m.Groups[1].Value;
            }

            if (item.Equals("vendor"))
            {
                expenseClaim.Vendor = m.Groups[1].Value;
            }

            if (item.Equals("description"))
            {
                expenseClaim.Description = m.Groups[1].Value;
            }

            if (item.Equals("date"))
            {
                expenseClaim.Date = m.Groups[1].Value;
            }
        }

        private void CalculateAdditionalData(ExpenseClaim expenseClaim)
        {
            //assuming GST was calculated at 15%
            expenseClaim.TotaWithoutGST = GetTotaWithoutGST(expenseClaim.Total).ToString();

            expenseClaim.GstCalculated = GetGstCalculated
                (Convert.ToDouble(expenseClaim.Total), Convert.ToDouble(expenseClaim.TotaWithoutGST)).ToString();
        }

        private double GetGstCalculated(double total, double gstCalculated)
        {
            return Math.Round(total - gstCalculated, 3);
        }

        private double GetTotaWithoutGST(string total)
        {
            return Math.Round((Convert.ToDouble(total) / 1.15), 3);
        }

        private bool IsValidXml(string xml)
        {
            try
            {
                XDocument.Parse(xml);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
