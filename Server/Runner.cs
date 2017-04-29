// <copyright file="Runner.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using TcpIPApp.Dispatcher;
using TcpIPApp.Logger;
using TcpIPApp.WorkLoad;

namespace TcpIPApp.Server
{
  internal class Runner
  {
    public int Run(Options options)
    {
      this.options = options;
      this.parseBaseElements();

      runSyncServer(options);

      return 0;
    }

    private void runSyncServer(Options options)
    {
      TcpListener listener = new TcpListener(IPAddress.Any, options.ServerVerb.Port);
      listener.Start();

      this.dispatcher.Dispatch(listener, this.logger, this.workload.Workload);
    }

    private void parseBaseElements()
    {
      ServerSubOptions srvOptions = this.options.ServerVerb;

      switch (srvOptions.Dispatcher)
      {
        case DispatcherType.Thread:
          this.dispatcher = new ThreadDispatcher();
          break;

        case DispatcherType.Pool:
          this.dispatcher = new PoolDispatcher(srvOptions.NumOfConcurentProceses);
          break;

        case DispatcherType.Dynamic:
          this.dispatcher = new DynamicDispatcher(srvOptions.NumOfConcurentProceses);
          break;
      }

      switch (srvOptions.Workload)
      {
        case WorkloadType.Echo:
          this.workload = new Echo(srvOptions.MinWorkloadTime, srvOptions.MaxWorkloadTime, srvOptions.RealWork);
          break;

        case WorkloadType.Time:
          this.workload = new Time(srvOptions.MinWorkloadTime, srvOptions.MaxWorkloadTime, srvOptions.RealWork);
          break;
      }

      this.logger = (string.IsNullOrEmpty(srvOptions.LogFileName)) ?
        (ILogger)(new ConsoleLogger()) :
        (ILogger)(new FileLogger(srvOptions.LogFileName));
      this.logger.LogThreshold = srvOptions.LoggerThreshold;

      if (srvOptions.RealWork)
        this.logger.Log(LoggerThreshold.Debug, "real work");
      else
        this.logger.Log(LoggerThreshold.Debug, "NOT real work");
    }

    private Options options;
    private IDispatcher dispatcher;
    private IWorkload workload;
    private ILogger logger;
  }
}