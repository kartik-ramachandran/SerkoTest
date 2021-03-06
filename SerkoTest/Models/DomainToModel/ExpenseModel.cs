﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SerkoTest.Models.DomainToModel
{
    public class ExpenseModel
    {
        public string CostCentre { get; set; }
        public string Total { get; set; }
        public string PaymentMethod { get; set; }
        public string Vendor { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string GstCalculated { get; set; }
        public string TotaWithoutGST { get; set; }
        public bool HasError { get; set; }
    }
}