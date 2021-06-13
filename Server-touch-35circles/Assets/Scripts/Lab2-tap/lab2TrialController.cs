using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class lab2TrialController : MonoBehaviour
{
    const string curLabName = "Lab1-tap";
    const int totalTrialTimes = 35 * 35;

    public GameObject serverController;
    public GameObject fileProcessor;
    public GameObject blockAssigner;
    public GameObject lab1TargetScheduler;

    public GameObject backBtn;
    public GameObject moveToNextBlockBtn;

    [HideInInspector]
    public bool isConnecting;
    [HideInInspector]
    public bool isPortrait;

    public enum TrialPhase
    {
        start = 0,
        awaiting = 1,
        scheduling = 2,
        inactive = 3,
        ready = 4,
        ongoing_p1 = 51,
        ongoing_p2 = 52,
        ongoing_p3 = 53,
        server_output_data = 6,
        end = 9
    }

    public struct TrialData
    {
        // assign
        int trialid;
        int firstid, secondid;
        // raw
        public int tp1Count, tp2Count;
        public long t1ShowupStamp, t2ShowupStamp;
        public long tp0SuccessStamp, tp1SuccessStamp, tp2SuccessStamp;
        public Vector2 tp0SuccessPosition, tp1SuccessPosition, tp2SuccessPosition;
        public ArrayList tp1FailPositions;
        public string tp2FailPositions;
        public long serverSendDataStamp, clientReceivedDataStamp;
        // calculate
        public bool isTrialSuccessWithNoError;
        public bool isTarget1SuccessWithNoError, isTarget2SuccessWithNoError;
        public long completeTime, dataTransferTime;
        public long target1CompleteTime, target2CompleteTime;
        public long target1ShowTime, target2ShowTime;

        public void init(int idx, int id1, int id2)
        {
            trialid = idx;
            firstid = id1;
            secondid = id2;
            tp1Count = 0;
            tp2Count = 0;
            tp1FailPositions = new ArrayList(); tp1FailPositions.Clear();
            tp2FailPositions = null;
        }

        public long calTimeSpan(long later, long earlier)
        {
            return later - earlier;
        }

        public void calMoreData()
        {
            // trial success
            isTarget1SuccessWithNoError = tp1Count == 1 ? true : false;
            isTarget2SuccessWithNoError = tp2Count == 1 ? true : false;
            isTrialSuccessWithNoError = isTarget1SuccessWithNoError && isTarget2SuccessWithNoError;
            // trial time
            completeTime = calTimeSpan(tp2SuccessStamp, tp0SuccessStamp);
            dataTransferTime = calTimeSpan(clientReceivedDataStamp, serverSendDataStamp);
            target1CompleteTime = calTimeSpan(tp1SuccessStamp, tp0SuccessStamp);
            target2CompleteTime = calTimeSpan(tp2SuccessStamp, tp1SuccessStamp);
            target1ShowTime = calTimeSpan(tp1SuccessStamp, t1ShowupStamp);
            target2ShowTime = calTimeSpan(tp2SuccessStamp, t2ShowupStamp);
        }
        public string getAllData()
        {
            calMoreData();
            Debug.Log("Part data: " + tp1Count.ToString() + tp2Count.ToString());
            string str;
                // assign
            str = trialid.ToString() + ";" + firstid.ToString() + ";" + secondid.ToString() + ";"
                // calculate
                + isTrialSuccessWithNoError.ToString() + ";"
                + isTarget1SuccessWithNoError.ToString() + ";" + isTarget2SuccessWithNoError.ToString() + ";"
                + completeTime.ToString() + ";" + dataTransferTime.ToString() + ";"
                + target1CompleteTime.ToString() + ";" + target2CompleteTime.ToString() + ";"
                + target1ShowTime.ToString() + ";" + target2ShowTime.ToString() + ";"
                // raw: success position
                + tp1SuccessPosition.ToString() + ";" + tp2SuccessPosition + ";"
                // raw: other data
                + serverSendDataStamp.ToString() + ";" + clientReceivedDataStamp.ToString() + ";"
                + tp0SuccessStamp.ToString() + ";" + tp0SuccessPosition.ToString() + ";"
                + tp1Count.ToString() + ";" 
                + t1ShowupStamp.ToString() + ";" + tp1SuccessStamp.ToString() + ";" 
                + tp2Count.ToString() + ";" 
                + t2ShowupStamp.ToString() + ";" + tp2SuccessStamp.ToString() + ";" 
                ;
            if(tp1Count > 1)
            {
                for (int i = 0; i < tp1FailPositions.Count; i++)
                {
                    str += tp1FailPositions[i].ToString() + "*";
                }
            } else
            {
                str += "T1NoError";
            }
            str += ";";
            str += (tp2Count > 1) ? tp2FailPositions : "T2NoError";
            str += ";";
            return str;
        }
    }

    
    private int curTrialIndex;
    private TrialPhase prevTrialPhase = TrialPhase.end;
    private TrialPhase curTrialPhase;
    private bool clientSaidMoveon;
    private bool haveObjectOnScreen;
    private string filename;

    private TrialData trialData;

    // Start is called before the first frame update
    void Start()
    {
        isPortrait = true;
        backBtn.SetActive(false);
        moveToNextBlockBtn.SetActive(false);

        isConnecting = false;
        clientSaidMoveon = false;
        haveObjectOnScreen = false;

        filename = "text1.txt";

        curTrialIndex = 0;
        curTrialPhase = TrialPhase.start;
        
    }
    /*
    // Update is called once per frame
    void Update()
    {
        if(prevTrialPhase != curTrialPhase)
        {
            Debug.Log("Phase: " + prevTrialPhase + "->" + curTrialPhase);
            prevTrialPhase = curTrialPhase;
        }
        if (curTrialPhase == TrialPhase.start)
        {
            bool haveNextBlock = blockAssigner.GetComponent<BlockAssigner>().haveNextBlock();
            if(haveNextBlock)
            {
                filename = blockAssigner.GetComponent<BlockAssigner>().getFilename();
                blockAssigner.GetComponent<BlockAssigner>().moveToNextBlock();
                curTrialPhase = TrialPhase.awaiting;
            }
            
        }
        else if (curTrialPhase == TrialPhase.awaiting)
        {
            bool writeFinished;
            blockAssigner.GetComponent<BlockAssigner>().writeBlockDataToFile(out writeFinished);
            if (writeFinished)
            {
                curTrialPhase = TrialPhase.scheduling;
            }
        }
        else if (curTrialPhase == TrialPhase.scheduling)
        {
            if (curTrialIndex + 1 <= totalTrialTimes)
            {
                curTrialIndex++;
                /*
                lab1TargetScheduler.GetComponent<lab1TargetScheduler>().increaseTrialIndex();
                lab1TargetScheduler.GetComponent<lab1TargetScheduler>().scheduleNewTrial(isPortrait);
                trialData = new TrialData();
                trialData.init(curTrialIndex,
                    lab1TargetScheduler.GetComponent<lab1TargetScheduler>().getCurrentTarget1id(),
                    lab1TargetScheduler.GetComponent<lab1TargetScheduler>().getCurrentTarget2id()
                    );
                curTrialPhase = TrialPhase.inactive;
                
            }
            else
            {
                curTrialPhase = TrialPhase.end;
            }
        }
        else if (curTrialPhase == TrialPhase.inactive)
        {
            if (!haveObjectOnScreen)
            {
                lab1TargetScheduler.GetComponent<lab1TargetScheduler>().updateStartBtn(true);
                haveObjectOnScreen = true;
            }

#if UNITY_IOS || UNITY_ANDROID
            bool moveToNextPhase = false;
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    moveToNextPhase = process1Touch4StartBtn(touch.position);
                    if (moveToNextPhase)
                    {
                        trialData.tp0SuccessStamp = CurrentTimeMillis();
                        trialData.tp0SuccessPosition = touch.position;
                        lab1TargetScheduler.GetComponent<lab1TargetScheduler>().updateStartBtn(false);
                        haveObjectOnScreen = false;
                        curTrialPhase = TrialPhase.ready;
                    }
                    Debug.Log("Touch the phone at " + touch.position.ToString() + moveToNextPhase.ToString());
                }
            }
#endif
#if UNITY_ANDROID && UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
            {
                moveToNextPhase = process1Touch4StartBtn(Input.mousePosition);
                if (moveToNextPhase)
                {
                    trialData.tp0SuccessStamp = CurrentTimeMillis();
                    trialData.tp0SuccessPosition = Input.mousePosition;
                    lab1TargetScheduler.GetComponent<lab1TargetScheduler>().updateStartBtn(false);
                    haveObjectOnScreen = false;
                    curTrialPhase = TrialPhase.ready;
                }
                //Debug.Log("Click the PC screen at " + Input.mousePosition.ToString() + moveToNextPhase.ToString());
            }
#endif
        }
        else if (curTrialPhase == TrialPhase.ready)
        {
            if (!haveObjectOnScreen)
            {
                lab1TargetScheduler.GetComponent<lab1TargetScheduler>().updateTarget1(true);
                trialData.t1ShowupStamp = CurrentTimeMillis();
                haveObjectOnScreen = true;
            }

#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    trialData.tp1Count++;
                    bool touchSuccess = process1Touch4Target1(touch.position);
                    if (touchSuccess)
                    {
                        trialData.tp1SuccessStamp = CurrentTimeMillis();
                        trialData.tp1SuccessPosition = touch.position;
                        lab1TargetScheduler.GetComponent<lab1TargetScheduler>().updateTarget1(false);
                        haveObjectOnScreen = false;
                        curTrialPhase = TrialPhase.ongoing_p1;
                    }
                    else
                    {
                        trialData.tp1FailPositions.Add(touch.position);
                    }
                }
            }
#endif
#if UNITY_ANDROID && UNITY_EDITOR
            if (Input.GetMouseButtonUp(0))
            {
                trialData.tp1Count++;
                bool touchSuccess = process1Touch4Target1(Input.mousePosition);
                if (touchSuccess)
                {
                    trialData.tp1SuccessStamp = CurrentTimeMillis();
                    trialData.tp1SuccessPosition = Input.mousePosition;
                    lab1TargetScheduler.GetComponent<lab1TargetScheduler>().updateTarget1(false);
                    haveObjectOnScreen = false;
                    curTrialPhase = TrialPhase.ongoing_p1;
                }
                else
                {
                    trialData.tp1FailPositions.Add(Input.mousePosition);
                }

            }
#endif
        }
        else if (curTrialPhase == TrialPhase.ongoing_p1)
        {
            if (isConnecting)
            {
                trialData.serverSendDataStamp = CurrentTimeMillis();
                //serverController.GetComponent<ServerController>().prepareNewMessage4Client(false, true);

                curTrialPhase = TrialPhase.ongoing_p2;
            }

        }
        else if (curTrialPhase == TrialPhase.ongoing_p2)
        {
            bool moveToNextPhase = clientSaidMoveon;
            if (isConnecting && moveToNextPhase)
            {
                clientSaidMoveon = false;
                curTrialPhase = TrialPhase.ongoing_p3;
            }
        }
        else if (curTrialPhase == TrialPhase.ongoing_p3)
        {
            curTrialPhase = TrialPhase.server_output_data;
        }
        else if (curTrialPhase == TrialPhase.server_output_data)
        {
            bool writeFinished;
            string strContent = trialData.getAllData();
            fileProcessor.GetComponent<FileProcessor>().
                writeNewDataToFile(filename, strContent, out writeFinished);
            if (writeFinished)
            {
                curTrialPhase = TrialPhase.scheduling;
            }
            
            
        }
        else if (curTrialPhase == TrialPhase.end)
        {
            backBtn.SetActive(true);
            moveToNextBlockBtn.SetActive(true);
        }
        else
        {
            Debug.LogError("Something bad happened.");
        }
        
    }
    
    bool process1Touch4StartBtn(Vector2 pos)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("info: " + hit.collider.gameObject.name);
            Debug.DrawLine(ray.origin, hit.point, Color.yellow);
            if(string.Equals(hit.collider.gameObject.name, "StartBtn-square"))
            {
                return true;
            }
        }

        return false;
    }

    bool process1Touch4Target1(Vector2 pos)
    {
        int hitid = -1;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit))
        {
            hitid = Convert.ToInt32(hit.collider.gameObject.name.Substring(9, 2));
            Debug.Log("info: " + hitid.ToString() + " " + hit.collider.gameObject.name);
            Debug.DrawLine(ray.origin, hit.point, Color.yellow);
        }

        int targetid = lab1TargetScheduler.GetComponent<lab1TargetScheduler>().getCurrentTarget1id();
        if(hitid == targetid)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    // timer: https://www.jianshu.com/p/3cc24fb852d7
    long CurrentTimeMillis()
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
        return (long)diff.TotalMilliseconds;
    }

    long AnthoerWayToGetCurrentTimeMillis()
    {
        return(long)(DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }
    
    void parseTouch2DataString(string cTouchMsg)
    {
        Debug.Log("Befor parse: " + cTouchMsg);
        string[] messages = cTouchMsg.Split('#');
        int cTrialIndex = System.Convert.ToInt32(messages[0]);
        int cTarget2id = System.Convert.ToInt32(messages[1]);
        if (cTrialIndex == curTrialIndex &&
            cTarget2id == lab1TargetScheduler.GetComponent<lab1TargetScheduler>().getCurrentTarget2id()
            )
        {
            trialData.clientReceivedDataStamp = System.Convert.ToInt64(messages[2]);
            trialData.tp2Count = System.Convert.ToInt32(messages[3]);
            trialData.t2ShowupStamp = System.Convert.ToInt64(messages[4]);
            trialData.tp2SuccessStamp = System.Convert.ToInt64(messages[5]);
            trialData.tp2SuccessPosition = 
                new Vector2(System.Convert.ToSingle(messages[6]), System.Convert.ToSingle(messages[7]));
            trialData.tp2FailPositions = messages[8];
        }
    }
    
    
#region Public methods
    // check if server need to send message to client again
    public bool checkClientTargetTouch( string clientLabName,
        int cTrialIndex, int cTrialPhase, int cTarget2id, 
        bool cPhaseFinished, string cTouch2data)
    {
        int curTarget2id = lab1TargetScheduler.GetComponent<lab1TargetScheduler>().getCurrentTarget2id();
        if(string.Equals(clientLabName, curLabName) 
            && cTrialIndex == curTrialIndex && (cTrialPhase == (int)curTrialPhase) 
            && cTarget2id == curTarget2id)
        {
            parseTouch2DataString(cTouch2data);
            clientSaidMoveon = cPhaseFinished;
            return false;
        }
        return true;
    }

    public void getParams4Client(
        out string sLabName, 
        out int sTrialIndex, out int sTrialPhase, 
        out int sTarget2id)
    {
        sLabName = curLabName;
        sTrialIndex = curTrialIndex;
        sTrialPhase = (int)curTrialPhase;
        sTarget2id = lab1TargetScheduler.GetComponent<lab1TargetScheduler>().getCurrentTarget2id();
    }

    public void setFilename(string str)
    {
        filename = str;
    }
#endregion

}*/
