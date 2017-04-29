// <copyright file="JSLikeASyncClient.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TcpIPApp.Logger;

namespace TcpIPApp.Client
{
  internal class JSLikeASyncClient : IClient
  {
    public string Process(IPEndPoint localEndPoint, ILogger logger, string text)
    {
      string result = string.Empty;
      ManualResetEvent allDone = new ManualResetEvent(false);

      this.Execute(localEndPoint, logger, text, (Exception ex, string value) =>
      {
        result = value;
        allDone.Set();
      });

      allDone.WaitOne();
      return result;
    }

    public void Execute(IPEndPoint localEndPoint, ILogger logger, string text, Action<Exception, string> callback)
    {
      this.logger = logger;
      this.callback = callback;

      this.client = new TcpClient();
      this.state = null;

      try
      {
        this.client.Connect(localEndPoint);
        this.logger.Cache(LoggerThreshold.Debug, "Client connected to server");

        this.state = new AsyncClientState(this.client.GetStream(), text);
        IAsyncResult writeResult = this.state.NetStream.BeginWrite(this.state.WriteBuffer, 0, this.state.WriteBuffer.Length, new AsyncCallback(this.writeCallback), this.state);
      }
      catch (Exception ex)
      {
        this.logger.Cache(LoggerThreshold.Error, ex.ToString());
        this.executeCallback(ex, string.Empty);
      }
    }

    private void executeCallback(Exception ex, string value)
    {
      if ((this.state != null) && (this.state.NetStream != null)) this.state.NetStream.Close();
      if (this.client.Connected) this.client.Close();
      this.logger.Cache(LoggerThreshold.Debug, "Client disconected from server");
      this.logger.Flush();
      this.callback(ex, value);
    }

    private void writeCallback(IAsyncResult asyncResult)
    {
      try
      {
        this.state.NetStream.EndWrite(asyncResult);

        this.logger.Cache(LoggerThreshold.Debug, $"Client wrote [{this.state.Input}]");
        IAsyncResult readResult = this.state.NetStream.BeginRead(this.state.ReadBuffer, 0, this.state.ReadBuffer.Length, new AsyncCallback(this.readCallback), this.state);
      }
      catch (Exception ex)
      {
        this.logger.Log(LoggerThreshold.Error, ex.ToString());
        this.executeCallback(ex, string.Empty);
      }
    }

    private void readCallback(IAsyncResult asyncResult)
    {
      try
      {
        int bytesRead = this.state.NetStream.EndRead(asyncResult);
        this.state.AppendResponse(bytesRead);

        if (this.state.NetStream.DataAvailable)
          this.state.NetStream.BeginRead(this.state.ReadBuffer, 0, this.state.ReadBuffer.Length, new AsyncCallback(this.readCallback), this.state);
        else
        {
          this.logger.Cache(LoggerThreshold.Debug, $"Client has read [{this.state.Response.ToString()}]");
          this.executeCallback(null, this.state.Response.ToString());
        }
      }
      catch (Exception ex)
      {
        this.executeCallback(ex, string.Empty);
      }
    }

    private ILogger logger;
    private Action<Exception, string> callback;
    private TcpClient client = new TcpClient();
    private AsyncClientState state = null;
  }
}