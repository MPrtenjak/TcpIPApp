// <copyright file="SyncServer.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TcpIPApp.Logger;

namespace TcpIPApp.Server
{
  internal class SyncServer
  {
    public SyncServer(TcpClient tcpClient, ILogger logger, Func<string, string> workload)
    {
      this.tcpClient = tcpClient;
      this.logger = logger;
      this.workload = workload;
    }

    public void Run()
    {
      try
      {
        using (var networkStream = this.tcpClient.GetStream())
        {
          var buffer = new byte[256];
          var sb = new StringBuilder();
          do
          {
            var bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
          }
          while (networkStream.DataAvailable);

          string input = sb.ToString();
          this.logger.Cache(LoggerThreshold.Debug, $"Server received [{input}]");

          string output = this.workload(input);

          byte[] sendBuffer = Encoding.UTF8.GetBytes(output);
          networkStream.Write(sendBuffer, 0, sendBuffer.Length);

          this.logger.Cache(LoggerThreshold.Debug, $"Server send [{output}]");

          if ((int)this.logger.LogThreshold <= (int)LoggerThreshold.Minimal)
            this.logger.Cache(LoggerThreshold.Minimal, ".");
        }
      }
      catch (SocketException se)
      {
        this.logger.Cache(LoggerThreshold.Error, se.ErrorCode + ": " + se.Message);
      }
      catch (IOException io)
      {
        this.logger.Cache(LoggerThreshold.Error, io.Data + ": " + io.Message);
      }

      this.tcpClient.Close();
      this.logger.Flush();
    }

    private TcpClient tcpClient;
    private ILogger logger;
    private Func<string, string> workload;
  }
}