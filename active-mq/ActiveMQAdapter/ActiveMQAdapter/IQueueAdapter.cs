using Apache.NMS;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveMQAdapter
{
    public interface IQueueAdapter
    {
        Task<IMessage> RecieveMessages(string recieveMessagesChannelName, CancellationToken cancellationToken);

        Task SendMessage(byte[] messageData, string destinationChannelName);
    }
}