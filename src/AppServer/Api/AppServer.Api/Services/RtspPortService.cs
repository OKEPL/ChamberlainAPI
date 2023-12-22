using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Chamberlain.AppServer.Api.Contracts.DataTransfer;
using Chamberlain.AppServer.Api.Contracts.Services;
using Serilog;

namespace Chamberlain.AppServer.Api.Services
{
    public class RtspPortService : IRtspPortService
    {
        public CheckResultModel CheckRtspPort(string ip, int port)
        {
            TcpClient tcpClient = null;
            var bytesRead = 0;
            try
            {
                Log.Debug($"Testing RTSP on {ip}:{port}...");
                tcpClient = new TcpClient();
                tcpClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                var stream = tcpClient.GetStream();
                var testMessage =
                    Encoding.ASCII.GetBytes(
                        "OPTIONS rtsp://example.com/media.mp4 RTSP/1.0\r\nCSeq: 1\r\nRequire: implicit-play\r\n\r\n");
                var buff = new byte[100000];
                var readTask = stream.ReadAsync(buff, 0, buff.Length);
                stream.Write(testMessage, 0, testMessage.Length);
                bytesRead = readTask.Result;
            }
            catch (Exception e)
            {
                // nothing to do
                Log.Error("Exception while testing RTSP port: " + e.Message);
            }
            finally
            {
                tcpClient?.Close();
            }

            return new CheckResultModel {Exists = bytesRead > 0};
        }

        public CheckResultModel CheckHostPort(string ip, int port)
        {
            var ret = true;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Connect(ip, port);
                }
                catch (SocketException)
                {
                    ret = false;
                }
            }
            return new CheckResultModel {Exists = ret};
        }
    }
}