using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        int port = 8888; // Porta para escutar as conexões

        DateTime horarioLogin = DateTime.Now; // Horario que o usuário realizou login
        Console.Write("Digite seu nome: ");
        string nome = Console.ReadLine();
        Console.WriteLine($"\n[!] {nome}, você está logado com sucesso!");

        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("\n[!] Aguardando conexões...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("\n[!] Cliente conectado com sucesso!\n[!] Verificando se há uma conexão RDP...");

            bool isConnected = CheckRdpConnection();

            string response = isConnected ?
                $"\n[!] Existe uma conexão RDP ativa com o usuário: {nome}\n[!] Conectado às: {horarioLogin:dd/MM/yyyy HH:mm:ss}"
                : "\n[!] Não existe uma conexão RDP ativa";

            // Envia a resposta para o cliente
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
            NetworkStream stream = client.GetStream();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            if (!isConnected)
            {
                Console.Clear();
                Console.Write("Digite seu nome: ");
                nome = Console.ReadLine();
                Console.WriteLine($"\n[!] {nome}, você está logado com sucesso!");
            }

            // Fecha a conexão com o cliente
            client.Close();
        }
    }

    static bool CheckRdpConnection()
    {
        string output = RunNetstatCommand();
        return output.Contains(":3389");
    }

    static string RunNetstatCommand()
    {
        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c netstat -n";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        process.WaitForExit();

        string output = process.StandardOutput.ReadToEnd();
        return output;
    }
}
