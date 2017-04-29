namespace TcpIPApp.WorkLoad
{
  internal class Echo : BaseWorkload, IWorkload
  {
    public Echo(int minWorkingTimeInMS, int maxWorkingTimeInMS, bool doRealWork) :
      base(minWorkingTimeInMS, maxWorkingTimeInMS, doRealWork)
    { }

    public string Workload(string clientText)
    {
      this.Wait();
      return clientText;
    }
  }
}