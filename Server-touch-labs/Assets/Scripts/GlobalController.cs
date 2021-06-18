using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicLabFactors;
using static PublicBlockFactors;

public class GlobalController : MonoBehaviour
{
    // https://www.sitepoint.com/saving-data-between-scenes-in-unity/
    /*
     * Singleton Mode
     */
    public static GlobalController Instance;

    public ServerController server;
    public FileProcessor fileProcessor;
    public AngleProcessor angleProcessor;

    // entry params
    [HideInInspector]
    public int userid;
    [HideInInspector]
    public string serverip;
    [HideInInspector]
    public Vector3 accClient;
    [HideInInspector]
    public float curAngle;
    [HideInInspector]
    public bool isUserLabInfoSet;

    [HideInInspector]
    public ServerCommand curServerCommand;
    [HideInInspector]
    public LabScene curServerScene, curClientScene;

    // block params
    [HideInInspector]
    public int curBlockid;
    [HideInInspector]
    public lab0Factors.BlockCondition curLab0BlockCondition;
    [HideInInspector]
    public lab1Factors.BlockCondition curLab1BlockCondition;
    [HideInInspector]
    public WelcomePhase curEntryPhase;
    [HideInInspector]
    public LabInfos curLabInfos;

    // public trial params
    [HideInInspector]
    public bool clientRefreshedTrialData;

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
    public lab0Factors.TrialSequence curLab0TrialSequence;
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
    public lab1Factors.TrialSequence curLab1TrialSequence;
    public lab1Factors.TrialDataWithLocalTime curLab1TrialData;

    private bool isConnecting;
    
