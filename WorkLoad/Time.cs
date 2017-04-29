using System;

namespace TcpIPApp.WorkLoad
{
  internal class Time : BaseWorkload, IWorkload
  {
    public Time(int minWorkingTimeInMS, int maxWorkingTimeInMS, bool doRealWork) :
      base(minWorkingTimeInMS, maxWorkingTimeInMS, doRealWork)
    { }

    public string Workload(string clientText)
    {
      this.Wait();
      return DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString(); ;
    }
  }
}