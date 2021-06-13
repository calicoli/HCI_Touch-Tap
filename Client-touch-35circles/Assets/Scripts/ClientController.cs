using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PublicLabFactors;

public class ClientController : MonoBehaviour
{

    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    private string thisLabName, serverLabName;

    private Queue receivedQueue;

    private static char paramSeperators = ';';
    private static string[] stringSeparators = new string[] { "//##MSGEND##//" };


    void Start()
    {
        receivedQueue = new Queue();
        if (receivedQueue.Count != 0)
        {
            receivedQueue.Clear();
        }


        socketConnection = null;
        
        //thisLabName = trialController.GetComponent<TrialController>().getClientLabName();
    }

    void Update()
    {
        bool isConnecting = (socketConnection != null);
        GlobalController.Instance.setConnectingStatus(isConnecting);

        while (receivedQueue.Count != 0)
        {
            processReceivedMessage();
        }
    }

    private void ConnectToTcpServer(string ipText)
    {
        try
        {
            clientReceiveThread = new Thread(() => ListenForData(ipText));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }

    private void ListenForData(string ipText)
    {
        socketConnection = null;
        try
        {
            socketConnection = new TcpClient(ipText, 8052);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        string streamMsg = Encoding.ASCII.GetString(incommingData);
                        string[] rawMsg = streamMsg.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                        for(int i=0; i< rawMsg.Length; i++)
                        {
                            receivedQueue.Enqueue(rawMsg[i]);
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void sendMessage(string clientMessage)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                clientMessage += stringSeparators[0];
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("C sendMsg: " + clientMessage);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private string getIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    private void processReceivedMessage()
    {
        string receiveMsg = (string)receivedQueue.Dequeue();
        Debug.Log("C rcvMsg: " + receiveMsg);
        string[] messages = receiveMsg.Split(';');
        MessageType msgType = (MessageType)Enum.Parse(typeof(MessageType), messages[0]);
        if (msgType == MessageType.Command) {
            analyzeCommand(messages);
        }
        else if(msgType == MessageType.Block)
        {
            analyzeBlockInfo(messages);
        }
        else if (msgType == MessageType.Scene)
        {
            analyzeSceneInfo(messages);
        }
        else if (msgType == MessageType.Trial)
        {
            analyzeTrialInfo(messages);
        }
    }

    private void analyzeCommand(string[] messages)
    {
        ServerCommand cmd = (ServerCommand)Enum.Parse(typeof(ServerCommand), messages[1]);
        GlobalController.Instance.addServerCommandToQueue(cmd);
    }

    private void analyzeBlockInfo(string[] messages)
    {
        GlobalController.Instance.setTargetLabName(messages[1]);
    }

    private void analyzeSceneInfo(string[] messages)
    {
        GlobalController.Instance.curServerScene = (LabScene)Enum.Parse(typeof(LabScene), messages[1]);
    }

    private void analyzeTrialInfo(string[] messages)
    {
        int sRepetitionid= Convert.ToInt32(messages[1]);
        int sTrialid = Convert.ToInt32(messages[2]);
        int sTarget2id = Convert.ToInt32(messages[3]);
        string sTrialPhase = messages[4];

        GlobalController.Instance.
            receiveTrialDataFromServer(sRepetitionid, sTrialid, sTarget2id, sTrialPhase);
        
    }

    public void connect(string address)
    {
        /*
#if UNITY_ANDROID && UNITY_EDITOR
        //string address_editor = "192.168.236.1";
        //string address_editor = "10.21.39.144";
        string address_editor = "192.168.0.108";
        ConnectToTcpServer(address_editor);
        ipText.text = address_editor;
#endif

#if UNITY_IOS || UNITY_ANDROID
        string address_mobile = "192.168.0.108";
        //string address_mobile = "100.46.121.97";
        ConnectToTcpServer(address_mobile);
        ipText.text = address_mobile;
#endif
        */
        if(address != null)
        {
            ConnectToTcpServer(address);
        } else
        {
            Debug.Log("Do not have server ip yet");
        }
        
        
    }

    public void prepareNewMessage4Server(MessageType msgType)
    {
        string msgContent = "";
        if (msgType == MessageType.Angle)
        {
            Vector3 acc = Input.acceleration;
            msgContent = msgType.ToString() + paramSeperators
                        + acc.x + paramSeperators
                        + acc.y + paramSeperators
                        + acc.z + paramSeperators; 
        }
        else if (msgType == MessageType.Scene)
        {
            string clientScene = GlobalController.Instance.curClientScene.ToString();
            msgContent = msgType.ToString() + paramSeperators
                        + clientScene + paramSeperators;
            
        }
        else if(msgType == MessageType.Trial)
        {
            Vector3 acc = Input.acceleration;
            int cTrialid = GlobalController.Instance.curLab1Trialid;
            int cRepeatitionid = GlobalController.Instance.curLab1Repeateid;
            int cTarget2id = GlobalController.Instance.curLab1Trial.secondid;
            string cTrialPhase = GlobalController.Instance.curLab1TrialPhase.ToString();
            string cTouch2data = GlobalController.Instance.getTrialData4Server();
            msgContent = msgType.ToString() + paramSeperators
                        + acc.x + paramSeperators
                        + acc.y + paramSeperators
                        + acc.z + paramSeperators
                        + cTrialid + paramSeperators
                        + cRepeatitionid + paramSeperators
                        + cTarget2id + paramSeperators
                        + cTrialPhase + paramSeperators
                        + cTouch2data + paramSeperators;
        }
        sendMessage(msgContent);
    }
}
