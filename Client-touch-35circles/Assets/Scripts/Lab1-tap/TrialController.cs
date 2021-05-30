using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialController : MonoBehaviour
{
    //public GameObject connectBtn;
    public GameObject clientController;
    public GameObject fileProcessor;
    public GameObject targetScheduler;

    [HideInInspector]
    public bool inSameLabWithServer, isConnecting;

    const string curLabName = "Lab1-tap";
    private int curTrialIndex;
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
        // assgin
        int trialid, secondid;
        public int tp2Count;
        public long t2ShowupStamp;
        public long tp2SuccessStamp;
        public Vector2 tp2SuccessPosition;
        public ArrayList tp2FailPositions;
        public long clientReceivedDataStamp;

        public void init(int idx, int id2)
        {
            trialid = idx;
            secondid = id2;
            tp2Count = 0;
            tp2FailPositions = new ArrayList();
            tp2FailPositions.Clear();
        }

        public string getAllData()
        {
            Debug.Log("Part data: " + tp2Count.ToString());
            string str;
            str = trialid.ToString() + "#" + secondid.ToString() + "#"
                + clientReceivedDataStamp + "#"
                + tp2Count.ToString() + "#" + t2ShowupStamp.ToString() + "#"
                + tp2SuccessStamp.ToString() + "#" 
                + tp2SuccessPosition.x.ToString() + "#"
                + tp2SuccessPosition.y.ToString() + "#"
                ;
            if (tp2Count > 1)
            {
                for (int i = 0; i < tp2FailPositions.Count; i++)
                {
                    str += tp2FailPositions[i].ToString() + "*";
                }
            }
            else
            {
                str += " ";
            }
            str += "#";
            Debug.Log("cTrialData: " + str);
            return str;
        }
    }

    private TrialPhase prevTrialPhase = TrialPhase.end;
    private TrialPhase curTrialPhase;

    private TrialData trialData;

    private bool serverRefreshData;
    private bool haveObjectOnScreen;
    //private bool clientTouchSuccess;

    void Start()
    {
        inSameLabWithServer = false;
        isConnecting = false;
        serverRefreshData = false;
        haveObjectOnScreen = false;
        //clientTouchSuccess = false;
        //connectBtn.SetActive(true);
        curTrialIndex = 0;
        curTrialPhase = TrialPhase.inactive;
    }

    // Update is called once per frame
    void Update()
    {

        if (isConnecting && inSameLabWithServer)
        {
            if (prevTrialPhase != curTrialPhase)
            {
                Debug.Log("Phase: " + prevTrialPhase + "->" + curTrialPhase);
                prevTrialPhase = curTrialPhase;
            }

            if (curTrialPhase == TrialPhase.start || curTrialPhase == TrialPhase.awaiting ||
                curTrialPhase == TrialPhase.scheduling ||
                curTrialPhase == TrialPhase.inactive || curTrialPhase == TrialPhase.ready
                )
            {
                // server is rensponsible for this phase
                // client just go on
            }
            else if (curTrialPhase == TrialPhase.ongoing_p1)
            { 
                if(serverRefreshData || haveObjectOnScreen)
                {
                    if(serverRefreshData)
                    {
                        serverRefreshData = false;
                        targetScheduler.GetComponent<TargetScheduler>().updateTarget2(true);
                        haveObjectOnScreen = true;
                        trialData.t2ShowupStamp = CurrentTimeMillis();
                    }
#if UNITY_IOS || UNITY_ANDROID
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Ended)
                        {
                            trialData.tp2Count++;
                            bool touchSuccess = process1Touch4Target2(touch.position);
                            if(touchSuccess)
                            {
                                trialData.tp2SuccessStamp = CurrentTimeMillis();
                                trialData.tp2SuccessPosition = touch.position;
                                targetScheduler.GetComponent<TargetScheduler>().updateTarget2(false);
                                haveObjectOnScreen = false;
                                curTrialPhase = TrialPhase.ongoing_p2;
                                
                            } else
                            {
                                trialData.tp2FailPositions.Add(touch.position);
                            }
                            
                        }
                    }
#endif
#if UNITY_ANDROID && UNITY_EDITOR
                    if (Input.GetMouseButtonUp(0))
                    {
                        Vector2 pos = Input.mousePosition;
                        trialData.tp2Count++;
                        bool touchSuccess = process1Touch4Target2(pos);
                        if(touchSuccess)
                        {
                            trialData.tp2SuccessStamp = CurrentTimeMillis();
                            trialData.tp2SuccessPosition = pos;
                            targetScheduler.GetComponent<TargetScheduler>().updateTarget2(false);
                            haveObjectOnScreen = false;
                            curTrialPhase = TrialPhase.ongoing_p2;
                        } else
                        {
                            trialData.tp2FailPositions.Add(pos);
                        }
                        //clientTouchSuccess = touchSuccess;
                    }
#endif
                }

            }
            else if (curTrialPhase == TrialPhase.ongoing_p2)
            {
                clientController.GetComponent<ClientController>().prepareNewMessage4Server(false, true);
                curTrialPhase = TrialPhase.ongoing_p3;
            }
            else if (curTrialPhase == TrialPhase.ongoing_p3 ||
                     curTrialPhase == TrialPhase.server_output_data || 
                     curTrialPhase == TrialPhase.end)
            {
                // server is rensponsible for this phase
                // client just go on
            }
            else
            {
                Debug.LogError("Something bad happened.");
            }
        }
        
    }

    bool process1Touch4Target2(Vector2 pos)
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

        int targetid = targetScheduler.GetComponent<TargetScheduler>().getCurrentTarget2id();
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
        return (long)(DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    #region public method
    public void receiveServerParams(
        bool connecting, bool sameLab,
        int sTrialIndex, int sTrialPhase,
        int sTarget2id)
    {
        Debug.Log("TC - rsp func: " + connecting.ToString() + sameLab.ToString() + "/"
            + sTrialIndex + "/" + sTrialIndex + "/" + sTarget2id);
        isConnecting = connecting;
        inSameLabWithServer = sameLab;
        if (connecting && sameLab)
        {
            curTrialIndex = sTrialIndex;
            curTrialPhase = (TrialPhase)sTrialPhase;
            targetScheduler.GetComponent<TargetScheduler>().
                scheduleTargetsWithServerData(sTrialIndex, sTarget2id);
            trialData.init(sTrialIndex, sTarget2id);
            trialData.clientReceivedDataStamp = CurrentTimeMillis();
            serverRefreshData = true;
            Debug.Log("TC - serverRefreshData: " + serverRefreshData);
        }
    }

    public string getClientLabName()
    {
        return curLabName;
    }

    public void getParams4Server(
        out string cLabName,
        out int cTrialIndex, out int cTrialPhase, out int cTarget2id, 
        out bool cPhaseFinished)
    {

        cLabName = curLabName;
        cTrialIndex = curTrialIndex;
        cTrialPhase = (int)curTrialPhase;
        cTarget2id = targetScheduler.GetComponent<TargetScheduler>().getCurrentTarget2id();
        cPhaseFinished = true;
        //cPhaseSuccess = clientTouchSuccess;
    }

    public string getTrialData4Server()
    {
        return trialData.getAllData();
    }
    #endregion
}
