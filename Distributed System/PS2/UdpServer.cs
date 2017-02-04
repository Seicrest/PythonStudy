using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DistributeSystem
{
    public class UdpServer
    {
        private Socket serverSocket;

        private byte[] byteData = new byte[1024];
        private PS1 ps1;
        public UdpServer(string wordPath,string dictionaryPath)
        {
            //init the word dictionary using the PS1
            ps1 = new PS1(wordPath, dictionaryPath);
        }
        public void UdpStart()
        {
            try
            {
                //We are using UDP sockets
                serverSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);

                //Assign the any IP of the machine and listen on port number 1000
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1000);

                //Bind this address to the server
                serverSocket.Bind(ipEndPoint);

                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                //The epSender identifies the incoming clients
                EndPoint epSender = (EndPoint)ipeSender;

                //Start receiving data
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
                    SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
                Console.WriteLine("Start UDP listen on port 1000...");
            }
            catch (Exception ex)
            {
                PrintMessage(ex.Message, ConsoleColor.Red);
            }
        }
        public void UdpStop()
        {
            Console.WriteLine("Stop UDP");
            //stop the udp
            serverSocket.Close();
        }
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = (EndPoint)ipeSender;

                serverSocket.EndReceiveFrom(ar, ref epSender);

                //Transform the array of bytes received from the user into string
                string word = ReadWord(byteData);
                PrintMessage(string.Format("receive from {0} data '{1}'", epSender.ToString(), word),ConsoleColor.Green);
                byte[] message;
                string strMessage="";
                string mean;
                var exist = ps1.CheckWord(word,out mean);
                if(exist)
                {
                    PrintMessage(string.Format("Find '{0}' send the mean to client", word), ConsoleColor.Green);
                    strMessage=string.Format("Find '{0}'\r\nMean:\r\n{1}", word, mean);
                }
                else
                {
                    PrintMessage(string.Format("Can't Find '{0}'", word), ConsoleColor.Yellow);
                    strMessage = string.Format("Not Find '{0}'", word);
                }
                //send message by every 10k                
                do{
                    if (strMessage.Length > 10240)
                    {
                        message = ToByte(strMessage.Substring(0, 10240));
                        strMessage = strMessage.Substring(10240);
                    }
                    else
                    {
                        message = ToByte(strMessage);
                        strMessage = "";
                    }
                    //Send the mean to client
                    serverSocket.BeginSendTo(message, 0, message.Length, SocketFlags.None, epSender,
                            new AsyncCallback(OnSend), epSender);
                    
                } while (strMessage.Length > 0) ;
                
                //continue receiving data
                serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length,
                    SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
            }
            catch (Exception ex)
            {
                PrintMessage(ex.Message, ConsoleColor.Red);
            }
        }
        private string ReadWord(byte[] data)
        {
            int len = BitConverter.ToInt32(data, 0);
            if (len > 0)
            {
                return Encoding.UTF8.GetString(data, 4, len);
            }
            else
                return string.Empty;
        }
        public byte[] ToByte(string str)
        {
            List<byte> result = new List<byte>();
            if (str != null)
            {
                //First four are for the Command
                result.AddRange(BitConverter.GetBytes((int)str.Length));
                result.AddRange(Encoding.UTF8.GetBytes(str));
            }
            else
            {
                result.AddRange(BitConverter.GetBytes(0));
            }
            return result.ToArray();
        }
        public void OnSend(IAsyncResult ar)
        {
            try
            {
                serverSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                PrintMessage(ex.Message, ConsoleColor.Red);
            }
        }
        private void PrintMessage(string message,ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
    /*
    static void Main(string[] args)
        {
            UdpServer udpServer = new UdpServer(@"words.txt", @"dictionary.txt");
            udpServer.UdpStart();
            string str = "";
            do
            {
                Console.Write("Input stop for stop UDP server:");
                str = Console.ReadLine();
            }
            while (!"stop".Equals(str,StringComparison.OrdinalIgnoreCase));
            udpServer.UdpStop();
        }
     * */
}
