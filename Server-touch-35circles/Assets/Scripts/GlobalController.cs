using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicLabFactors;

public class GlobalController : MonoBehaviour
{
    // https://www.sitepoint.com/saving-data-between-scenes-in-unity/
    /*
     * Singleton Mode
     */
    public static GlobalController Instance;

    public GameObject server;
    public GameObject fileProcessor;
    public GameObject angleProcessor;

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
    public BlockCondition curBlockCondition;
    [HideInInspector]
    public WelcomePhase curEntryPhase;
    [HideInInspector]
    public LabInfos curLabInfos;

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
    public bool clientRefreshedTrialData;
    [HideInInspector]
    public lab1Factors.TrialSequence curLab1TrialSequence;
    public lab1Factors.TrialData curLab1TrialData;

    private bool isConnecting;
    
    private BlockSequence seqBlocks;
    private BlockCondition[] conBlocks;
    
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
        if (curLabInfos.labName == LabScene.Lab1_tap_33_33)
        {
            conBlocks = new BlockCondition[curLabInfos.totalBlockCount+1];

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
                conBlocks[blockid] = new BlockCondition(blockid, pid, aid, sid, oid);
            }

            curBlockCondition = conBlocks[curBlockid];
            return true;
        }

        else if (curLabInfos.labName == LabScene.Lab2_tap_57_57)
        {
            //todo
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

    #region Public Method
    public void setConnectingStatus(bool con)
    {
        isConnecting = con;
        //angleProcessor.GetComponent<AngleProcessor>().isConnecting = isConnecting;
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
        curBlockCondition = conBlocks[curBlockid];

        server.GetComponent<ServerController>().prepareNewMessage4Client(MessageType.Command, ServerCommand.server_say_exit_lab);
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
        server.GetComponent<ServerController>().
            prepareNewMessage4Client(MessageType.Command, cmd);

    }

    public bool checkClientTarget2Touch(int cTrialIndex, int cRepeateIndex,
       int cTarget2id, string cTrialPhase, string cTouch2data)
    {
        int sTarget2id = curLab1Trial.secondid;
        if (cTrialIndex == curLab1Trialid && cRepeateIndex == curLab1Repeateid
            && cTarget2id == sTarget2id && cTrialPhase.Equals(curLab1TrialPhase.ToString()))
        {
            parseTouch2DataString(cTouch2data);
            clientRefreshedTrialData = true;
            return false;
        }
        return true;
    }

    public void parseTouch2DataString(string cTouchMsg)
    {
        Debug.Log("Befor parse: " + cTouchMsg);
        string[] messages = cTouchMsg.Split('#');
        int cTrialIndex = Convert.ToInt32(messages[0]);
        int cTarget2id = Convert.ToInt32(messages[1]);
        curLab1TrialData.clientReceivedDataStamp = Convert.ToInt64(messages[2]);
        curLab1TrialData.clientSendDataStamp = Convert.ToInt64(messages[3]);
        curLab1TrialData.tp2Count = Convert.ToInt32(messages[4]);
        curLab1TrialData.t2ShowupStamp = Convert.ToInt64(messages[5]);
        curLab1TrialData.tp2SuccessStamp = Convert.ToInt64(messages[6]);
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
        strContent += seqBlocks.getAllDataWithID(4);
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(allUserFileName, strContent);
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(userFilename, strContent);
    }

    public void writeCurrentBlockConditionToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = Environment.NewLine + DateTime.Now.ToString() + Environment.NewLine
            + curBlockCondition.getAllDataForFile();
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(userFilename, strContent);
    }

    public void writeCurrentRepeatIndexTrialSequenceToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = curLab1TrialSequence.getAllDataForFile();
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(userFilename, strContent);
    }
    
    public void writeCurrentTrialDataToFile(out bool finishedWrite)
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = curLab1TrialData.getAllDataForFile();
        strContent += curBlockCondition.getShape() + ";" + curAngle + ";";
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(userFilename, strContent);
        finishedWrite = true;
    }

    public void writeAllBlocksFinishedFlagToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-"
            + curLabInfos.labMode.ToString() + "-"
            + "User" + userid.ToString() + ".txt";
        string strContent = Environment.NewLine + DateTime.Now.ToString() 
            + "~~AllBlocksFinished!~~"+ Environment.NewLine;
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(userFilename, strContent);
    }
    #endregion
}
