using System;

namespace TcpIPApp
{
  internal class Program
  {
    private static int Main(string[] args)
    {
      Options options = parseOptions(args);

      try
      {
        switch (options.Mode)
        {
          case Mode.Server:
            Server.Runner serverRunner = new Server.Runner();
            return serverRunner.Run(options);

          case Mode.Client:
            Client.Runner clientRunner = new Client.Runner();
            return clientRunner.Run(options);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }

      return 1;
    }

    private static Options parseOptions(string[] args)
    {
      string invokedVerb = null;
      object invokedVerbInstance = null;

      var options = new Options();
      if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, options,
        (verb, subOptions) =>
        {
          invokedVerb = verb;
          invokedVerbInstance = subOptions;
        }))
      {
        Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
      }

      if (string.Compare(invokedVerb, "server", true) == 0)
      {
        options.Mode = Mode.Server;
        options.ServerVerb = invokedVerbInstance as ServerSubOptions;
      }
      else
      {
        options.Mode = Mode.Client;
        options.ClientVerb = invokedVerbInstance as ClientSubOptions;
      }

      return options;
    }
  }
}