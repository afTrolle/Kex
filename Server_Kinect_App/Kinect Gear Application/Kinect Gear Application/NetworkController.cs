using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kinect_Gear_Application
{
    class NetworkController
    {

        const int port = 11000;

        //start networking code
        public Response initNetworkController()
        {

            Console.WriteLine("Initializing network..");

            IPAddress localAddress = getNetworkInterface();

            if (localAddress == null)
            {
                return new Response(false, "failed finding network interface", null);
            }

            //setup socket
            Socket ServerSocket = initServerSocket(localAddress);

            try
            {
                //create thread that handels netowrk commuincation. 
                NetworkThread networkThread = new NetworkThread(ServerSocket);
                Thread netThread = new Thread(new ThreadStart(networkThread.start));
                netThread.Start();
            }
            catch (Exception e)
            {
                return new Response(false, e.Message, null);
            }


            return new Response(true, null, null);
        }

        //setup main server socket.
        private Socket initServerSocket(IPAddress localAddress)
        {
            IPEndPoint localEndPoint = new IPEndPoint(localAddress, port);

            // Create a TCP/IP socket.
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //sends packet straight away, to minimze delay
            socket.NoDelay = true;
            //set ip and port
            socket.Bind(localEndPoint);
            //number of waiting connections
            socket.Listen(10);

            return socket;
        }

        //finds an ipv4 network.
        private IPAddress getNetworkInterface()
        {
            try
            {
                // Establish the local endpoint for the socket.
                IPHostEntry localHost = Dns.GetHostEntry(Dns.GetHostName());

                //find first ipv4 address 
                for (int i = 0; i < localHost.AddressList.Length; i++)
                {
                    if (localHost.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return localHost.AddressList[i];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

    }
}
