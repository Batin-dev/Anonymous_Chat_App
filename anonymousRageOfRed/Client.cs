using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Client
{
    private TcpClient client;
    private string clientId;

    public async Task Start()
    {
        client = new TcpClient();
        await client.ConnectAsync("192.168.1.45", 5000);

        NetworkStream stream = client.GetStream();
        Console.WriteLine("Sunucuya bağlandı...");

        clientId = new Random().Next(10000, 99999).ToString();


        await SendClientId(stream);

        Task.Run(() => ReceiveMessages(stream, true));


        Task.Run(() => ReceiveMessages(stream, false));

        while (true)
        {
            Console.Write(">");
            string messageContent = Console.ReadLine();

            if (messageContent.ToLower() == "exit")
                break;

            byte[] buffer = Encoding.UTF8.GetBytes(messageContent);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }

        client.Close();
    }

    private async Task SendClientId(NetworkStream stream)
    {
        string message = $"ID:{clientId}";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(buffer, 0, buffer.Length);
    }

    private async Task ReceiveMessages(NetworkStream stream, bool isLog)
    {
        byte[] buffer = new byte[1024];
        int byteCount;

        while ((byteCount = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            if (isLog)
            {
                Console.WriteLine("You: " + message);
            }
            else
            {
                Console.WriteLine(">" + message);
            }
        }
    }
}
