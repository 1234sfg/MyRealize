﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
namespace Realization.Models
{
    public class Managers
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Password { set; get; }
        public string Power { set; get; }
        public string Phone { set; get; }
        public int Identifier { set; get; }
    }
}