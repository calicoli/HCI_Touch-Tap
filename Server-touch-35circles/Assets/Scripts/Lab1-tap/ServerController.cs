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

public class ServerController : MonoBehaviour
{
    public Text ipText;
    public Camera renderCamera;

    public GameObject angleProcessor;
    public GameObject trialController;

    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    private Vector3 pos = new Vector3(0, 0, 0);

    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    private string receiveMsg = "";
    private bool clientRefreshed = false;

    private bool noConnection = true;

    void Start()
    {
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    void Update()
    {
        ipText.text = getIPAddress();
        bool isConnecting = connectedTcpClient != null;
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
        angleProcessor.GetComponent<AngleProcessor>().isConnecting = isConnecting;
        trialController.GetComponent<TrialController>().isConnecting = isConnecting;

        if (isConnecting && noConnection)
        {
            // the first time to sendMessage();
            sendMessage(true, true);
            noConnection = false;
        }
        if (clientRefreshed)
        {
            getVector();
            clientRefreshed = false;
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
                            receiveMsg = Encoding.ASCII.GetString(incommingData);
                            clientRefreshed = true;
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

    private void sendMessage(bool sendAngleData, bool sendTrialData)
    {
        if (connectedTcpClient == null)
        {
            return;
        }
        string flag;
        flag = sendAngleData ? "T" : "F";
        flag = sendTrialData ? flag + "T" : flag + "F";
        float angle = angleProcessor.GetComponent<AngleProcessor>().getAngle();
        string sLabName;
        int sTrialIndex, sTrialPhase, sTarget2id;
        trialController.GetComponent<TrialController>().
            getParams4Client(out sLabName, out sTrialIndex, out sTrialPhase, out sTarget2id);

        try
        {
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                string serverMessage =
                    flag + "," +
                    angle + "," +
                    sLabName + "," +
                    sTrialIndex + "," +
                    sTrialPhase + "," +
                    sTarget2id + ","
                    ;
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
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

    private void getVector()
    {
        Debug.Log("S rcvMsg: " + receiveMsg);
        string[] messages = receiveMsg.Split(',');

        char refreshAngle = messages[0][0];
        char refreshTrial = messages[0][1];

        if (refreshAngle - 'T' == 0)
        {
            Vector3 accClient = new Vector3(
                System.Convert.ToSingle(messages[1]),
                System.Convert.ToSingle(messages[2]),
                System.Convert.ToSingle(messages[3]));
        }
        if (refreshTrial - 'T' == 0)
        {
            string cLabName = messages[4];
            int cTrialIndex = System.Convert.ToInt32(messages[5]);
            int cTrialPhase = System.Convert.ToInt32(messages[6]);
            int cTarget2id = System.Convert.ToInt32(messages[7]);
            bool cPhaseFinished = (messages[8][0] - 'T' == 0);
            bool cPhaseSuccess = (messages[8][1] - 'T' == 0);

            bool sendMessageToClientAgain = false;
            sendMessageToClientAgain = trialController.GetComponent<TrialController>().
                checkClientTargetTouch(cLabName, cTrialIndex, cTrialPhase, cTarget2id, cPhaseFinished, cPhaseSuccess);
            if (!sendMessageToClientAgain)
            {
                prepareNewMessage4Client(false, true);
            }

        }
    }
    public void prepareNewMessage4Client(bool refreshAngle, bool refreshTrial)
    {
        sendMessage(refreshAngle, refreshTrial);
    }
}
