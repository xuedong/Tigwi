﻿#region copyright
// Copyright (c) 2012, TIGWI
// All rights reserved.
// Distributed under  BSD 2-Clause license
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tigwi.UI.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Tigwi.UI.Models.Storage;
    public class AccountDeleteViewModel
    {
        //Not Required
        public Guid deleteAccountId { get; set; }
        [AllowHtml]
        public string deleteAccountName { get; set; }
    }
}
