// <copyright file="Runner.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using TcpIPApp.Logger;

namespace TcpIPApp.Client
{
  internal class Runner
  {
    public int Run(Options options)
    {
      init(options);

      if (!this.clientOpt.UseThreads)
      {
        if (this.clientOpt.Flow == ClientFlow.JS)
          return this.runJS();
        if (this.clientOpt.Flow == ClientFlow.NewAsync)
          return this.runAsync();
      }

      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();

      Thread[] threads = new Thread[this.clientOpt.NumOfClients];
      for (int i = 0; i < threads.Length; i++)
      {
        threads[i] = new Thread(() =>
        {
          string text = $"Hi from client [{i}]";
          IClient client = this.createNewClient();
          client.Process(this.localEndPoint, this.logger, text);
        });

        threads[i].Start();
      }

      for (int i = 0; i < options.ClientVerb.NumOfClients; i++)
        threads[i].Join();

      this.logger.Log(LoggerThreshold.Debug, $"RunTime in ms = {stopWatch.ElapsedMilliseconds}");
      return 0;
    }

    private int runJS()
    {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();

      CountdownEvent cde = new CountdownEvent(this.clientOpt.NumOfClients);
      for (int i = 0; i < this.clientOpt.NumOfClients; i++)
      {
        string text = $"Hi from client [{i}]";
        JSLikeASyncClient client = new JSLikeASyncClient();
        client.Execute(this.localEndPoint, this.logger, text, (Exception ex, string value) =>
        {
          cde.Signal();
          //this.logger.Log(LoggerThreshold.Debug, $"Client received [{value}]");
        });
      }

      cde.Wait();

      this.logger.Log(LoggerThreshold.Debug, $"RunTime in ms = {stopWatch.ElapsedMilliseconds}");
      return 0;
    }

    private int runAsync()
    {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();

      CountdownEvent cde = new CountdownEvent(this.clientOpt.NumOfClients);
      for (int i = 0; i < this.clientOpt.NumOfClients; i++)
      {
        string text = $"Hi from client [{i}]";
        NewASyncClient client = new NewASyncClient();
        client.Execute(this.localEndPoint, this.logger, text).ContinueWith((returnedTask) =>
        {
          cde.Signal();
          //this.logger.Log(LoggerThreshold.Debug, $"Client received [{returnedTask.Result}]");
        });
      }

      cde.Wait();

      this.logger.Log(LoggerThreshold.Debug, $"RunTime in ms = {stopWatch.ElapsedMilliseconds}");
      return 0;
    }

    private void init(Options options)
    {
      this.options = options;
      this.clientOpt = this.options.ClientVerb;
      this.ipAddress = IPAddress.Parse(this.clientOpt.IPAddress);
      this.localEndPoint = new IPEndPoint(this.ipAddress, this.clientOpt.Port);

      this.logger = (string.IsNullOrEmpty(this.clientOpt.LogFileName)) ?
        (ILogger)(new ConsoleLogger()) :
        (ILogger)(new FileLogger(this.clientOpt.LogFileName));
      this.logger.LogThreshold = this.clientOpt.LoggerThreshold;
    }

    private IClient createNewClient()
    {
      switch (this.clientOpt.Flow)
      {
        case ClientFlow.Sync:
          return new SyncClient();

        case ClientFlow.Async:
          return new ClasicASyncClient();

        case ClientFlow.JS:
          return new JSLikeASyncClient();

        default:
          return new NewASyncClient();
      }
    }

    private Options options;
    private ClientSubOptions clientOpt;
    private IPAddress ipAddress;
    private IPEndPoint localEndPoint;
    private ILogger logger;
  }
}