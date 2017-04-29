// <copyright file="ClasicASyncClient.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TcpIPApp.Logger;

namespace TcpIPApp.Client
{
  internal class ClasicASyncClient : IClient
  {
    public string Process(IPEndPoint localEndPoint, ILogger logger, string text)
    {
      this.logger = logger;

      TcpClient client = new TcpClient();
      AsyncClientState cs = null;

      try
      {
        client.Connect(localEndPoint);
        logger.Cache(LoggerThreshold.Debug, "Client connected to server");

        cs = new AsyncClientState(client.GetStream(), text);
        IAsyncResult writeResult = cs.NetStream.BeginWrite(cs.WriteBuffer, 0, cs.WriteBuffer.Length, new AsyncCallback(this.writeCallback), cs);

        writeResult.AsyncWaitHandle.WaitOne();
        this.logger.Cache(LoggerThreshold.Debug, $"Client wrote [{cs.Input}]");

        IAsyncResult readResult = cs.NetStream.BeginRead(cs.ReadBuffer, 0, cs.ReadBuffer.Length, new AsyncCallback(this.readCallback), cs);
        this.ReadDone.WaitOne();
      }
      catch (Exception ex)
      {
        logger.Cache(LoggerThreshold.Error, ex.ToString());
      }

      if ((cs != null) && (cs.NetStream != null)) cs.NetStream.Close();
      if (client.Connected) client.Close();
      logger.Cache(LoggerThreshold.Debug, "Client disconected from server");
      logger.Flush();

      return (cs != null) ? cs.Response.ToString() : string.Empty;
    }

    private void writeCallback(IAsyncResult asyncResult)
    {
      try
      {
        AsyncClientState cs = (AsyncClientState)asyncResult.AsyncState;
        cs.NetStream.EndWrite(asyncResult);
      }
      catch (Exception ex)
      {
        this.logger.Log(LoggerThreshold.Error, ex.ToString());
      }
    }

    private void readCallback(IAsyncResult asyncResult)
    {
      try
      {
        AsyncClientState cs = (AsyncClientState)asyncResult.AsyncState;

        int bytesRead = cs.NetStream.EndRead(asyncResult);
        cs.AppendResponse(bytesRead);

        if (cs.NetStream.DataAvailable)
          cs.NetStream.BeginRead(cs.ReadBuffer, 0, cs.ReadBuffer.Length, new AsyncCallback(this.readCallback), cs);
        else
        {
          this.logger.Cache(LoggerThreshold.Debug, $"Client has read [{cs.Response}]");
          this.ReadDone.Set();
        }
      }
      catch (Exception ex)
      {
        logger.Log(LoggerThreshold.Error, ex.ToString());
      }
    }

    private ILogger logger;
    private ManualResetEvent ReadDone = new ManualResetEvent(false);
  }
}