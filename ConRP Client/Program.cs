using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("\n[!] Digite o IP da máquina que deseja verificar a conexão RDP: ");
        string ip = Console.ReadLine();

        int port = 8888; // Porta do servidor

        try
        {
            TcpClient client = new TcpClient(ip, port);

            byte[] buffer = new byte[1024];
            StringBuilder response = new StringBuilder();

            NetworkStream stream = client.GetStream();
            int bytesRead;
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                response.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            } while (stream.DataAvailable);

            Console.WriteLine("\n[!] Resposta do servidor: " + response.ToString());

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n[!] Erro ao conectar ao servidor: " + ex.Message);
        }
    }
}