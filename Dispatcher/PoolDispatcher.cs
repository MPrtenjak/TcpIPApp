// <copyright file="PoolDispatcher.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Net.Sockets;
using System.Threading;
using TcpIPApp.Logger;
using TcpIPApp.Server;

namespace TcpIPApp.Dispatcher
{
  internal class PoolDispatcher : IDispatcher
  {
    public void Dispatch(TcpListener listener, ILogger logger, Func<string, string> workload)
    {
      for (int i = 0; i < numThreads; i++)
      {
        ThreadInPool poolThread = new ThreadInPool(listener, logger, workload);
        Thread thread = new Thread(new ThreadStart(poolThread.Run));
        thread.Start();
      }
    }

    public PoolDispatcher(int numThreads)
    {
      this.numThreads = numThreads;
    }

    private int numThreads;
  }

  internal class ThreadInPool
  {
    public ThreadInPool(TcpListener listener, ILogger logger, Func<string, string> workload)
    {
      this.listener = listener;
      this.logger = logger;
      this.workload = workload;
    }

    public void Run()
    {
      while (true)
      {
        try
        {
          TcpClient tcpClient = this.listener.AcceptTcpClient();
          SyncServer server = new SyncServer(tcpClient, this.logger, this.workload);

          Thread thread = new Thread(new ThreadStart(server.Run));
          thread.Start();
        }
        catch (Exception ex)
        {
          logger.Log(LoggerThreshold.Error, ex.Message);
        }
      }
    }

    private TcpListener listener;
    private ILogger logger;
    private Func<string, string> workload;
  }
}