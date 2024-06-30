using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Server
{
    private List<TcpClient> clients = new List<TcpClient>();
    private const string LogFilePath = "Logs/messageLog.txt";

    public Server()
    {
        Directory.CreateDirectory("Logs");
    }

    public async Task Start()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Sunucu başlatıldı...");

        LoadMessagesFromLog();

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


            await SendLogToClient(stream);

            while ((byteCount = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string messageContent = Encoding.UTF8.GetString(buffer, 0, byteCount);
                string message = $"[{clientId}] {messageContent}";
                Console.WriteLine(message);

                LogMessage(message);

                BroadcastMessage(message);
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

    private void LogMessage(string message)
    {
        try
        {
            File.AppendAllText(LogFilePath, message + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Loglama hatası: {ex.Message}");
        }
    }

    private void LoadMessagesFromLog()
    {
        if (File.Exists(LogFilePath))
        {
            string[] messages = File.ReadAllLines(LogFilePath);
            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
        }
    }

    private async Task SendLogToClient(NetworkStream stream)
    {
        if (File.Exists(LogFilePath))
        {
            string[] messages = File.ReadAllLines(LogFilePath);
            foreach (var message in messages)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message + Environment.NewLine);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}
