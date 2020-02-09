﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LapTrinhWeb.Utility
{
    public class SD
    {
        public const string DefaultProductImage = "default_image.png";
        public const string ImageFolder = @"images\ProductImage";

        public const string AdminEndUser = "Admin";
        public const string SuperAdminEndUser = "Super Admin";
        public const string MultiRole = AdminEndUser + "," + SuperAdminEndUser;
    }
}
