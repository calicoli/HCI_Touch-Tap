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

public class ClientController : MonoBehaviour
{
    public Text ipText;
    public GameObject connectBtn;
    public Camera renderCamera;

    public GameObject angleProcessor;
    public GameObject trialController;

    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    private TcpClient socketConnection;
    private Thread clientReceiveThread;

    private string receiveMsg;
    private bool refreshed = false;

    private string thisLabName, serverLabName;

    void Start()
    {
        socketConnection = null;
        ipText.text = "Client";
        thisLabName = trialController.GetComponent<TrialController>().getClientLabName();
        connectBtn.SetActive(true);
    }

    void Update()
    {
        bool isConnecting = (socketConnection != null);
        connectBtn.SetActive(!isConnecting);
        renderCamera.backgroundColor = (!isConnecting ? disconnectColor : connectColor);
        ipText.gameObject.SetActive(isConnecting);
        angleProcessor.GetComponent<AngleProcessor>().isConnecting = isConnecting;

        if (refreshed)
        {
            getVector();
            refreshed = false;
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
                        receiveMsg = Encoding.ASCII.GetString(incommingData);
                        refreshed = true;
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void prepareNewMessage4Server(bool refreshAngle, bool refreshTrial)
    {
        sendMessage(refreshAngle, refreshTrial);
    }

    private void sendMessage(bool refreshAngle, bool refreshTrial)
    {
        
        if (socketConnection == null) {
            return;
        }
        string flag;
        flag = refreshAngle ? "T" : "F";
        flag = refreshTrial ? flag + "T" : flag + "F";
        Vector3 acc = Input.acceleration;
        string cLabName;
        int cTrialIndex, cTrialPhase, cTarget2id;
        bool cPhaseFinished, cPhaseSuccess;
        trialController.GetComponent<TrialController>().
            getParams4Server(out cLabName, out cTrialIndex, out cTrialPhase, out cTarget2id, out cPhaseFinished, out cPhaseSuccess);
        string flagPhase;
        flagPhase = cPhaseFinished ? "T" : "F";
        flagPhase = cPhaseSuccess ? flagPhase + "T" : flagPhase + "F";
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage =
                    flag + "," +
                    acc.x + "," +
                    acc.y + "," +
                    acc.z + "," +
                    cLabName + "," +
                    cTrialIndex + "," +
                    cTrialPhase + "," +
                    cTarget2id + "," +
                    flagPhase + ","
                    ;
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
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

    private void getVector()
    {
        Debug.Log("C rcvMsg: " + receiveMsg);
        string[] messages = receiveMsg.Split(',');

        char refreshAngle = messages[0][0];
        char refreshTrial = messages[0][1];

        if (refreshAngle - 'T' == 0)
        {
            float sAngle = System.Convert.ToSingle(messages[1]);
        }
        if (refreshTrial - 'T' == 0)
        {
            serverLabName = messages[2];
            int sTrialIndex = System.Convert.ToInt32(messages[3]);
            int sTrialPhase = System.Convert.ToInt32(messages[4]);
            int sTarget2id = System.Convert.ToInt32(messages[5]);

            trialController.GetComponent<TrialController>().receiveServerParams(
                socketConnection != null,
                string.Equals(thisLabName, serverLabName),
                sTrialIndex,
                sTrialPhase,
                sTarget2id
                );
        }
    }

    public void connect()
    {
#if UNITY_ANDROID && UNITY_EDITOR
        //string address_editor = "192.168.236.1";
        string address_editor = "10.21.37.169";
        ConnectToTcpServer(address_editor);
#endif
#if UNITY_IOS || UNITY_ANDROID
        string address_mobile = "192.168.0.108";
        ConnectToTcpServer(address_mobile);
#endif
    }
}
