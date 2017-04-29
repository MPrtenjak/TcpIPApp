// <copyright file="IDispatcher.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Net.Sockets;
using TcpIPApp.Logger;

namespace TcpIPApp.Dispatcher
{
  internal interface IDispatcher
  {
    void Dispatch(TcpListener listener, ILogger logger, Func<string, string> workload);
  }
}