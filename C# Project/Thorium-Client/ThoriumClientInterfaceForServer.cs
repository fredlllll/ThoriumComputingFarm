﻿using System;
using Thorium_Shared;

namespace Thorium_Client
{
    public class ThoriumClientInterfaceForServer : IThoriumClientInterfaceForServer
    {
        string id = Util.GetRandomID();

        protected string currentTaskID;
        public string GetCurrentTaskID()
        {
            return currentTaskID;
        }

        public void SetCurrentTaskID(string id)
        {
            currentTaskID = id;
        }

        public string GetID()
        {
            return id;
        }

        public void AbortTask(string ID)
        {
            throw new NotImplementedException();
        }

        public void Ping()
        {
            //dummy function to execute ping
        }
    }
}
