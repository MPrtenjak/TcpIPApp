// <copyright file="AsyncClientState.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Net.Sockets;
using System.Text;

namespace TcpIPApp.Client
{
  internal class AsyncClientState
  {
    public string Input { get; private set; }
    public byte[] WriteBuffer { get; private set; }
    public byte[] ReadBuffer { get; private set; }
    public long TotalBytesRead { get; private set; }
    public NetworkStream NetStream { get; private set; }
    public StringBuilder Response { get; private set; }

    public AsyncClientState(NetworkStream netStream, string input)
    {
      this.NetStream = netStream;

      this.Input = input;
      this.WriteBuffer = Encoding.UTF8.GetBytes(input);

      this.ReadBuffer = new byte[1024];
      this.TotalBytesRead = 0;
      this.Response = new StringBuilder();
    }

    public void AppendResponse(int bytesRead)
    {
      this.TotalBytesRead += bytesRead;
      this.Response.Append(Encoding.UTF8.GetString(this.ReadBuffer, 0, bytesRead));
    }
  }
}