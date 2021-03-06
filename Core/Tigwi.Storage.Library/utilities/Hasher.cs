﻿#region copyright
// Copyright (c) 2012, TIGWI
// All rights reserved.
// Distributed under  BSD 2-Clause license
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Tigwi.Storage.Library.Utilities
{
    public class Hasher
    {
        public static Guid Hash(string s)
        {
            return new Guid(MD5.Create().ComputeHash(Encoding.Default.GetBytes(s)));
        }
    }
}
