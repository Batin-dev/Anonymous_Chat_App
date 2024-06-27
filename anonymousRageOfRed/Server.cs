using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

public class Server
{
    private List<TcpClient> clients = new List<TcpClient>();

    public async Task Start()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Sunucu başlatıldı...");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            clients.Add(client);

            HandleClient(client);
        }
    }

    private async void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int byteCount;

            string clientId = await ReceiveClientId(stream);
            Console.WriteLine($"ID {clientId} bağlandı.");

            while ((byteCount = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string messageContent = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Console.WriteLine($"[{clientId}] {messageContent}");

                BroadcastMessage($"[{clientId}] {messageContent}");
            }

            clients.Remove(client);
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }

    private async Task<string> ReceiveClientId(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
        string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

        string[] parts = message.Split(':');
        if (parts.Length == 2 && parts[0] == "ID")
        {
            return parts[1];
        }
        else
        {
            throw new FormatException("Geçersiz ID formatı.");
        }
    }

    private void BroadcastMessage(string message)
    {
        foreach (TcpClient client in clients)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }
    }
}
