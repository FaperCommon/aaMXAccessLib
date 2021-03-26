using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Intma.Libraries;


namespace MXAccessMXServiceExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = new TcpChannel(12345);
            ChannelServices.RegisterChannel(channel, false);

            var service = new aaMXManager("TestServer");
            service.Register();
            RemotingServices.Marshal(service, "MXAccessService", service.GetType());
            Console.WriteLine("Example service start");
            Console.ReadLine();
            service.Unregister();
        }
    }
}
