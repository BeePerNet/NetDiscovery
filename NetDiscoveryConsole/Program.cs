using NetDiscovery.Lib;
using System;

namespace NetDiscoveryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            //DiscoveryClient client = new DiscoveryClient();

            try
            {
                DiscoveryZone ip = DummyDiscoveryClient.Zone;
                //client.Start();
                Console.WriteLine("Started. Press an key to stop");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                //client.Stop();
                Console.WriteLine(ex.Message);
                Console.WriteLine("Exception. Press an key to quit");
                Console.ReadKey();
            }
        }
    }
}
