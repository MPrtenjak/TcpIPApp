// <copyright file="IClient.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.Net;
using TcpIPApp.Logger;

namespace TcpIPApp.Client
{
  internal interface IClient
  {
    string Process(IPEndPoint localEndPoint, ILogger logger, string text);
  }
}