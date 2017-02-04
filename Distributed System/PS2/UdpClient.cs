using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpClient
{
    public class UdpClient
    {
        private Socket clientSocket;
        private EndPoint epServer;
        private byte[] receiveData;
        public UdpClient(string serverAddress)
        {
            receiveData = new byte[10240];
            try
            {
                //Using UDP sockets
                clientSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);
                
                //IP address of the server machine
                IPAddress ipAddress = IPAddress.Parse(serverAddress);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1000);

                epServer = (EndPoint)ipEndPoint;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } 
        }
        public void SearchWordByUdp(string word)
        {
            byte[] byteData = ToByte(word);

            clientSocket.BeginSendTo(byteData, 0, byteData.Length,SocketFlags.None, epServer, new AsyncCallback(OnSend), null);

            try
            {
                //Start listening to the data asynchronously
                clientSocket.BeginReceiveFrom(receiveData,
                                           0, receiveData.Length,
                                           SocketFlags.None,
                                           ref epServer,
                                           new AsyncCallback(OnReceive),
                                           null);
            }
            catch(Exception e)
            {
                PrintMessage(e.Message, ConsoleColor.Red);
            }
        }
        private void OnReceive(IAsyncResult ar)
        {
            IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint)ipeSender;

            clientSocket.EndReceiveFrom(ar, ref epSender);

            //Transform the array of bytes received from the user into string
            string word = ReadWord(receiveData);
            PrintMessage(string.Format("receive from {0}:\r\n{1}", epSender.ToString(), word), ConsoleColor.Green);
            //continue receiving data
            clientSocket.BeginReceiveFrom(receiveData, 0, receiveData.Length,
                SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
        }
        //Converts the Data structure into an array of bytes
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
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                PrintMessage("Local UDP Port is:" + ((IPEndPoint)(clientSocket.LocalEndPoint)).Port, ConsoleColor.Green);
                //clientSocket.Bind(new IPEndPoint(IPAddress.Any, ((IPEndPoint)(clientSocket.LocalEndPoint)).Port));
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void PrintMessage(string message, ConsoleColor c)
        {
            Console.ForegroundColor = c;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
    /*
     static void Main(string[] args)
        {
            UdpClient udpClient = new UdpClient("127.0.0.1");
            string word = "";
            do
            {
                Console.Write("Input a word(E for exit):");
                word = Console.ReadLine();
                if (word != "E")
                    udpClient.SearchWordByUdp(word);
            }
            while (word != "E");
        }
     */
}
