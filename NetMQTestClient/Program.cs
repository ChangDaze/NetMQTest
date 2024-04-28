using NetMQ;
using NetMQ.Sockets;

namespace NetMQTestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Pub/Sub client
            //https://netmq.readthedocs.io/en/latest/pub-sub/
            HashSet<string> allowableCommandLineArgs = new HashSet<string>() { "TopicA", "TopicB", "All" };
            if (args.Length != 1 || !allowableCommandLineArgs.Contains(args[0]))
            {
                Console.WriteLine("Expected one argument, either " + "'TopicA', 'TopicB' or 'All'");
                Console.ReadLine();
                Environment.Exit(-1);
            }
            string topic = args[0] == "All" ? "" : args[0];
            Console.WriteLine($"Subscriber started for Topic : {topic}");
            using(var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://localhost:12345"); //只聽特定IP
                subSocket.Subscribe(topic);
                Console.WriteLine("Subscriber socket connecting...");
                DateTime startTime = DateTime.Now;
                while((DateTime.Now - startTime).TotalMinutes < 5) 
                {
                    string messageTopicReceiced = subSocket.ReceiveFrameString();
                    string messageReceiced = subSocket.ReceiveFrameString();
                    Console.WriteLine($"{messageTopicReceiced} : {messageReceiced}");
                }
            }
            #endregion

            Console.ReadLine();
        }
    }
}
