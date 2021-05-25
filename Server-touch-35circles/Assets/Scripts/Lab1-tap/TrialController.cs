using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialController : MonoBehaviour
{
    public GameObject serverController;
    public GameObject fileProcessor;
    public GameObject targetScheduler;

    public GameObject backBtn;

    [HideInInspector]
    public bool isConnecting;
    public bool isPortrait;

    public enum TrialPhase
    {
        inactive = 0,
        ready = 1,
        ongoing_p1 = 21,
        ongoing_p2 = 22,
        ongoing_p3 = 23,
        end = 3
    }

    public struct TrialData
    {
        // assign
        int trialid;
        int firstid, secondid;
        // raw
        long t1ShowupStamp, t1VanishStamp, t2ShowupStamp, t2VanishStamp;
        int tp1Count, tpCount;
        long tp0SuccessStamp, tp1SuccessStamp, tp2SccessStamp;
        Vector2 tp1CorrectPosition, tp2CorrectPosition;
        Vector2[] tp1WrongPositions, tp2WrongPositions;
        // calculate
        bool isTrialSuccessWithNoError;
        bool isTarget1SuccessWithNoError, isTarget2SuccessWithNoError;
        long completeTime;
        long target1CompleteTime, target2CompleteTime;
        long target1ShowTime, target2ShowTime;

        long calTimeSpan(long earlier, long later)
        {
            // DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            return later - earlier;
        }
    }

    const string curLabName = "Lab1-tap";
    const int totalTrialTimes = 25;
    private int curTrialIndex;
    private TrialPhase prevTrialPhase = TrialPhase.end;
    private TrialPhase curTrialPhase;
    private bool clientSaidMoveon;
    private bool haveObjectOnScreen;

    // Start is called before the first frame update
    void Start()
    {
        backBtn.SetActive(false);

        isConnecting = false;
        clientSaidMoveon = false;
        haveObjectOnScreen = false;

        curTrialIndex = 1;
        curTrialPhase = TrialPhase.inactive;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(prevTrialPhase != curTrialPhase)
        {
            Debug.Log("Phase: " + prevTrialPhase + "->" + curTrialPhase);
            prevTrialPhase = curTrialPhase;
        }

        if(curTrialPhase == TrialPhase.inactive)
        {
            if(!haveObjectOnScreen)
            {
                targetScheduler.GetComponent<TargetScheduler>().updateStartBtn(true);
                haveObjectOnScreen = true;
            }
            
            bool moveToNextPhase = false;
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    moveToNextPhase = process1Touch4StartBtn(touch.position);
                    if(moveToNextPhase)
                    {
                        targetScheduler.GetComponent<TargetScheduler>().updateStartBtn(false);
                        haveObjectOnScreen = false;
                        curTrialPhase = TrialPhase.ready;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                moveToNextPhase = process1Touch4StartBtn(Input.mousePosition);
                if (moveToNextPhase)
                {
                    targetScheduler.GetComponent<TargetScheduler>().updateStartBtn(false);
                    haveObjectOnScreen = false;
                    curTrialPhase = TrialPhase.ready;
                }
            }

        }
        else if (curTrialPhase == TrialPhase.ready)
        {
            if (!haveObjectOnScreen)
            {
                targetScheduler.GetComponent<TargetScheduler>().updateTarget1(true);
                haveObjectOnScreen = true;
            }
                
            //bool moveToNextPhase; // depend on the task completive condition
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Ended)
                {
                    bool touchSuccess = process1Touch4Target1(touch.position);
                    if(touchSuccess)
                    {
                        targetScheduler.GetComponent<TargetScheduler>().updateTarget1(false);
                        haveObjectOnScreen = false;
                        curTrialPhase = TrialPhase.ongoing_p1;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                bool touchSuccess = process1Touch4Target1(Input.mousePosition);
                if(touchSuccess)
                {
                    targetScheduler.GetComponent<TargetScheduler>().updateTarget1(false);
                    haveObjectOnScreen = false;
                    curTrialPhase = TrialPhase.ongoing_p1;
                }
                
            }

        }
        else if (curTrialPhase == TrialPhase.ongoing_p1)
        {
            if(isConnecting)
            {
                serverController.GetComponent<ServerController>().prepareNewMessage4Client(false, true);
                
                curTrialPhase = TrialPhase.ongoing_p2;

            } else
            {
                curTrialPhase = TrialPhase.ongoing_p2;
            }
        }
        else if (curTrialPhase == TrialPhase.ongoing_p2)
        {
            bool moveToNextPhase = clientSaidMoveon;
            if(isConnecting && moveToNextPhase)
            {
                clientSaidMoveon = false;
                curTrialPhase = TrialPhase.ongoing_p3;
            }
            else if(!isConnecting)
            {
                curTrialPhase = TrialPhase.ongoing_p3;
            }
        }
        else if (curTrialPhase == TrialPhase.ongoing_p3)
        {
            if (curTrialIndex + 1 < totalTrialTimes)
            {
                curTrialIndex++;
                curTrialPhase = TrialPhase.inactive;
                targetScheduler.GetComponent<TargetScheduler>().increaseTrialIndex();
            }
            else
            {
                curTrialPhase = TrialPhase.end;
            }
        }
        else if(curTrialPhase == TrialPhase.end)
        {
            backBtn.SetActive(true);
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
            if(hit.collider.gameObject.name == "StartBtn-square")
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

        int targetid = targetScheduler.GetComponent<TargetScheduler>().getCurrentTarget1id();
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

    long AnthoerWayToGetCurrentTimeMillis(DateTime date)
    {
        return(long)(DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    // check if server need to send message to client again
    public bool checkClientTargetTouch( string clientLabName,
        int cTrialIndex, int cTrialPhase, int cTarget2id, 
        bool cPhaseFinished,bool cPhaseSuccess)
    {
        int curTarget2id = targetScheduler.GetComponent<TargetScheduler>().getCurrentTarget2id();
        if(string.Equals(clientLabName, curLabName) 
            && cTrialIndex == curTrialIndex && (cTrialPhase == (int)curTrialPhase) 
            && cTarget2id == curTarget2id)
        {
            clientSaidMoveon = cPhaseFinished;
            // deal with cPhaseSuccess
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
        sTarget2id = targetScheduler.GetComponent<TargetScheduler>().getCurrentTarget2id();
    }

}
