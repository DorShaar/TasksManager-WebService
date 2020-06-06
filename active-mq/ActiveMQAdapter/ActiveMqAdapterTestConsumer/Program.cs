using System;

namespace ActiveMqAdapterTestConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            ActiveMQAdapterTester activeMQAdapterTester = new ActiveMQAdapterTester();
            activeMQAdapterTester.Test().Wait();
        }
    }
}