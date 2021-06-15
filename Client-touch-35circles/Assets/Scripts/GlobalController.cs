using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;

public class GlobalController : MonoBehaviour
{
    /*
     * Singleton Mode
     */
    public static GlobalController Instance;

    public GameObject client;
    public AngleProcessor angleProcessor;

    // entry params
    [HideInInspector]
    public string serverip;

    [HideInInspector]
    public Queue<ServerCommand> serverCmdQueue;
    [HideInInspector]
    public LabScene curServerScene, curClientScene;

    // block params
    [HideInInspector]
    public int curBlockid;
    [HideInInspector]
    public BlockCondition curBlockCondition;

    // entry params
    [HideInInspector]
    public WelcomePhase curEntryPhase;
    [HideInInspector]
    public bool isLabInfoSet;

    // lab1 params
    [HideInInspector]
    public lab1Factors.Lab1Phase curLab1Phase;
    [HideInInspector]
    public int curLab1Trialid, curLab1Repeateid;
    [HideInInspector]
    public lab1Factors.Trial curLab1Trial;
    [HideInInspector]
    public lab1Factors.TrialPhase curLab1TrialPhase;
    [HideInInspector]
    public lab1Factors.TrialData curTrialData;
    [HideInInspector]
    public bool serverRefreshTrialData;

    private bool isConnecting;
    private string targetLabName;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            curClientScene = LabScene.Entry_scene;
            serverCmdQueue = new Queue<ServerCommand>();
            curEntryPhase = WelcomePhase.in_entry_scene;
            isLabInfoSet = false;
            isConnecting = false;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        serverRefreshTrialData = false;
    }

    public void connectServer()
    {
        client.GetComponent<ClientController>().connect(serverip);
    }

    public void setConnectingStatus(bool con)
    {
        isConnecting = con;
        //angleProcessor.GetComponent<AngleProcessor>().isConnecting = isConnecting;
    }

    public bool getConnectionStatus()
    {
        return isConnecting;
    }

    public void addServerCommandToQueue(ServerCommand cmd)
    {
        serverCmdQueue.Enqueue(cmd);
    }

    public void setAngleDetectStatus(bool open)
    {
        angleProcessor.GetComponent<AngleProcessor>().setConveyAccStatus(open);
    }

    public void setTargetLabName(string name)
    {
        targetLabName = name;
    }

    public string getTargetLabName()
    {
        return targetLabName;
    }

    public void receiveTrialDataFromServer(int rid, int tid, int t2id, string tPhase)
    {
        curLab1Repeateid = rid;
        curLab1Trialid = tid;
        curLab1Trial.setParams(tid, t2id);
        curLab1TrialPhase = (lab1Factors.TrialPhase)Enum.Parse(typeof(lab1Factors.TrialPhase), tPhase);
        serverRefreshTrialData = true;
    }

    public string getTrialData4Server()
    {
        return curTrialData.getAllData();
    }
}
