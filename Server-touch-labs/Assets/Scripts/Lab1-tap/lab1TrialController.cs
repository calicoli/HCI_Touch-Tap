using System;
using UnityEngine;
using static lab1Factors;

public class lab1TrialController : MonoBehaviour
{
    public lab1PhaseController phaseController;
    public lab1Target1Controller target1Controller;
    public lab1UIController uiController;

    private ServerController sender;
    private bool isConnecting;
    private bool inExperiment;

    private int repeateTimes, totalTrialsPerRepeatition;
    private bool inProtraitBlock;
    private PublicLabFactors.Lab1_tap_99.Posture blockPosture;

    private int curRepeateTime, curTrialIndex;
    private TrialPhase prevTrialPhase, curTrialPhase;

    private bool clientSaidMoveon;
    private bool haveObjectOnScreen;

    private TrialSequence[] trialSequences;
    private TrialSequence curSequence;
    private Trial curTrial;

    // Start is called before the first frame update
    void Start()
    {
        isConnecting = false;
        inExperiment = false;
        
        //init params with GloabalController
        sender = GlobalController.Instance.server;
        totalTrialsPerRepeatition = GlobalController.Instance.curLabInfos.totalTrialCount;
        repeateTimes = PublicLabFactors.Lab1_tap_99.repetitionCount;
        inProtraitBlock = (GlobalController.Instance.curLab1BlockCondition.getOrientation()
            == PublicLabFactors.Lab1_tap_99.Orientation.protrait);
        blockPosture = GlobalController.Instance.curLab1BlockCondition.getPosture();
        curRepeateTime = 0;
        curTrialIndex = PublicLabFactors.trial_start_index;

        prevTrialPhase = TrialPhase.block_end;
        curTrialPhase = TrialPhase.block_start;
        clientSaidMoveon = false;
        haveObjectOnScreen = false;

        trialSequences = new TrialSequence[repeateTimes+1];
        curSequence = new TrialSequence();
    }

