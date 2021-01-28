using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.Providers
{
    public class Cookies:Controller
    {
        public Cookies()
        {
            //empty constructor
        }
        public string Getcookie(string name)
        {
            return Request.Cookies[name];
        }

    }
}
