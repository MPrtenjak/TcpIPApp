using System;
using System.Diagnostics;
using System.Threading;

namespace TcpIPApp.WorkLoad
{
  internal class BaseWorkload
  {
    public int MinWorkingTimeInMS { get; set; }
    public int MaxWorkingTimeInMS { get; set; }
    public bool DoRealWork { get; set; }

    public BaseWorkload(int minWorkingTimeInMS, int maxWorkingTimeInMS, bool doRealWork)
    {
      this.MinWorkingTimeInMS = Math.Max(0, minWorkingTimeInMS);
      this.MaxWorkingTimeInMS = Math.Max(0, maxWorkingTimeInMS);
      this.DoRealWork = doRealWork;

      if (this.MaxWorkingTimeInMS < this.MinWorkingTimeInMS)
      {
        int tmp = this.MaxWorkingTimeInMS;
        this.MaxWorkingTimeInMS = this.MinWorkingTimeInMS;
        this.MinWorkingTimeInMS = tmp;
      }
    }

    public void Wait()
    {
      if (this.MaxWorkingTimeInMS == 0)
        return;

      int wait = (this.MinWorkingTimeInMS == this.MaxWorkingTimeInMS) ? this.MinWorkingTimeInMS : this.rnd.Next(this.MinWorkingTimeInMS, this.MaxWorkingTimeInMS);

      if (this.DoRealWork)
        this.realWork(wait);
      else
        this.sleep(wait);
    }

    public void sleep(int wait)
    {
      Thread.Sleep(wait);
    }

    public void realWork(int wait)
    {
      var watch = Stopwatch.StartNew();
      long sumMS = 0;
      while (sumMS < wait)
      {
        int j = 0;
        for (int i = 0; i < 1000000; i++)
          j += 3;

        watch.Stop();
        sumMS += Math.Max(1, watch.ElapsedMilliseconds);
        watch.Restart();
      }
    }

    private Random rnd = new Random();
  }
}