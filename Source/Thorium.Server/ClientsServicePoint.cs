﻿using System.Net;
using Newtonsoft.Json.Linq;
using Thorium.Net.ServiceHost;
using Thorium.Net.ServiceHost.InvokationHandlers;
using Thorium.Shared;
using static Thorium.Shared.Net.ClientToServerCommands;

namespace Thorium.Server
{
    public class ClientsServicePoint
    {
        ThoriumServer server;

        ServiceHost servicePoint;

        public ClientsServicePoint(ThoriumServer thoriumServer)
        {
            server = thoriumServer;

            servicePoint = new ServiceHost("clients_service_point");

            RoutineInvokationHandler rih = new RoutineInvokationHandler();

            rih.RegisterRoutine(new Routine(Register, HandleRegister));
            rih.RegisterRoutine(new Routine(Unregister, HandleUnregister));
            rih.RegisterRoutine(new Routine(TurnInTask, HandleTurnInTask));
            rih.RegisterRoutine(new Routine(AbandonTask, HandleAbandonTask));
            rih.RegisterRoutine(new Routine(FailTask, HandleFailTask));

            rih.RegisterRoutine(new Routine(CheckoutTask, HandleCheckoutTask));

            servicePoint.RegisterInvokationHandler(rih);
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

            Client client = new Client(argObject.Get<string>("clientId"), IPAddress.Parse(argObject.Get<string>("ip")), ClientStatus.Idle);
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

            TaskData task = server.TaskManager.GetAssignableTask();
            if(task != null)
            {
                //TODO: keep track of what client processes what task
                string clientId = argObject.Get<string>("clientId");
                var relation = new ClientTaskRelation(clientId, task.Id);
                server.ClientTaskRelationManager.Add(relation);

                LightweightTask lt = task.ToLightweightTask();
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
