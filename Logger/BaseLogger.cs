// <copyright file="BaseLogger.cs" company="MNet">
//     Copyright (c) Matjaz Prtenjak All rights reserved.
// </copyright>
// <author>Matjaz Prtenjak</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;

namespace TcpIPApp.Logger
{
  internal abstract class BaseLogger
  {
    public LoggerThreshold LogThreshold { get; set; }

    public void Cache(LoggerThreshold threshold, string line)
    {
      if (this.logCache == null)
        this.logCache = new List<string>();

      if (this.thresholdReached(threshold))
      {
        mutex.WaitOne();
        this.logCache.Add(line);
        mutex.ReleaseMutex();
      }
    }

    public void Log(LoggerThreshold threshold, string line)
    {
      this.doLogging(threshold, () =>
      {
        this.write(line);
      });
    }

    public void Flush()
    {
      if (this.logCache == null) return;
      if (this.logCache.Count < 1) return;

      this.doLogging(this.LogThreshold, () =>
      {
        foreach (var line in this.logCache)
          this.write(line);

        this.logCache.Clear();
      });
    }

    protected abstract void write(string line);

    // use method Write (compact logging) or WriteLine (non compact logging)
    protected bool compact
    {
      get
      {
        return (int)this.LogThreshold <= (int)LoggerThreshold.Minimal;
      }
    }

    // cache for logging
    protected List<string> logCache;

    // test if info needs to get logged
    protected bool thresholdReached(LoggerThreshold threshold)
    {
      return ((int)threshold <= (int)this.LogThreshold);
    }

    // working method to execute logging in multi threading app
    protected void doLogging(LoggerThreshold threshold, Action loggingAction)
    {
      if (!thresholdReached(threshold)) return;

      mutex.WaitOne();
      loggingAction();
      mutex.ReleaseMutex();
    }

    private static Mutex mutex = new Mutex();
  }
}