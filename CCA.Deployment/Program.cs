using CCA.Deployment;
using Pulumi;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class Program
{
    static Task<int> Main()
    {
        BuildProject(@"../CCA.User.Service");
        BuildProject(@"../CCA.Event.Service");
        BuildProject(@"../CCA.Meeting.Service");
        BuildProject(@"../CCA.DialIn.Service");
        BuildProject(@"../CCA.Health.Service");

        Console.WriteLine("Completed build steps");

        return Deployment.RunAsync<CCAStack>();
    }

    private static void BuildProject(string directory)
    {
        Console.WriteLine($"Building project at: {directory}");

        var processStartInfo = new ProcessStartInfo("dotnet", "publish --configuration Release --nologo --verbosity q")
        {
            WorkingDirectory = directory
        };

        var process = Process.Start(processStartInfo);
        process.WaitForExit();
    }
}
