// <copyright file="NewASyncClient.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpIPApp.Logger;

namespace TcpIPApp.Client
{
  internal class NewASyncClient : IClient
  {
    public string Process(IPEndPoint localEndPoint, ILogger logger, string text)
    {
      Task<string> value = this.Execute(localEndPoint, logger, text);
      value.Wait();

      return value.Result;
    }

    public async Task<string> Execute(IPEndPoint localEndPoint, ILogger logger, string text)
    {
      this.logger = logger;

      TcpClient client = new TcpClient();
      try
      {
        await client.ConnectAsync(localEndPoint.Address, localEndPoint.Port);
        this.logger.Cache(LoggerThreshold.Debug, "Client connected to server");

        using (var networkStream = client.GetStream())
        using (var writer = new StreamWriter(networkStream))
        using (var reader = new StreamReader(networkStream))
        {
          writer.AutoFlush = true;
          await writer.WriteAsync(text);
          this.logger.Cache(LoggerThreshold.Debug, $"Client wrote [{text}]");

          var bytesRead = 0;
          var buffer = new char[1024];
          var result = new StringBuilder();
          while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
          {
            result.Append(buffer, 0, bytesRead);
          }

          this.logger.Cache(LoggerThreshold.Debug, $"Client has read [{result.ToString()}]");
          return result.ToString();
        }
      }
      catch (Exception ex)
      {
        this.logger.Cache(LoggerThreshold.Error, ex.ToString());
      }
      finally
      {
        client.Close();
        this.logger.Cache(LoggerThreshold.Debug, "Client disconected from server");
        this.logger.Flush();
      }

      return string.Empty;
    }

    private ILogger logger;
  }
}