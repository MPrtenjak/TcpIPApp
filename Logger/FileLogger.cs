// <copyright file="FileLogger.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System.IO;

namespace TcpIPApp.Logger
{
  internal class FileLogger : BaseLogger, ILogger
  {
    private StreamWriter logFile;

    public FileLogger(string filename)
    {
      logFile = new StreamWriter(filename, true);
    }

    protected override void write(string line)
    {
      if (this.compact)
        logFile.Write(line);
      else
        logFile.WriteLine(line);
    }
  }
}