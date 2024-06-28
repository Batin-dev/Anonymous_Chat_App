using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string password = "ruhi123";
        Console.WriteLine("Admin-1");
        Console.WriteLine("Join chat-2");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.WriteLine("Password : ");
            string newpass;
            newpass = Console.ReadLine();
            if (newpass == password)
            {
                Server server = new Server();
                await server.Start();

            }
            else
            {
                Console.WriteLine("Incorret");
            }
        }
        else if (choice == "2")
        {
            Client client = new Client();
            await client.Start();
        }
    }
}
