﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Thorium_Shared
{
    public abstract class AJob
    {
        public string ID { get; protected set; }
        public string Name { get; protected set; }
        public JObject Information { get; protected set; }

        public AJob(string id, string name, JObject info)
        {
            ID = id;
            Information = info;
        }

        public abstract ATaskProducer TaskProducer {get;}
    }
}
