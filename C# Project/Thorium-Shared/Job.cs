﻿using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using static Thorium_Shared.JobProperties;

namespace Thorium_Shared
{
    public class Job
    {
        public string ID { get; protected set; }
        public string Name { get; protected set; }
        public JObject Information { get; protected set; }

        public Job(string id, string name, JObject info)
        {
            ID = id;
            Information = info;
        }

        protected ATaskProducer taskProducer = null;
        public ATaskProducer TaskProducer
        {
            get
            {
                if(taskProducer == null)
                {
                    string producerClass = Information.Get<string>(TaskProducerType);
                    var ci = Type.GetType(producerClass).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(Job) }, null);
                    if(ci == null)
                    {
                        throw new Exception("the type " + producerClass + " does not have a constructor that takes a job object");
                    }
                    taskProducer = (ATaskProducer)ci.Invoke(new object[] { this });
                }
                return taskProducer;
            }
        }
    }
}