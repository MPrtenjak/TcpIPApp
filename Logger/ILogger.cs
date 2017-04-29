// <copyright file="ILogger.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

namespace TcpIPApp.Logger
{
  internal interface ILogger
  {
    LoggerThreshold LogThreshold { get; set; }

    // log immediately
    void Log(LoggerThreshold threshold, string line);

    // cache log data
    void Cache(LoggerThreshold threshold, string line);

    // flush all cached data and clear cache
    void Flush();
  }
}