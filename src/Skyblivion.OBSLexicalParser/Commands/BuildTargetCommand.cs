using Skyblivion.OBSLexicalParser.Builds;
using Skyblivion.OBSLexicalParser.Commands.Dispatch;
using Skyblivion.OBSLexicalParser.TES4.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Skyblivion.OBSLexicalParser.Commands
{
    public class BuildTargetCommand : LPCommand
    {
        public const int DefaultThreads = 1;

        public BuildTargetCommand()
            : base("skyblivion:parser:build", "Build Target", "Create artifact(s) from OBScript source")
        {
            Input.AddArgument(new LPCommandArgument("targets", "The build targets", BuildTarget.DEFAULT_TARGETS));
            Input.AddArgument(new LPCommandArgument("threadsNumber", "Threads number", DefaultThreads.ToString()));
            Input.AddArgument(new LPCommandArgument("buildPath", "Build folder", Build.DEFAULT_BUILD_PATH));
        }

        public void Execute(List<LPCommandArgument> input)
        {
            string targets = input.Where(i => i.Name == "targets").First().Value;
            int threadsNumber = int.Parse(input.Where(i => i.Name == "threadsNumber").First().Value);
            string buildPath = input.Where(i => i.Name == "buildPath").First().Value;
            Execute(targets, threadsNumber, buildPath);
        }

        public override void Execute()
        {
            Execute(BuildTarget.DEFAULT_TARGETS);
        }

        public void Execute(string targets, int threadsNumber = DefaultThreads, string buildPath = null)
        {
            if (!PreExecutionChecks(true, true, true, true)) { return; }
            if (buildPath == null) { buildPath = Build.DEFAULT_BUILD_PATH; }
            Build build = new Build(buildPath);
            using (BuildLogServices buildLogServices = new BuildLogServices(build))
            {
                BuildTargetCollection buildTargets = BuildTargetFactory.GetCollection(targets, build, buildLogServices);
                if (!buildTargets.CanBuildAndWarnIfNot()) { return; }
                BuildTracker buildTracker = new BuildTracker(buildTargets);
                Transpile(build, buildTracker, buildTargets, buildLogServices, threadsNumber);
                WriteTranspiled(buildTargets, buildTracker);
                ESMAnalyzer.Deallocate();//Hack - force ESM analyzer deallocation.
                PrepareWorkspace(buildTargets);
                Compile(build, buildTargets);
            }
            Console.WriteLine("Build Complete");
        }

        private static void Transpile(Build build, BuildTracker buildTracker, BuildTargetCollection buildTargets, BuildLogServices buildLogServices, int threadsNumber)
        {
            var buildPlan = buildTargets.GetBuildPlan(threadsNumber);
            int totalScripts = buildPlan.Sum(p => p.Value.Sum(chunk => chunk.Sum(c => c.Value.Count)));
            ProgressWriter progressWriter = new ProgressWriter("Transpiling Scripts", totalScripts);
            using (StreamWriter errorLog = new StreamWriter(build.GetErrorLogPath(), false))
            {
                foreach (var threadBuildPlan in buildPlan)
                {
                    TranspileChunkJob task = new TranspileChunkJob(build, buildTracker, buildLogServices, threadBuildPlan.Value);
                    task.RunTask(errorLog, progressWriter);
                }
            }
            progressWriter.WriteLast();
        }

        private static void WriteTranspiled(BuildTargetCollection buildTargets, BuildTracker buildTracker)
        {
            ProgressWriter writingTranspiledScriptsProgressWriter = new ProgressWriter("Writing Transpiled Scripts", buildTargets.Sum(bt => bt.GetSourceFileList().Count()));
            foreach (var buildTarget in buildTargets)
            {
                buildTarget.Write(buildTracker, writingTranspiledScriptsProgressWriter);
            }
            writingTranspiledScriptsProgressWriter.WriteLast();
        }

        private static void PrepareWorkspace(BuildTargetCollection buildTargets)
        {
            ProgressWriter preparingBuildWorkspaceProgressWriter = new ProgressWriter("Preparing Build Workspace", buildTargets.Count() * PrepareWorkspaceJob.CopyOperationsPerBuildTarget);
            PrepareWorkspaceJob prepareCommand = new PrepareWorkspaceJob(buildTargets);
            prepareCommand.Run(preparingBuildWorkspaceProgressWriter);
            preparingBuildWorkspaceProgressWriter.WriteLast();
        }

        private static void Compile(Build build, BuildTargetCollection buildTargets)
        {
            CompileScriptJob task = new CompileScriptJob(build, buildTargets);
            task.Run();
        }
    }
}