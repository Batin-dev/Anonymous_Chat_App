using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;

public class Client
{
    private TcpClient client;
    private string clientId;

    public async Task Start()
    {
        client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 5000);

        NetworkStream stream = client.GetStream();
        Console.WriteLine("Sunucuya bağlandı...");

        // Rastgele 5 basamaklı ID oluştur
        clientId = GenerateClientId();

        // ID'yi server'e gönder
        SendMessage($"ID:{clientId}");

        // Mesajları dinlemeye başla
        Task.Run(() => ReceiveMessages(stream));

        while (true)
        {
            Console.Write(">");
            string messageContent = Console.ReadLine();

            if (messageContent.ToLower() == "exit")
                break;

            SendMessage(messageContent);
        }

        client.Close();
    }

    private void SendMessage(string message)
    {
        try
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }

    private async Task ReceiveMessages(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int byteCount;

        while ((byteCount = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
            Console.WriteLine(message);
        }
    }

    private string GenerateClientId()
    {
        Random random = new Random();
        return random.Next(10000, 100000).ToString();
    }
}
