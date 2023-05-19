using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.ServiceProcess;


public class Program : ServiceBase
{
    private TcpListener listener;
    private int port = 8888; // Porta para escutar as conexões

    public Program()
    {
        this.ServiceName = "MyService";
        this.CanStop = true;
    }

    protected override void OnStart(string[] args)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Aguardando conexões...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Cliente conectado. Verificando conexão RDP...");

            string output = RunNetstatCommand();
            bool isConnected = output.Contains(":3389");

            string response = isConnected ? "Existe uma conexão RDP ativa" : "Não existe uma conexão RDP ativa";

            // Envia a resposta para o cliente
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(response);
            NetworkStream stream = client.GetStream();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            // Fecha a conexão com o cliente
            client.Close();
        }
    }

    protected override void OnStop()
    {
        listener.Stop();
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

    public static void Main(string[] args)
    {
        ServiceBase.Run(new Program());
    }
}
