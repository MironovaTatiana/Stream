using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Stream
{
    /// <summary>
    /// Сервер. Посылает файл Example1.txt клиенту
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            var localPort = 1111;
            var fileName = @"c:\temp\Example1.txt";
            var localAddress = IPAddress.Loopback;
            //Прослушиваем подключения от TCP-клиентов сети
            var server = new TcpListener(localAddress, localPort);
            server.Start();

            while (true)
            {
                //Ожидаем подключения
                var client = await server.AcceptTcpClientAsync();
                _ = Task.Run(() => Send(client, fileName));
            }
        }

        /// <summary>
        /// Посылаем файл в поток
        /// </summary>
        static async Task Send(TcpClient client, string fileName)
        {
            using var _ = client;
            var stream = client.GetStream();
            using var file = File.OpenRead(fileName);
            var length = file.Length;
            var lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
            //Вначале посылаем длину файла
            await stream.WriteAsync(lengthBytes);
            await file.CopyToAsync(stream);
        }
    }
}