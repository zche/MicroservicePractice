﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public class TestAuthCodeService : IAuthCodeService
    {
        public bool Validate(string tel, string authCode)
        {
            return true;
        }
    }
}
