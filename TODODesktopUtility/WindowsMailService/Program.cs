namespace WindowsMailService
{
    using System;
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            // Configure and run a service host using the HostFactory
            var exitCode = HostFactory.Run(x => {
                x.Service<WindowsEmailService>(s =>
                {
                    s.ConstructUsing(heartbeat => new WindowsEmailService());
                    s.WhenStarted(heartbeat => heartbeat.Start());
                    s.WhenStopped(heartbeat => heartbeat.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("WindowsMailService");
                x.SetDisplayName("Windows Mail Service");
            });
            
            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
