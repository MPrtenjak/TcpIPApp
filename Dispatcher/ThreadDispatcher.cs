// <copyright file="ThreadDispatcher.cs" company="MNet">
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
  internal class ThreadDispatcher : IDispatcher
  {
    public void Dispatch(TcpListener listener, ILogger logger, Func<string, string> workload)
    {
      while (true)
      {
        try
        {
          TcpClient tcpClient = listener.AcceptTcpClient();
          SyncServer server = new SyncServer(tcpClient, logger, workload);

          Thread thread = new Thread(new ThreadStart(server.Run));
          thread.Start();
        }
        catch (Exception ex)
        {
          logger.Log(LoggerThreshold.Error, ex.Message);
        }
      }
    }
  }
}