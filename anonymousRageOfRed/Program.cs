using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("1. Sunucu Başlat");
        Console.WriteLine("2. İstemci Başlat");
        Console.Write("Seçiminiz: ");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Server server = new Server();
            server.Start().Wait();
        }
        else if (choice == "2")
        {
            Client client = new Client();
            client.Start().Wait();
        }
    }
}
