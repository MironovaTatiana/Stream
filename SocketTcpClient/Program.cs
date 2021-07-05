using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketTcpClient
{
    /// <summary>
    /// Клиент. Получает файл Example1.txt от клиента и сохраняет его в Example2.txt на диск
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            var serverAddress = IPAddress.Loopback;
            var serverPort = 1111;
            var fileName = @"c:\temp\Example2.txt";

            using var client = new TcpClient(serverAddress.ToString(), serverPort);
            //Получаем данные с сервера
            var stream = client.GetStream();
            var buf = new byte[65536];

            await ReadBytesByPotrions(sizeof(long), stream, buf);

            //Преобразует число из сетевого байтового формата в байтовый формат узла
            var remainingLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0));

            using var file = File.Create(fileName);

            while (remainingLength > 0)
            {
                var lengthToRead = (int)Math.Min(remainingLength, buf.Length);

                await ReadBytesByPotrions(lengthToRead, stream, buf);
                await file.WriteAsync(buf, 0, lengthToRead);
                remainingLength -= lengthToRead;
            }
        }

        /// <summary>
        /// Чтение из потока
        /// </summary>
        static async Task ReadBytesByPotrions(int portion, NetworkStream stream, byte[] buf)
        {
            var readPos = 0;

            while (readPos < portion)
            {
                //Асинхронно считываем последовательность байтов из текущего потока и перемещаем позицию внутри потока на число считанных байтов
                var currentRead = await stream.ReadAsync(buf, readPos, portion - readPos);

                if (currentRead == 0)
                {
                    throw new EndOfStreamException();
                }

                readPos += currentRead;
            }
        }
    }
}