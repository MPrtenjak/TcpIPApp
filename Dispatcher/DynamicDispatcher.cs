// <copyright file="DynamicDispatcher.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using TcpIPApp.Logger;
using TcpIPApp.Server;

namespace TcpIPApp.Dispatcher
{
  internal class DynamicDispatcher : IDispatcher
  {
    public void Dispatch(TcpListener listener, ILogger logger, Func<string, string> workload)
    {
      this.listener = listener;
      this.logger = logger;
      this.threadWorkload = workload;

      Thread threadWork = new Thread(new ThreadStart(processThreadQueue));
      threadWork.Start();

      while (true)
      {
        try
        {
          TcpClient tcpClient = this.listener.AcceptTcpClient();
          this.queue.Enqueue(tcpClient);
        }
        catch (System.IO.IOException e)
        {
          this.logger.Log(LoggerThreshold.Error, "Exception = " + e.Message);
        }
      }
    }

    public DynamicDispatcher(int maxWorkers)
    {
      this.maxConcurentThreads = new SemaphoreSlim(maxWorkers);
    }

    private void doThreadWork(object objServer)
    {
      var server = objServer as SyncServer;
      server.Run();
      this.maxConcurentThreads.Release();
    }

    private void processThreadQueue()
    {
      while (true)
      {
        try
        {
          TcpClient tcpClient;
          if (this.queue.TryDequeue(out tcpClient))
          {
            this.maxConcurentThreads.Wait();
            var server = new SyncServer(tcpClient, this.logger, this.threadWorkload);
            var workingThread = new Thread(new ParameterizedThreadStart(this.doThreadWork));
            workingThread.Start(server);
          }
          else
            Thread.Sleep(100);
        }
        catch (System.IO.IOException e)
        {
          this.logger.Log(LoggerThreshold.Error, "Exception = " + e.Message);
        }
      }
    }

    private Func<string, string> threadWorkload;
    private ConcurrentQueue<TcpClient> queue = new ConcurrentQueue<TcpClient>();
    private SemaphoreSlim maxConcurentThreads;

    private TcpListener listener;
    private ILogger logger;
  }
}