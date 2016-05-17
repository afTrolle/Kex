using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Kinect_Gear_Application;
using System.Threading;

namespace Kinect_Gear_Application
{
    class SocketHandler
    {

        Socket listener;
      public Clients clientz;


       public Response init()
        {

            try
            {
                // Establish the local endpoint for the socket.
                // Dns.GetHostName returns the name of the 
                // host running the application.
                IPHostEntry localHost = Dns.GetHostEntry(Dns.GetHostName());

                printavailableIps(localHost);
                IPAddress ipAddress = null;

                //make sure we have an ipv4 address 
                for (int i = 0; i < localHost.AddressList.Length; i++)
                {
                     if (localHost.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                     {
                        ipAddress = localHost.AddressList[i];
                        break;
                     }
                }

                if(ipAddress == null)
                {
                    return new Response(false,"Can't an avaiable ipv4 address", null);
                }

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
             listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientz = new Clients();
                listener.NoDelay = true;
                listener.Bind(localEndPoint);
                listener.Listen(10);
            
                SocketListener SL = new SocketListener(clientz, listener);
                Thread listenerthread = new Thread(new ThreadStart(SL.ThreadRun));
                listenerthread.Start();

            }
            catch (Exception e)
            {
                return new Response(false,e.Message,null );
            }


            return new Response(true, null,null);
        }


        private void printavailableIps(IPHostEntry localHost)
        {
            Console.WriteLine("Avaliable ip adrresses");
            for (int i = 0; i < localHost.AddressList.Length; i++)
            {
                Console.WriteLine(localHost.AddressList[i]);
            }

        }
    }

}


public class SocketListener
{
    private Clients client;
    private Socket listener;

    public SocketListener(Clients clien, Socket listener)
    {
        this.listener = listener;
        this.client = clien;
    }

    // This method that will be called when the thread is started
    public void ThreadRun()
    {
        Console.WriteLine("Waiting for a connection...");

        //waits until connection is made.
        Socket ConnectedClient = listener.Accept();

        //print information about the newly made connection.
        Console.WriteLine("I am connected to " + IPAddress.Parse(((IPEndPoint)ConnectedClient.RemoteEndPoint).Address.ToString()) + "on port number " + ((IPEndPoint)ConnectedClient.RemoteEndPoint).Port.ToString());

        //connection has been made so create a new thread that is waiting for a connection.
        startListenerThread(new Clients(),listener);
        byte[] bytes = new byte[1024];

        while (true)
        {

            int bytesRec = ConnectedClient.Receive(bytes);
           
            int func = bytes[0] | bytes[1] << 8;

            HandleMessage(func,bytes);

            Thread.Sleep(500);
            //TBD do Reading of the code here
            if(!ConnectedClient.Connected || ConnectedClient.Poll(-1, SelectMode.SelectError))
            {
                Console.WriteLine("client disconnected, ip: {0}",ConnectedClient.RemoteEndPoint.AddressFamily);
                return;
            }   


        }
    }

    private void HandleMessage(int func, byte[] data)
    {
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


    private void startListenerThread(Clients client, Socket listener)
    {
        SocketListener SL = new SocketListener(client, listener);
        Thread listenerthread = new Thread(new ThreadStart(SL.ThreadRun));
        listenerthread.Start();
    }
};