    private BlockSequence seqBlocks;
    private lab0Factors.BlockCondition[] conLab0Blocks;
    private lab1Factors.BlockCondition[] conLab1Blocks;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            curServerScene = LabScene.Entry_scene;
            curClientScene = LabScene.Entry_scene;
            curBlockid = trial_start_index;
            curEntryPhase = WelcomePhase.in_entry_scene;
            isUserLabInfoSet = false;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }


    private bool scheduleBlocks() 
    {
        if (curLabInfos.labName == LabScene.Lab0_tap_5_5)
        {
            conLab0Blocks = new lab0Factors.BlockCondition[curLabInfos.totalBlockCount + 1];

            // set variable: seqBlock
            seqBlocks.setBlockLength(LabScene.Lab0_tap_5_5);
            seqBlocks.setAllSequence(userid);

            // set variable: conBlock
            for (int blockid = block_start_index; blockid <= curLabInfos.totalBlockCount; blockid++)
            {
                int pid = (int)seqBlocks.seqPosture[blockid - 1];
                int aid = (int)seqBlocks.seqAngle[blockid - 1];
                int sid = (int)seqBlocks.seqShape[blockid - 1];
                int oid = (int)seqBlocks.seqOrientation[blockid - 1];
                conLab0Blocks[blockid] = new lab0Factors.BlockCondition(blockid, pid, aid, sid, oid);
            }

            curLab0BlockCondition = conLab0Blocks[curBlockid];
            return true;
        }
        else if (curLabInfos.labName == LabScene.Lab1_tap_33_33)
        {
            conLab1Blocks = new lab1Factors.BlockCondition[curLabInfos.totalBlockCount+1];

            // set variable: seqBlock
            seqBlocks.setBlockLength(LabScene.Lab1_tap_33_33);
            seqBlocks.setAllSequence(userid);

            // set variable: conBlock
            for (int blockid = block_start_index; blockid <= curLabInfos.totalBlockCount; blockid++)
            {
                int pid = (int)seqBlocks.seqPosture[blockid-1];
                int aid = (int)seqBlocks.seqAngle[blockid-1];
                int sid = (int)seqBlocks.seqShape[blockid-1];
                int oid = (int)seqBlocks.seqOrientation[blockid-1];
                conLab1Blocks[blockid] = new lab1Factors.BlockCondition(blockid, pid, aid, sid, oid);
            }

            curLab1BlockCondition = conLab1Blocks[curBlockid];
            return true;
        }
        return false;
        
    }

    private ArrayList getPalindromeArrayList(ArrayList list)
    {
        ArrayList res = list;
        for(int i = list.Count-1; i >= 0; i--)
        {
            res.Add(list[i]);
        }
        return res;
    }

    private long CurrentTimeMillis()
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
        return (long)diff.TotalMilliseconds;
    }

    private long AnthoerWayToGetCurrentTimeMillis()
    {
        return (long)(DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    #region Public Method
    public void setConnectingStatus(bool con)
    {
        isConnecting = con;
        //angleProcessor.isConnecting = isConnecting;
    }

    public bool getConnectionStatus()
    {
        return isConnecting;
    }

    public bool setLabParams(LabScene name, bool isFullMode)
    {
        curLabInfos = new LabInfos();
        curLabInfos.setLabInfoWithName(name);
        curLabInfos.setTotalTrialCount(isFullMode);
        bool end = scheduleBlocks();
        if(end)
        {
            return true;
        }
        return false;
    }

    public void moveToNextBlock()
    {
        curBlockid++;
        switch (curLabInfos.labName)
        {
            case LabScene.Lab0_tap_5_5:
                curLab0BlockCondition = conLab0Blocks[curBlockid];
                break;
            case LabScene.Lab1_tap_33_33:
                curLab1BlockCondition = conLab1Blocks[curBlockid];
                break;
        }

        server.prepareNewMessage4Client(MessageType.Command, ServerCommand.server_say_exit_lab);
        curEntryPhase = WelcomePhase.check_client_scene;
        string entrySceneName = (LabScene.Entry_scene).ToString();
        SceneManager.LoadScene(entrySceneName);
    }

    public bool haveNextBlock()
    {
        if(curBlockid + 1 <= curLabInfos.totalBlockCount)
        {
            return true;
        }
        return false;
    }

    public string getLabSceneToEnter()
    {
        string str = curLabInfos.labName.ToString();
        return str;
    }

    public void excuteCommand(ServerCommand cmd)
    {
        curServerCommand = cmd;
        server.prepareNewMessage4Client(MessageType.Command, cmd);

    }

    public bool checkClientTarget2Touch(int cTrialIndex, int cRepeateIndex,
       int cTarget2id, string cTrialPhase, string cTouch2data)
    {
        switch (curLabInfos.labName)
        {
            case LabScene.Lab0_tap_5_5:
                int sTarget2idLab0 = curLab0Trial.secondid;
                if (cTrialIndex == curLab0Trialid && cRepeateIndex == curLab0Repeateid
                    && cTarget2id == sTarget2idLab0 && cTrialPhase.Equals(curLab0TrialPhase.ToString()))
                {
                    parseLab0Touch2DataString(cTouch2data);
                    clientRefreshedTrialData = true;
                    return false;
                }
                break;
            case LabScene.Lab1_tap_33_33:
                int sTarget2idLab1 = curLab1Trial.secondid;
                if (cTrialIndex == curLab1Trialid && cRepeateIndex == curLab1Repeateid
                    && cTarget2id == sTarget2idLab1 && cTrialPhase.Equals(curLab1TrialPhase.ToString()))
                {
                    parseLab1Touch2DataString(cTouch2data);
                    clientRefreshedTrialData = true;
                    return false;
                }
                break;
        }
        return true;
    }
    public void parseLab0Touch2DataString(string cTouchMsg)
    {
        curLab0TrialData.localTime.serverReceiveDataStamp = CurrentTimeMillis();
        Debug.Log("Befor parse: " + cTouchMsg);
        string[] messages = cTouchMsg.Split('#');
        int cTrialIndex = Convert.ToInt32(messages[0]);
        int cTarget2id = Convert.ToInt32(messages[1]);
        curLab0TrialData.localTime.clientReceiveDataStamp = Convert.ToInt64(messages[2]);
        curLab0TrialData.localTime.clientSendDataStamp = Convert.ToInt64(messages[3]);
        curLab0TrialData.tp2Count = Convert.ToInt32(messages[4]);
        curLab0TrialData.localTime.t2ShowupStamp = Convert.ToInt64(messages[5]);
        curLab0TrialData.localTime.tp2SuccessStamp = Convert.ToInt64(messages[6]);
        curLab0TrialData.tp2SuccessPosition =
            new Vector2(Convert.ToSingle(messages[7]), Convert.ToSingle(messages[8]));
        curLab0TrialData.tp2FailPositions = messages[9];
    }
    public void parseLab1Touch2DataString(string cTouchMsg)
    {
        curLab1TrialData.localTime.serverReceiveDataStamp = CurrentTimeMillis();
        Debug.Log("Befor parse: " + cTouchMsg);
        string[] messages = cTouchMsg.Split('#');
        int cTrialIndex = Convert.ToInt32(messages[0]);
        int cTarget2id = Convert.ToInt32(messages[1]);
        curLab1TrialData.localTime.clientReceiveDataStamp = Convert.ToInt64(messages[2]);
        curLab1TrialData.localTime.clientSendDataStamp = Convert.ToInt64(messages[3]);
        curLab1TrialData.tp2Count = Convert.ToInt32(messages[4]);
        curLab1TrialData.localTime.t2ShowupStamp = Convert.ToInt64(messages[5]);
        curLab1TrialData.localTime.tp2SuccessStamp = Convert.ToInt64(messages[6]);
        curLab1TrialData.tp2SuccessPosition =
            new Vector2(Convert.ToSingle(messages[7]), Convert.ToSingle(messages[8]));
        curLab1TrialData.tp2FailPositions = messages[9];
    }

    #endregion


    #region Public Write-file Method
    public void writeAllBlockConditionsToFile()
    {
        string allUserFileName = curLabInfos.labName.ToString() + "-" + curLabInfos.labMode.ToString() +"-AllUsers.txt";
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        DateTime dt = DateTime.Now;
        string strContent = dt.ToString() + Environment.NewLine
            + "All block conditions of user" + userid.ToString() + Environment.NewLine;
        //strContent += seqBlocks.getAllDataWithID(4);
        strContent += seqBlocks.getAllDataWithLabName();
        fileProcessor.writeNewDataToFile(allUserFileName, strContent);
        fileProcessor.writeNewDataToFile(userFilename, strContent);
    }

    public void writeCurrentBlockConditionToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine;
        switch(curLabInfos.labName)
        {
            case LabScene.Lab0_tap_5_5:
                strContent += curLab0BlockCondition.getAllDataForFile();
                break;
            case LabScene.Lab1_tap_33_33:
                strContent += curLab1BlockCondition.getAllDataForFile();
                break;
            default:
                break;
        }
        strContent += Environment.NewLine + string.Format("B{0:D2};", curBlockid) + getCurrentShapeAndAngle() + Environment.NewLine;
        fileProcessor.writeNewDataToFile(userFilename, strContent);
    }

    public void writeCurrentRepeatIndexTrialSequenceToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = "";
        switch (curLabInfos.labName)
        {
            case LabScene.Lab0_tap_5_5:
                strContent = curLab0TrialSequence.getAllDataForFile();
                break;
            case LabScene.Lab1_tap_33_33:
                strContent = curLab1TrialSequence.getAllDataForFile();
                break;
        }
        fileProcessor.writeNewDataToFile(userFilename, strContent);
    }
    
    public void writeCurrentTrialDataToFile(out bool finishedWrite)
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = "";
        switch (curLabInfos.labName)
        {
            case LabScene.Lab0_tap_5_5:
                strContent = curLab0TrialData.getAllDataForFile();
                break;
            case LabScene.Lab1_tap_33_33:
                strContent = curLab1TrialData.getAllDataForFile();
                break;
        }
        strContent += getCurrentShapeAndAngle();
        
        fileProcessor.writeNewDataToFile(userFilename, strContent);
        finishedWrite = true;
    }

    public void writeAllBlocksFinishedFlagToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = Environment.NewLine + DateTime.Now.ToString() 
            + "~~AllBlocksFinished!~~"+ Environment.NewLine;
        fileProcessor.writeNewDataToFile(userFilename, strContent);
    }

    public string getCurrentShapeAndAngle()
    {
        string res = "";
        switch (curLabInfos.labName)
        {
            case LabScene.Lab0_tap_5_5:
                res = curLab0BlockCondition.getShape() + ";" + curAngle + ";";
                break;
            case LabScene.Lab1_tap_33_33:
                res = curLab1BlockCondition.getShape() + ";" + curAngle + ";";
                break;
            default:
                break;
        }
        return res;
    }
    #endregion
}
