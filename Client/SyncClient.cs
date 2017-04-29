// <copyright file="SyncClient.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpIPApp.Logger;

namespace TcpIPApp.Client
{
  internal class SyncClient : IClient
  {
    public string Process(IPEndPoint localEndPoint, ILogger logger, string text)
    {
      TcpClient client = new TcpClient();
      NetworkStream netStream = null;
      var returnValue = string.Empty;

      try
      {
        client.Connect(localEndPoint);
        logger.Cache(LoggerThreshold.Debug, "Client connected to server");

        netStream = client.GetStream();
        var writeBuffer = Encoding.UTF8.GetBytes(text);
        netStream.Write(writeBuffer, 0, writeBuffer.Length);

        logger.Cache(LoggerThreshold.Debug, $"Client wrote [{text}]");

        var result = new StringBuilder();
        var readBuffer = new byte[256];
        var totalBytes = 0;
        do
        {
          var bytesRead = netStream.Read(readBuffer, 0, readBuffer.Length);
          result.Append(Encoding.UTF8.GetString(readBuffer, 0, bytesRead));
          totalBytes += bytesRead;
        }
        while (client.Available > 0);

        logger.Cache(LoggerThreshold.Debug, $"Client has read [{result.ToString()}]");
        returnValue = result.ToString();
      }
      catch (Exception ex)
      {
        logger.Cache(LoggerThreshold.Error, ex.ToString());
      }

      if (netStream != null) netStream.Close();
      if (client.Connected) client.Close();
      logger.Cache(LoggerThreshold.Debug, "Client disconected from server");

      logger.Flush();

      return returnValue;
    }
  }
}