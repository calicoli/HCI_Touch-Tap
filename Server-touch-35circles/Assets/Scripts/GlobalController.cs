using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LabFactors;

public class GlobalController : MonoBehaviour
{
    // https://www.sitepoint.com/saving-data-between-scenes-in-unity/
    /*
     * Singleton Mode
     */
    public static GlobalController Instance;

    public GameObject fileProcessor;

    [HideInInspector]
    public int userid;
    [HideInInspector]
    public string serverip;

    [HideInInspector]
    public LabScene curScene;
    [HideInInspector]
    public int curBlockid, curTrialid;
    [HideInInspector]
    public BlockCondition curBlockCondition;
    [HideInInspector]
    public WelcomePhase curEntryPhase;

    private LabInfos curLabInfos;
    private BlockSequence seqBlocks;
    private BlockCondition[] conBlocks;
    
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            curScene = LabScene.Entry_scene;
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
            conBlocks = new BlockCondition[curLabInfos.totalTrialCount + 10];

            // set variable: seqBlock
            seqBlocks.setBlockLength(LabScene.Lab1_tap_33_33);
            seqBlocks.setAllSequence(userid);

            // set variable: conBlock
            for (int blockid = 0; blockid < curLabInfos.totalBlockCount; blockid++)
            {
                int pid = (int)seqBlocks.seqPosture[blockid];
                int aid = (int)seqBlocks.seqAngle[blockid];
                int sid = (int)seqBlocks.seqShape[blockid];
                int oid = (int)seqBlocks.seqOrientation[blockid];
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
    public bool setLabParams(LabScene name)
    {
        curLabInfos = new LabInfos();
        curLabInfos.setLabInfoWithName(name);
        bool end = scheduleBlocks();
        if(end)
        {
            return true;
        }
        return false;
        
    }

    public void getBlockConditions()
    {

    }

    public void moveToNextBlock()
    {
        //setNextBlockConsitions();
        curBlockid++;
        curBlockCondition = conBlocks[curBlockid];
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

    #endregion


    #region Public Write-file Method
    public void writeAllBlockConditionsToFile()
    {
        string allUserFileName = curLabInfos.labName.ToString() + "-AllUsers.txt";
        string userFilename = curLabInfos.labName.ToString() + "-User" + userid.ToString() + ".txt";
        DateTime dt = DateTime.Now;
        string strContent = dt.ToString() + Environment.NewLine
            + "All block conditions of user" + userid.ToString() + Environment.NewLine;
        strContent += seqBlocks.getAllDataWithID(4);
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(allUserFileName, strContent);
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(userFilename, strContent);
    }

    public void writeCurrentBlockConditionToFile()
    {
        string userFilename = curLabInfos.labName.ToString() + "-User" + userid.ToString() + ".txt";
        string strContent = curBlockCondition.getAllDataForFile();
        fileProcessor.GetComponent<FileProcessor>().writeNewDataToFile(userFilename, strContent);
    }

    #endregion
}