    // Update is called once per frame
    void Update()
    {
        if(isConnecting & inExperiment)
        {
            if(prevTrialPhase != curTrialPhase)
            {
                Debug.Log("TrialPhase: " + prevTrialPhase + "->" + curTrialPhase);
                prevTrialPhase = curTrialPhase;
                GlobalController.Instance.curLab1TrialPhase = curTrialPhase;
            }

            if(curTrialPhase == TrialPhase.block_start)
            {
                curTrialPhase = TrialPhase.repeatition_start;
            }
            else if (curTrialPhase == TrialPhase.repeatition_start)
            {
                if (curRepeateTime < repeateTimes)
                {
                    curRepeateTime++;
                    GlobalController.Instance.curLab1Repeateid = curRepeateTime;
                    curTrialPhase = TrialPhase.repeatition_scheduling;
                }
                else
                {
                    curTrialPhase = TrialPhase.block_end;
                }
            }
            else if (curTrialPhase == TrialPhase.repeatition_scheduling)
            {
                target1Controller.resetAllTargets1();

                int bid = GlobalController.Instance.curBlockid;
                int rid = curRepeateTime;
                string prefix = string.Format("B{0:D2}-R{1:D2}", bid, rid);

                curSequence.setTrialLength(PublicLabFactors.LabScene.Lab1_tap_33_33);
                curSequence.setPrefix(prefix);
                curSequence.setAllQuence(blockPosture);
                trialSequences[curRepeateTime] = curSequence;
                GlobalController.Instance.curLab1TrialSequence = curSequence;
                GlobalController.Instance.writeCurrentRepeatIndexTrialSequenceToFile();

                curTrialIndex = PublicLabFactors.trial_start_index-1;
                curTrialPhase = TrialPhase.a_trial_set_params;
            }
            else if (curTrialPhase == TrialPhase.a_trial_set_params)
            {
                // trial index
                curTrialIndex++;
                GlobalController.Instance.curLab1Trialid = curTrialIndex;

                // set trial
                curTrial.setParams(curTrialIndex,
                    curSequence.seqTarget1[curTrialIndex - 1],
                    curSequence.seqTarget2[curTrialIndex - 1]);
                curTrial.printParams();
                GlobalController.Instance.curLab1Trial = curTrial;

                // set trial data
                GlobalController.Instance.curLab1TrialData = new TrialDataWithLocalTime();
                GlobalController.Instance.curLab1TrialData.init(curTrial.index, curTrial.firstid, curTrial.secondid);
                int bid = GlobalController.Instance.curBlockid;
                int rid = curRepeateTime;
                int tid = curTrialIndex;
                string prefix = string.Format("B{0:D2}-R{1:D2}-T{2:D2}", bid, rid, tid);
                GlobalController.Instance.curLab1TrialData.setPrefix(prefix);

                // test mode show params
                uiController.setTrialInfo(prefix, curTrial.firstid, curTrial.secondid);

                // move to next phase
                curTrialPhase = TrialPhase.a_trial_ready;
            }
            else if (curTrialPhase == TrialPhase.a_trial_ready)
            {
                if(!haveObjectOnScreen)
                { 
                    target1Controller.updateTarget1OnScreen(curTrial.firstid);
                    GlobalController.Instance.curLab1TrialData.localTime.t1ShowupStamp = CurrentTimeMillis();
                    haveObjectOnScreen = true;
                }
#if UNITY_ANDROID && UNITY_EDITOR
                if (Input.GetMouseButtonUp(0))
                {
                    GlobalController.Instance.curLab1TrialData.tp1Count++;
                    bool touchSuccess = process1Touch4Target1(Input.mousePosition, curTrial.firstid);
                    if (touchSuccess)
                    {
                        GlobalController.Instance.curLab1TrialData.localTime.tp1SuccessStamp = CurrentTimeMillis();
                        GlobalController.Instance.curLab1TrialData.tp1SuccessPosition = Input.mousePosition;
                        target1Controller.updateTarget1TouchedStatus(curTrial.firstid);
                        haveObjectOnScreen = false;
                        curTrialPhase = TrialPhase.a_trial_ongoing_p1;
                    }
                    else
                    {
                        GlobalController.Instance.curLab1TrialData.tp1FailPositions.Add(Input.mousePosition);
                    }

                }

#elif UNITY_IOS || UNITY_ANDROID
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Ended)
                    {
                        GlobalController.Instance.curLab1TrialData.tp1Count++;
                        bool touchSuccess = process1Touch4Target1(touch.position, curTrial.firstid);
                        if (touchSuccess)
                        {
                            GlobalController.Instance.curLab1TrialData.localTime.tp1SuccessStamp = CurrentTimeMillis();
                            GlobalController.Instance.curLab1TrialData.tp1SuccessPosition = touch.position;
                            target1Controller.updateTarget1TouchedStatus(curTrial.firstid);
                            haveObjectOnScreen = false;
                            curTrialPhase = TrialPhase.a_trial_ongoing_p1;
                        }
                        else
                        {
                            GlobalController.Instance.curLab1TrialData.tp1FailPositions.Add(touch.position);
                        }
                    }
                }
#endif
            }
            else if(curTrialPhase == TrialPhase.a_trial_ongoing_p1)
            {
                // send command to client to display target-2
                GlobalController.Instance.curLab1TrialData.localTime.serverSendDataStamp = CurrentTimeMillis();
                sender.prepareNewMessage4Client(PublicLabFactors.MessageType.Trial);
                curTrialPhase = TrialPhase.a_trial_ongoing_p2;
            }
            else if(curTrialPhase == TrialPhase.a_trial_ongoing_p2)
            {
                clientSaidMoveon = GlobalController.Instance.clientRefreshedTrialData;
                if(isConnecting && clientSaidMoveon)
                {
                    //GlobalController.Instance.curLab1TrialData.localTime.serverReceiveDataStamp = CurrentTimeMillis();
                    clientSaidMoveon = false;
                    GlobalController.Instance.clientRefreshedTrialData = false;
                    curTrialPhase = TrialPhase.a_trial_ongoing_p3;
                }
            }
            else if (curTrialPhase == TrialPhase.a_trial_ongoing_p3)
            {
                curTrialPhase = TrialPhase.a_trial_output_data;
            }
            else if (curTrialPhase == TrialPhase.a_trial_output_data)
            {
                bool writeFinished = false;
                GlobalController.Instance.writeCurrentTrialDataToFile(out writeFinished);
                if(writeFinished)
                {
                    curTrialPhase = TrialPhase.a_trial_end;
                }
            }
            else if (curTrialPhase == TrialPhase.a_trial_end)
            {
                if(curTrialIndex == totalTrialsPerRepeatition)
                {
                    curTrialPhase = TrialPhase.repeatition_start;
                } else
                {
                    curTrialPhase = TrialPhase.a_trial_set_params;
                }
            }
            else if (curTrialPhase == TrialPhase.block_end)
            {
                sender.prepareNewMessage4Client(PublicLabFactors.MessageType.Trial);
                phaseController.moveToPhase(Lab1Phase.end_experiment);
            }
            else
            {
                Debug.LogError("Something bad happened.");
            }
        }
    }

    private bool process1Touch4Target1(Vector2 pos, int targetid)
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

        if (hitid == targetid)
            return true;
        else
            return false;
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
    public void setExperimentStatus(bool working)
    {
        inExperiment = working;
    }

    public void setConnectionStatus(bool con)
    {
        if(con != isConnecting)
        {
            isConnecting = con;
        }
    }


    #endregion
}
