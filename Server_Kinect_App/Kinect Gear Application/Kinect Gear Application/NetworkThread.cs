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
    class NetworkThread
    {
        private Socket serverSocket;

        //data buffer
        private byte[] data = new byte[1024];
        public NetworkThread(Socket serverSocket)
        {
            this.serverSocket = serverSocket;
        }

        internal void start()
        {
            Console.WriteLine("Waiting for a connection...");

            Socket ConnectedSocket = waitForConnection();

            StartListenerThread();

            while (true)
            {
                Thread.Sleep(16);

                try
                {

                    //check if there are any message to recive.
                    if (ConnectedSocket.Available > 0)
                    {

                        // read receive buffer
                        int numOfBytes = ConnectedSocket.Receive(data);

                        handleData();

                        Array.Clear(data, 0, data.Length);

                    }

                    //TO DO check if new position is found. 
                    /* if (Player)
                     {

                     }
                     */


                }
                catch (Exception e)
                {
                    if (e.InnerException is ObjectDisposedException)
                    {
                        return;
                    }
                }
            }
        }

        private void handleData()
        {
            int func = data[0] | data[1] << 8;

            switch (func)
            {
                case 1:
                    Console.WriteLine("func 1 called");
                    return;
                case 2:
                    // user wants too sync 
                    return;
            }


        }




        //wait until someone connects
        private Socket waitForConnection()
        {

            //waits until connection is made.
            Socket ConnectedClient = serverSocket.Accept();

            //print information about the newly made connection.
            Console.WriteLine("Client connected from: " + IPAddress.Parse(((IPEndPoint)ConnectedClient.RemoteEndPoint).Address.ToString()) + "on port number " + ((IPEndPoint)ConnectedClient.RemoteEndPoint).Port.ToString());



            return ConnectedClient;
        }


        //creates new thread with main server socket.
        private void StartListenerThread()
        {
            NetworkThread nt = new NetworkThread(serverSocket);
            Thread netowrkThread = new Thread(new ThreadStart(nt.start));
            netowrkThread.Start();
        }
    }
}
