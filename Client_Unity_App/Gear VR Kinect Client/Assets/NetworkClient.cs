using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Net;

public class NetworkClient : MonoBehaviour
{

    //  player.AddForce(new Vector3(1,0,0));
    // player.transform.Translate(new Vector3(1,2,3));
    // player.transform.position = new Vector3(2,2,2);

    //set in unity editor
    public int port;
    public string ServerIp;

    public GameObject player;
    private Vector3 playerOrginalPos;
    clientThread _clientThread;

    private PlayerPositionHolder playerPos = new PlayerPositionHolder();

    public Canvas HintCanvas;

    // Use this for initialization
    void Start()
    {
        _clientThread = new clientThread(port, ServerIp, playerPos);
        Thread listenerthread = new Thread(new ThreadStart(_clientThread.ThreadRun));
        listenerthread.Start();

        playerOrginalPos = player.transform.position;
    }


    void Update()
    {

      Vector3 newpos =   playerPos.getPosition();
        if (newpos.x != 0f && newpos.y != 0f && newpos.z != 0f)
        {
            player.transform.transform.position = playerOrginalPos + newpos;
        }

    }


    public void ConnectButtonClicked()
    {
        print("identifcation button clicked");
        _clientThread.RequestIdentificationofKinect(HintCanvas);
        HintCanvas.enabled = false;
    }

}

public class PlayerPositionHolder
{
    UnityEngine.Object PositionLock = new UnityEngine.Object();

    float refX = 0;
    float refY = 0;
    float refZ = 0;

    float x = 0;
    float y = 0;
    float z = 0;

    public void setPoisition(float x, float y, float z)
    {
        lock (PositionLock)
        {
            if (refX == 0 && refY == 0 && refZ == 0)
            {
                refX = x;
                refY = y;
                refZ = z;
            }

            this.x = x;
            this.y = y;
            this.z = z;

        }
    }

    public Vector3 getPosition()
    {
        lock (PositionLock)
        {
                return new Vector3(x - refX, y - refY, -(z - refZ));
        }
    }

}

public class clientThread : MonoBehaviour
{
    private int port;
    private string serverIp;
    private Canvas hintCanvas;

    private System.Object syncLock = new System.Object();
    private bool sync = false;

    private PlayerPositionHolder pholder;

    public clientThread(int port, string serverIp, PlayerPositionHolder pholder)
    {
        this.port = port;
        this.serverIp = serverIp;
        this.pholder = pholder;
    }

    internal void RequestIdentificationofKinect(Canvas canvas)
    {

        lock (syncLock)
        {
            hintCanvas = canvas;
            sync = true;
        }

    }
    // Data buffer for incoming data.
    byte[] bytes = new byte[1024];

    public void ThreadRun()
    {


        // Connect to a remote device.
        try
        {
            // Establish the remote endpoint for the socket.
            // This example uses port 11000 on the local computer.
            System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(serverIp);  //127.0.0.1 as an example
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            // Create a TCP/IP  socket.
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(remoteEP);

            if (sender.IsBound)
            {
                print("connected to server");
            }
            int func = 1;
            byte[] data = BitConverter.GetBytes(func);
            sender.Send(data);

            while (true)
            {
                Thread.Sleep(10);

                if (!sender.IsBound)
                {
                    return;
                }

                if (sender.Available > 0)
                {
                    //TODO Read data


                    // read receive buffer
                    int numOfBytes = sender.Receive(bytes);

                    handleData();

                    Array.Clear(bytes, 0, data.Length);

                }

                lock (syncLock)
                {
                    if (sync)
                    {
                        sender.Send(BitConverter.GetBytes(4));
                        sync = false;

                    }
                }


            }

        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

    private void handleData()
    {
        int func = BitConverter.ToInt32(bytes, 0);

        switch (func)
        {
            case 1:
                return;

            case 4:
                // 2 byte func
                // 2 float
                // 2 float
                // 2 byte float


                float posX = BitConverter.ToSingle(bytes, 4);
                float posy = BitConverter.ToSingle(bytes, 4 + 4);
                float posz = BitConverter.ToSingle(bytes, 4 + 4 + 4);
                //  print("posx: " + posX + "posy:" + posy + "posz:" + posz);
                pholder.setPoisition(posX,posy,posz);

                return;
            case 404:

                return;
        }


    }
}
