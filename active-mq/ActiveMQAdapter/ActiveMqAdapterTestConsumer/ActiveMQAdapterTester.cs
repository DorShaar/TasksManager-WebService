using ActiveMQAdapter;
using Apache.NMS;
using Logger;
using Logger.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveMqAdapterTestConsumer
{
    public class ActiveMQAdapterTester
    {
        private readonly ILogger mLogger = new ConsoleLogger();

        public async Task Test()
        {
            using QueueAdapter queueAdapter = new QueueAdapter(mLogger);

            string queueName = "test-queue";

            string message1 = "message1";
            string message2 = "message2";
            string message3 = "message3";

            byte[] message1Bytes = Encoding.ASCII.GetBytes(message1);
            byte[] message2Bytes = Encoding.ASCII.GetBytes(message2);
            byte[] message3Bytes = Encoding.ASCII.GetBytes(message3);

            await queueAdapter.SendMessage(message1Bytes, queueName);
            await queueAdapter.SendMessage(message2Bytes, queueName);
            IMessage recievedMessage1 = await queueAdapter.RecieveMessages(queueName, new CancellationToken());
            PrintMessageContent(recievedMessage1);
            IMessage recievedMessage2 = await queueAdapter.RecieveMessages(queueName, new CancellationToken());
            PrintMessageContent(recievedMessage2);

            await queueAdapter.SendMessage(message3Bytes, queueName);
            IMessage recievedMessage3 = await queueAdapter.RecieveMessages(queueName, new CancellationToken());
            PrintMessageContent(recievedMessage3);
        }

        private void PrintMessageContent(IMessage message)
        {
            IBytesMessage bytesMessage = message as IBytesMessage;
            mLogger.Log(bytesMessage.ReadString());
        }
    }
}