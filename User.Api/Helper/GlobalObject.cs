﻿using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Api.Helper
{
    public class GlobalObject
    {
        public static IApplicationBuilder App { get; set; }
    }
}