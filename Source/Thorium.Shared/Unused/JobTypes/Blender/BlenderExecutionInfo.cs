﻿/*using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Thorium.Shared.ExecutionActions;
using Thorium.Shared.Services;
using Thorium.Shared.Services.Client;
using Thorium.Shared.Services.Server;
using Codolith.Config;
using Thorium.Shared.WCF;

namespace Thorium.Shared.Blender
{
    public class BlenderExecutionInfo : ITaskExecutionInfo
    {
        //Config data;

        string jobID, taskID;
        int frame;
        int tilesPerFrame;
        int tile;
        Layer[] layers;
        string filename;
        Resolution resolution;

        DirectoryInfo workingDir;
        string outputFileName;

        public BlenderExecutionInfo(Config data)
        {
            //this.data = data;

            jobID = data.Get("jobID");
            taskID = data.Get("taskID");
            frame = data.Get<int>("frame");
            tilesPerFrame = data.Get<int>("tilesPerFrame");
            tile = data.Get<int>("tile");
            layers = data.Get("layers").Split(',').Select((x) => { return Layer.Parse(x); }).ToArray();
            filename = data.Get("filename");
            resolution = Resolution.Parse(data.Get("resolution"));
        }

        public void Setup()
        {
            //clientServiceManager = SharedData.Get<AServiceManager<IClientService>>(ClientConfigConstants.SharedDataID_ClientServiceManager);
            var provider = ServiceManager.Instance.GetService<DataPackageProviderClient>();

            workingDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            if(workingDir.Exists)
            {
                workingDir.Delete();
            }
            Directory.CreateDirectory(workingDir.FullName);
            provider.UnpackPackageIntoDirectory(jobID, workingDir);
        }

        public void Run()
        {
            outputFileName = Path.GetRandomFileName();
            string outputFile = Path.Combine(Path.GetTempPath(), outputFileName);
            //run blender
            RunExecutableAction rea = new RunExecutableAction();
            rea.ExecutionFolder = workingDir.FullName;
            rea.ExecutableFile = "blender";
            //https://www.blender.org/manual/advanced/command_line/arguments.html
            rea.Arguments = new string[] {
                "-b", //console mode
                Path.Combine(workingDir.FullName,filename), //filename
                "-y", // auto run python
                /*output file/
                "-o", outputFile,
                /*format to render to/
                "-F",  "PNG",
                 /*render engine to use/
                "-E","Cycles",
                 /*run this python file, can be in there multiple times/ 
                //"-P","pythonscripttorun.py",
                "-a", //use settings from blend file
                /* -s 1 -e 2 would mean frames 1 and 2, for future use/
                "-f", frame.ToString(),
                "--" //blender stops parsing after this, but can be parsed by python scripts
                /*
                import sys
argv = sys.argv
try:
    index = argv.index("--") + 1
except ValueError:
    index = len(argv)

argv = argv[index:]
                /
            };
            rea.ExecuteAndWaitForExit();

            string[] files = Directory.GetFiles(Path.GetTempPath(), outputFileName + "*");
            var serverInterface = DependencyInjection.Kernel.Get<IThoriumServerInterfaceForClient>();

            var address = serverInterface.GetServicePath(typeof(IResultsCache));
            var cache = WCFServiceManager.Instance.GetServiceInstance<IResultsCache>(address);
            cache.RegisterResult(jobID + taskID, File.ReadAllBytes(files[0]));
            WCFServiceManager.Instance.FreeServiceInstance<IResultsCache>();
        }

        public void Cleanup()
        {
            workingDir.Delete();
        }
    }
}
*/