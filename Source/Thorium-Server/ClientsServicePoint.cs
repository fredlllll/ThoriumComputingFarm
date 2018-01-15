﻿using System.Net;
using Newtonsoft.Json.Linq;
using Thorium_Shared;
using Thorium_Shared.Net.ServicePoint;
using static Thorium_Shared.Net.ClientToServerCommands;

namespace Thorium_Server
{
    public class ClientsServicePoint
    {
        ThoriumServer server;

        Thorium_Shared.Net.ServicePoint.ServicePoint servicePoint;

        public ClientsServicePoint(ThoriumServer thoriumServer, int port)
        {
            server = thoriumServer;

            servicePoint = new Thorium_Shared.Net.ServicePoint.ServicePoint("clients_service_point");

            servicePoint.RegisterRoutine(new Routine(Register, HandleRegister));
            servicePoint.RegisterRoutine(new Routine(Unregister, HandleUnregister));
            servicePoint.RegisterRoutine(new Routine(CheckoutTask, HandleCheckoutTask));
            servicePoint.RegisterRoutine(new Routine(TurnInTask, HandleTurnInTask));
            servicePoint.RegisterRoutine(new Routine(AbandonTask, HandleAbandonTask));
        }

        public void Start()
        {
            servicePoint.Start();
        }

        public void Stop()
        {
            servicePoint.Stop();
        }

        JToken HandleRegister(JToken arg)
        {
            JObject argObject = arg as JObject;

            Client client = new Client(IPAddress.Parse(argObject.Get<string>("ip")), argObject.Get<string>("clientId"));
            server.ClientManager.RegisterClient(client);

            return null;
        }

        JToken HandleUnregister(JToken arg)
        {
            JObject argObject = arg as JObject;

            server.ClientManager.UnregisterClient(argObject.Get<string>("clientId"));

            return null;
        }

        JToken HandleCheckoutTask(JToken arg)
        {
            JObject argObject = arg as JObject;

            Task task = server.TaskManager.CheckoutTask();
            if(task != null)
            {
                //TODO: keep track of what client processes what task
                string clientId = argObject.Get<string>("clientId");
                var relation = new ClientTaskRelation(clientId, task.ID);
                server.ClientTaskRelationManager.Add(relation);

                var job = server.DataManager.JobSerializer.Load(task.JobID);
                LightweightTask lt = new LightweightTask(task, job);
                JObject retval = JObject.FromObject(lt);
                return retval;
            }
            return null;
        }

        JToken HandleTurnInTask(JToken arg)
        {
            JObject argObject = arg as JObject;

            string id = argObject.Get<string>("taskId");
            server.TaskManager.TurnInTask(id);
            server.ClientTaskRelationManager.RemoveByTask(id);

            return null;
        }

        JToken HandleAbandonTask(JToken arg)
        {
            JObject argObject = arg as JObject;

            string taskId = argObject.Get<string>("taskId");
            server.TaskManager.AbandonTask(taskId, argObject.Get<string>("reason"));
            server.ClientTaskRelationManager.RemoveByTask(taskId);

            return null;
        }

        JToken HandleFailTask(JToken arg)
        {
            JObject argObject = arg as JObject;

            string taskId = argObject.Get<string>("taskId");
            server.TaskManager.FailTask(taskId, argObject.Get<string>("reason"));
            server.ClientTaskRelationManager.RemoveByTask(taskId);

            return null;
        }
    }
}