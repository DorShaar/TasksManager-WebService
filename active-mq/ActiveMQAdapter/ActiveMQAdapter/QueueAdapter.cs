using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Logger.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveMQAdapter
{
    public class QueueAdapter : IQueueAdapter, IDisposable
    {
        private readonly ILogger mLogger;
        private bool mDisposed = false;
        private IConnection mConnection;
        public string URI { get; } = "activemq:tcp://localhost:61616";
        //public string URI { get; } = "tcp://activemq:61616";

        public QueueAdapter(ILogger logger)
        {
            mLogger = logger;

            SetupConnection();
        }

        private void SetupConnection()
        {
            mConnection = new ConnectionFactory(URI).CreateConnection();
        }

        public Task<IMessage> RecieveMessages(string recieveMessagesChannelName, CancellationToken cancellationToken = default)
        {
            using ISession session = mConnection.CreateSession();
            using IDestination dest = session.GetTopic(recieveMessagesChannelName);
            using IMessageConsumer consumer = session.CreateConsumer(dest);
            Console.WriteLine($"Start listening to {recieveMessagesChannelName}");

            IMessage message = consumer.Receive();

            if (message != null)
            {
                if (message is IBytesMessage byteMessage)
                    return Task.FromResult(message);

                mLogger.Log($"Could not parse message, type of message is: {message.NMSType}");
            }
            else
            {
                mLogger.Log($"Null message recieved");
            }

            return Task.FromResult(message);
        }

        public Task SendMessage(byte[] messageData, string destinationChannelName)
        {
            using ISession session = mConnection.CreateSession();
            using IDestination destination = session.GetTopic(destinationChannelName);
            using IMessageProducer producer = session.CreateProducer(destination);
            producer.Send(producer.CreateBytesMessage(messageData));

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
                return;

            mDisposed = true;

            if (disposing)
            {
                SafeDispose(mConnection);
            }
        }

        private void SafeDispose(IDisposable disposable)
        {
            try
            {
                disposable?.Dispose();
            }
            catch (Exception e)
            {
                mLogger.LogError("NMS Dispose error", e);
            }
        }

        ~QueueAdapter()
        {
            Dispose(false);
        }
    }
}