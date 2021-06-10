using RabbitMQ.Core.Commands;

namespace RabbitMQ.Core.RPC
{
    public interface IRPCClient
    {
        string Call<T>(T request) where T : Command;
        void Close();
    }
}
