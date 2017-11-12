﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Thorium_Shared;
using Thorium_Shared.Jobtypes.Blender;
using Thorium_Shared.Logging;
using Thorium_Shared.Net;
using Thorium_Shared.Net.Comms;

namespace Thorium_Server
{
    class Program
    {
        static ThoriumServer server;
        static ConsoleMenu menu;
        static void Main(string[] args)
        {
            Logging.SetupLogging();

            server = new ThoriumServer();
            server.Start();

            Thread.Sleep(1000);

            AddBlenderJob();

            if(args.Contains("-menu"))
            {
                menu = new ConsoleMenu();
                menu.AddMethod("stop", Stop);
                menu.AddMethod("listjobs", ListJobs);
                menu.AddMethod("help", Help);
                Help(null);
                menu.Run();
            }
        }

        static void AddBlenderJob()
        {
            var client = CommsFactory.CreateClient("localhost", ThoriumServerConfig.ListeningPort);

            JObject info = new JObject
            {
                [JobProperties.TaskProducerType] = typeof(BlenderTaskProducer).AssemblyQualifiedName,
                [JobAndTaskProperties.ExecutionerType] = typeof(BlenderExecutioner).AssemblyQualifiedName,
                ["filename"] = @"C:\Users\Freddy\Desktop\sarfis_test.blend",
                ["blenderExecutable"] = @"E:\Program Files (x86)\Steam\SteamApps\common\Blender\blender.exe",
                ["framesStart"] = 34,
                ["framesEnd"] = 90,
            };

            JObject arg = new JObject
            {
                ["jobName"] = "test blender job",
                ["jobInformation"] = info
            };

            client.Invoke(ServerControlCommands.AddJob, arg);

        }

        static void Stop(string[] args)
        {
            server.Stop();
            menu.Stop();
        }

        static void ListJobs(string[] args)
        {
            Console.WriteLine("Jobs:");
            /*foreach(var j in server.JobManager.Jobs) {
                Console.WriteLine(j.Value.ID);
            }*/
        }

        static void Help(string[] args)
        {
            Console.WriteLine("available commands:");
            Console.WriteLine("Stop");
            Console.WriteLine("ListTasks");
            Console.WriteLine("Help");
        }
    }
}
