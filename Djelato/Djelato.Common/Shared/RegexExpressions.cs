﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Djelato.Common.Shared
{
    public class RegexExpressions
    {
        public const string NameRgx = @"^[a-zA-Z0-9_ -]+$";
        public const string EmailRgx = @"^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";
        public const string PasswordRgx = @"^(?=.*[A-Z])(?=.*\d)(.{8,100})$";
    }
}
