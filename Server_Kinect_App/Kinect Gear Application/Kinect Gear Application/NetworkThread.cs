using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Kinect_Gear_Application
{
    class NetworkThread
    {

        private Socket serverSocket;
        private Socket ConnectedSocket;

        //data buffer
        private byte[] data = new byte[1024];
        public NetworkThread(Socket serverSocket)
        {
            this.serverSocket = serverSocket;
        }

        internal void start()
        {
            Console.WriteLine("Waiting for a connection...");

            ConnectedSocket = waitForConnection();

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
                 
                        updatePosition();




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

        private void updatePosition()
        {
            lock (playerLock)
            {
                if (IsPlayerPositionUpdated)
                {
                    Joint neck = playerBody.Joints[JointType.Neck];
                    if (neck.TrackingState == TrackingState.Tracked)
                    {
                        int function =  4;
                        byte[] func = BitConverter.GetBytes(function);

                        byte[] x = BitConverter.GetBytes(neck.Position.X);
                        byte[] y = BitConverter.GetBytes(neck.Position.Y);
                        byte[] z = BitConverter.GetBytes(neck.Position.Z);

                        byte[] msg = new byte[func.Length + x.Length + y.Length + z.Length];

                        System.Buffer.BlockCopy(func, 0, msg, 0, func.Length);
                        System.Buffer.BlockCopy(x, 0, msg, func.Length, x.Length);
                        System.Buffer.BlockCopy(y, 0, msg, func.Length + x.Length, y.Length);
                        System.Buffer.BlockCopy(z, 0, msg, func.Length + x.Length + y.Length, z.Length);


                        ConnectedSocket.Send(msg);
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
                    // user wants to be located

                    if (PlayerController.enableIdenticationMode(this))
                    {
                        // Respond with success 
                        // Wait for position data.

                    }
                    else
                    {
                        // Respond with fail
                    }

                    return;
                case 4:
                    Console.WriteLine("func 4 called, trying to start identification");
                    bool status = PlayerController.enableIdenticationMode(this);
                    if (status == false)
                    {
                        ConnectedSocket.Send(BitConverter.GetBytes(404));
                    }

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

        Object playerLock = new Object();
        Body playerBody;
        bool IsPlayerPositionUpdated = false; 

        internal void setPosition(Body updatedUser)
        {
            lock (playerLock)
            {
                playerBody = updatedUser;
                IsPlayerPositionUpdated = true;
            }
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
