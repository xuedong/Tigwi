﻿#region copyright
// Copyright (c) 2012, TIGWI
// All rights reserved.
// Distributed under  BSD 2-Clause license
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tigwi.UI.Models.Storage;

namespace Tigwi.UI.Models
{
    using System.Web.Mvc;

    public class GenerateApiKeyViewModel
    {
        [Required]
        [StringLength(60)]
        [Display(Name = "Application name")]
        [AllowHtml]
        public string ApplicationName { get; set; }
    }
 }