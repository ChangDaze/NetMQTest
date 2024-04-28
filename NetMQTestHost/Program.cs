using NetMQ;
using NetMQ.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using System.Security.Policy;

namespace NetMQTestHost
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Request / Response
            //RequestSocket and ResponseSocket are synchronous, blocking, and throw exceptions if you try to read messages in the wrong order
            //Request / Response一定一來就要一回，所以會阻塞
            //https://netmq.readthedocs.io/en/latest/request-response/#request-response
            //using (var responseSocket = new ResponseSocket("tcp://*:5555"))//回覆的所以可以監聽所有
            //{
            //    using (var requestSocket = new RequestSocket("tcp://localhost:5555"))//主動送出的，所以對特定IP
            //    {
            //        Console.WriteLine("requestSocket : Sending 'Hello'");
            //        requestSocket.SendFrame("Hello");//用byte
            //        var message = responseSocket.ReceiveFrameString();//會幫忙解byte取字串
            //        //Request / Response開始接收沒收到不會停下，但沒回覆前也不能下指令繼續收
            //        Console.WriteLine($"responseSocket : Server Received '{message}'");
            //        Console.WriteLine("responseSocket : Sending 'World'");
            //        responseSocket.SendFrame("World");
            //        message = requestSocket.ReceiveFrameString();
            //        Console.WriteLine("requestSocket : Received '{0}'", message);
            //        Console.ReadLine();
            //    }
            //}
            #endregion

            #region Pub/Sub
            //Publish–subscribe is a messaging pattern where senders of messages, called publishers, do not program the messages to be sent directly to specific receivers, called subscribers.Instead, published messages are characterized into classes, without knowledge of what, if any, subscribers there may be. Similarly, subscribers express interest in one or more classes, and only receive messages that are of interest, without knowledge of what, if any, publishers there are.
            //Pub/Sub的發送者和訂閱者都可以不管對方要看哪個Topic，只看他們想做事的Topic
            //然後Topic的比對是大小敏感，前綴相同即可，所以聽一個Topic可能收到三四個不同Topic，空字串則是全聽
            //喔喔，這個模式Sub好像不會特別回傳給Pub只有一方在接收
            //用bat檔比較好測
            //https://netmq.readthedocs.io/en/latest/pub-sub/
            using (var pubSocket = new PublisherSocket())
            {
                Console.WriteLine("Publisher socket binding...");
                pubSocket.Options.SendHighWatermark = 1000; //可Queue住的buffer，0為no limit
                pubSocket.Bind("tcp://*:12345"); //類似server角度，且不會阻塞，所以可用 * 發?
                for(var i = 0; i < 100; i++)
                {
                    string msg = "";
                    //範例用random讓兩個topic 50% 50% 的收到訊息，我這邊就一起發
                    //給TopicA
                    msg = "TopicA msg-" + i.ToString();
                    Console.WriteLine($"Sending TopicA message : {msg}");
                    pubSocket.SendMoreFrame("TopicA").SendFrame(msg);//這個SendMoreFrame好像比較單純讓Topic變成整串訊息的前綴，後綴其他payload訊息，所以ZMQ好像也是比較單純的電文格式檢查ㄟ@@
                    //給TopicB
                    msg = "TopicB msg-" + i.ToString();
                    Console.WriteLine($"Sending TopicB message : {msg}");
                    pubSocket.SendMoreFrame("TopicB").SendFrame(msg);
                    Thread.Sleep(500);
                }
            }
            #endregion
        }
    }
}
