using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static PublicLabFactors;

public class ServerController : MonoBehaviour
{

    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;

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

        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    void Update()
    {
        GlobalController.Instance.serverip = getIPAddress();
        bool isConnecting = connectedTcpClient != null;
        
        GlobalController.Instance.setConnectingStatus(isConnecting);

        while(receivedQueue.Count !=0)
        {
            processReceivedMessage();
        }
    }

    private void ListenForIncommingRequests()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, 8052);
            tcpListener.Start();
            Debug.Log("Server is listening");
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    using (NetworkStream stream = connectedTcpClient.GetStream())
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
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    private void sendMessage(string serverMessage)
    {
        if (connectedTcpClient == null)
        {
            return;
        }
        try
        {
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                serverMessage += stringSeparators[0];
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                //Debug.Log("Server sent his message - should be received by client");
                Debug.Log("S sendMsg: " + serverMessage);
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
        Debug.Log("S rcvMsg: " + receiveMsg);
        string[] messages = receiveMsg.Split(paramSeperators);
        MessageType msgType = (MessageType)Enum.Parse(typeof(MessageType), messages[0]);
        if (msgType == MessageType.Angle)
        {
            analyzeAngleInfo(messages);
        }
        else if(msgType == MessageType.Scene)
        {
            analyzeSceneInfo(messages);
        }
        else if (msgType == MessageType.Trial)
        {
            analyzeTrialInfo(messages);
        }
    }

    private void analyzeSceneInfo(string[] messages)
    {
        GlobalController.Instance.curClientScene = (LabScene)Enum.Parse(typeof(LabScene), messages[1]);
    }

    private void analyzeAngleInfo(string[] messages)
    {
        Vector3 accReceived = new Vector3(
            Convert.ToSingle(messages[1]),
            Convert.ToSingle(messages[2]),
            Convert.ToSingle(messages[3])
        );
        GlobalController.Instance.accClient = accReceived;
    }

    private void analyzeTrialInfo(string[] messages)
    {
        Vector3 cAcc = new Vector3(
            Convert.ToSingle(messages[1]),
            Convert.ToSingle(messages[2]),
            Convert.ToSingle(messages[3]));

        int cTrialIndex = Convert.ToInt32(messages[4]);
        int cRepeatIndex = Convert.ToInt32(messages[5]);
        int cTarget2id = Convert.ToInt32(messages[6]);
        string cTrialPhase = messages[7];
        string cTouch2data = messages[8];
        bool sendMessageToClientAgain = false;
        
        sendMessageToClientAgain = GlobalController.Instance.
            checkClientTarget2Touch(cTrialIndex, cRepeatIndex, cTarget2id, 
            cTrialPhase, cTouch2data);
        if (sendMessageToClientAgain)
        {
            prepareNewMessage4Client(MessageType.Trial);
        } else
        {
            GlobalController.Instance.accClient = cAcc;
        }
    }

    public void prepareNewMessage4Client(MessageType msgType, ServerCommand cmd)
    {
        if (msgType == MessageType.Command)
        {
            string msgContent = msgType.ToString() + paramSeperators + cmd.ToString() + paramSeperators;
            sendMessage(msgContent);
        }
        
    }
    public void prepareNewMessage4Client(MessageType msgType)
    {
        string msgContent = "";
        if (msgType == MessageType.Block)
        {
            //int blockid = GlobalController.Instance.curBlockid;
            string targetLab = GlobalController.Instance.getLabSceneToEnter();
            msgContent = msgType.ToString() + paramSeperators
                       + targetLab + paramSeperators;
            
        }
        else if(msgType == MessageType.Scene)
        {
            string serverScene = GlobalController.Instance.curServerScene.ToString();
            msgContent = msgType.ToString() + paramSeperators
                        + serverScene + paramSeperators;
        }
        else if (msgType == MessageType.Trial)
        {
            //float angle = GlobalController.Instance.curAngle;
            int sTrialid = GlobalController.Instance.curLab1Trialid;
            int sRepetitionid = GlobalController.Instance.curLab1Repeateid;
            int sTarget2id = GlobalController.Instance.curLab1Trial.secondid;
            string sTrialPhase = GlobalController.Instance.curLab1TrialPhase.ToString();
            msgContent = msgType.ToString() + paramSeperators
                        + sRepetitionid + paramSeperators
                        + sTrialid + paramSeperators
                        + sTarget2id + paramSeperators
                        + sTrialPhase + paramSeperators;
        }
        sendMessage(msgContent);
    }
}
