using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections;


namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1024);
            ArrayList list = new ArrayList();
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Dictionary<string, EndPoint> users = new Dictionary<string, EndPoint>();

            server.Bind(localEP);
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            //dummy end-point


            int recv;
            byte[] data;

            while (true)
            {


                data = new byte[1024];
                recv = 0;
                recv = server.ReceiveFrom(data, ref remoteEP);
              //  Console.WriteLine("Waiting for a client...");

                String message = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine(message);
                
                switch (message)
                {
                    case "join":
                        Console.WriteLine("Received from {0}: ", remoteEP.ToString());
                        list.Add(remoteEP);
                        recv = server.ReceiveFrom(data, ref remoteEP);
                        String name1 = Encoding.ASCII.GetString(data, 0, recv);
                        Console.WriteLine(name1);
                        if (name1=="SERVER" || name1 == "")
                            server.SendTo(Encoding.ASCII.GetBytes("SERVER: Improper name. Set proper one and reconnect"), 51, SocketFlags.None, (EndPoint)remoteEP);
                        else
                        {
                            if (users.ContainsKey(name1))
                                server.SendTo(Encoding.ASCII.GetBytes("SERVER: name already used. Set proper one and reconnect"), 55, SocketFlags.None, (EndPoint)remoteEP);
                            else
                            {
                                server.SendTo(Encoding.ASCII.GetBytes("SERVER: Connected"), 17, SocketFlags.None, (EndPoint)remoteEP);
                                users.Add(name1, remoteEP);
                            }
                            
                        }
                        break;

                    case "message":

                        int recv2 = server.ReceiveFrom(data, ref remoteEP);
                        String name2 = Encoding.ASCII.GetString(data, 0, recv2);
                        recv = server.ReceiveFrom(data, ref remoteEP);
                        String receiver = Encoding.ASCII.GetString(data, 0, recv);
                        recv = server.ReceiveFrom(data, ref remoteEP);
                        String mes2 = Encoding.ASCII.GetString(data, 0, recv);
                        string mes = name2 + ": " + mes2;
                        if (users.ContainsKey(receiver))
                        {
                            server.SendTo(Encoding.ASCII.GetBytes(mes), recv + recv2 + 2, SocketFlags.None, users[receiver]);
                            Console.WriteLine("set message from " + name2 + " to " + receiver + ": " + mes2);
                        } else
                            server.SendTo(Encoding.ASCII.GetBytes("SERVER: no such user"), 20, SocketFlags.None, users[name2]);





                        break;

                    case "quit":
                        recv = server.ReceiveFrom(data, ref remoteEP);
                        String name3 = Encoding.ASCII.GetString(data, 0, recv);
                        users.Remove(name3);
                        break;
                    //default:
                    /*    int i = 0;
                        String Data = Encoding.ASCII.GetString(data);
                        while (i < list.Count)
                        {

                            server.SendTo(Encoding.ASCII.GetBytes(Data), recv, SocketFlags.None, (EndPoint)list[i]);
                            ++i;
                        }
                        break;*/
                }
                
            }
        }
    }
}
