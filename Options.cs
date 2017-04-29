// <copyright file="Options.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------
using CommandLine;
using CommandLine.Text;

namespace TcpIPApp
{
  internal enum Mode
  {
    Server,
    Client,
  }

  internal enum ClientFlow
  {
    Sync,
    Async,
    JS,
    NewAsync,
  }

  internal enum DispatcherType
  {
    Thread,
    Pool,
    Dynamic,
  }

  internal enum WorkloadType
  {
    Echo,
    Time,
  }

  public enum LoggerThreshold
  {
    Error = 0,  // only errors are logged
    Minimal,    // minimal info is logged
    Debug,      // everything gets logged
  }

  internal abstract class CommonSubOptions
  {
    [Option('l', "logFileName", HelpText = "Log file name (if none ==> console)")]
    public string LogFileName { get; set; }

    [Option('t', "loggerThreshold", DefaultValue = LoggerThreshold.Debug, HelpText = "Logger threshold (Error, Minimal, Debug)")]
    public LoggerThreshold LoggerThreshold { get; set; }

    [Option('p', "port", DefaultValue = 7777, HelpText = "TCP/IP port")]
    public int Port { get; set; }
  }

  internal class ServerSubOptions : CommonSubOptions
  {
    [Option('d', "dispatcher", DefaultValue = DispatcherType.Thread, HelpText = "Dispatcher [Thread, Pool, Dynamic]")]
    public DispatcherType Dispatcher { get; set; }

    [Option('w', "workload", DefaultValue = WorkloadType.Echo, HelpText = "Workload [Echo, Time]")]
    public WorkloadType Workload { get; set; }

    [Option('r', "realWork", DefaultValue = false, HelpText = "True if server worker is CPU bound (real work), false if not (thread.Sleep())")]
    public bool RealWork { get; set; }

    [Option('i', "minTime", DefaultValue = 0, HelpText = "Minimal workload time in ms")]
    public int MinWorkloadTime { get; set; }

    [Option('x', "maxTime", DefaultValue = 1000, HelpText = "Maximal workload time in ms")]
    public int MaxWorkloadTime { get; set; }

    [Option('n', "numOfConcurentProceses", DefaultValue = 5, HelpText = "Number of workers")]
    public int NumOfConcurentProceses { get; set; }

    [HelpOption]
    public string GetUsage()
    {
      return HelpText.AutoBuild(this,
        (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
    }
  }

  internal class ClientSubOptions : CommonSubOptions
  {
    [Option('f', "flow", DefaultValue = ClientFlow.Sync, HelpText = "Client flow [Sync, Async, JS, NewAsync]")]
    public ClientFlow Flow { get; set; }

    [Option('a', "ipAddress", HelpText = "TCP/IP addrress")]
    public string IPAddress { get; set; }

    [Option('n', "numOfClients", DefaultValue = 5, HelpText = "Number of clients")]
    public int NumOfClients { get; set; }

    [Option('u', "useThreads", DefaultValue = false, HelpText = "Use threads even for async clients")]
    public bool UseThreads { get; set; }

    [HelpOption]
    public string GetUsage()
    {
      return HelpText.AutoBuild(this,
        (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
    }
  }

  internal class Options
  {
    [VerbOption("server", HelpText = "Program acts as server")]
    public ServerSubOptions ServerVerb { get; set; }

    [VerbOption("client", HelpText = "Program acts as client")]
    public ClientSubOptions ClientVerb { get; set; }

    public Mode Mode { get; set; }

    [HelpVerbOption]
    public string GetUsage(string verb)
    {
      return HelpText.AutoBuild(this, verb);
    }
  }
}