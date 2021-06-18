using System;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;

public class GlobalController : MonoBehaviour
{
    /*
     * Singleton Mode
     */
    public static GlobalController Instance;

    public ClientController client;
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

    // entry params
    [HideInInspector]
    public WelcomePhase curEntryPhase;
    [HideInInspector]
    public bool isLabInfoSet;

    // public trial params
    [HideInInspector]
    public bool serverRefreshTrialData;

    // lab0 params
    [HideInInspector]
    public lab0Factors.Lab0Phase curLab0Phase;
    [HideInInspector]
    public int curLab0Trialid, curLab0Repeateid;
    [HideInInspector]
    public lab0Factors.Trial curLab0Trial;
    [HideInInspector]
    public lab0Factors.TrialPhase curLab0TrialPhase;
    [HideInInspector]
    public lab0Factors.TrialDataWithLocalTime curLab0TrialData;

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
    public lab1Factors.TrialDataWithLocalTime curLab1TrialData;
    [HideInInspector]
    public LabScene targetLabName;

    private bool isConnecting;
    

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            curClientScene = LabScene.Entry_scene;
            curEntryPhase = WelcomePhase.in_entry_scene;
            serverCmdQueue = new Queue<ServerCommand>();
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
        client.connect(serverip);
    }

    public void setConnectingStatus(bool con)
    {
        isConnecting = con;
        //angleProcessor.isConnecting = isConnecting;
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
        angleProcessor.setConveyAccStatus(open);
    }

    public void setLabName(string name)
    {
        targetLabName = (LabScene)Enum.Parse(typeof(LabScene), name);
    }

    public string getTargetLabName()
    {
        return targetLabName.ToString();
    }

    public void receiveTrialDataFromServer(int rid, int tid, int t2id, string tPhase)
    {
        switch(targetLabName)
        {
            case LabScene.Lab0_tap_5_5:
                curLab0Repeateid = rid;
                curLab0Trialid = tid;
                curLab0Trial.setParams(tid, t2id);
                curLab0TrialPhase = (lab0Factors.TrialPhase)Enum.Parse(typeof(lab0Factors.TrialPhase), tPhase);
                break;
            case LabScene.Lab1_tap_33_33:
                curLab1Repeateid = rid;
                curLab1Trialid = tid;
                curLab1Trial.setParams(tid, t2id);
                curLab1TrialPhase = (lab1Factors.TrialPhase)Enum.Parse(typeof(lab1Factors.TrialPhase), tPhase);
                break;
        }
        serverRefreshTrialData = true;
    }

    public string getTrialData4Server()
    {
        string res = "";
        switch (targetLabName)
        {
            case LabScene.Lab0_tap_5_5:
                res = curLab0TrialData.getAllData();
                break;
            case LabScene.Lab1_tap_33_33:
                res = curLab1TrialData.getAllData();
                break;
        }
        return res;
    }
}
