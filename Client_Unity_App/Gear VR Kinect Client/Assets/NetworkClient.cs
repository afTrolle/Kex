using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Net;

public class NetworkClient : MonoBehaviour {

    //  player.AddForce(new Vector3(1,0,0));
    // player.transform.Translate(new Vector3(1,2,3));
    // player.transform.position = new Vector3(2,2,2);

    //set in unity editor
    public int port;
    public string ServerIp;

    public GameObject player;

	// Use this for initialization
	void Start () {
        clientThread clientThread = new clientThread(port, ServerIp);
        Thread listenerthread = new Thread(new ThreadStart(clientThread.ThreadRun));
        listenerthread.Start();
    }

}

public class clientThread : MonoBehaviour
{
    private int port;
    private string serverIp;

    public clientThread(int port, string serverIp)
    {
        this.port = port;
        this.serverIp = serverIp;
    }

    public void ThreadRun()
    {
        // Data buffer for incoming data.
        byte[] bytes = new byte[1024];

        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.
            // This example uses port 11000 on the local computer.
            System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(serverIp);  //127.0.0.1 as an example
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            // Create a TCP/IP  socket.
            Socket sender = new Socket(AddressFamily.InterNetwork,  SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(remoteEP);

            if (sender.IsBound)
            {
                print("connected to server");
            }
            int func = 1;
            byte[] data = BitConverter.GetBytes(func);
            sender.Send(data);
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

}
