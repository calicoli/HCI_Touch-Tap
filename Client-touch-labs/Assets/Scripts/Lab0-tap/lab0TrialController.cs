using System;
using UnityEngine;
using static lab0Factors;

public class lab0TrialController : MonoBehaviour
{
    public lab0PhaseController phaseController;
    public lab0Target2Controller target2Controller;

    private ClientController sender;
    private bool isConnecting;
    private bool inExperiment;

    private int curRepeateTime, curTrialIndex;
    private TrialPhase curTrialPhase, prevTrialPhase;

    private bool serverRefreshed;
    private bool haveObjectOnScreen;

    private Trial curTrial;
    private TrialDataWithLocalTime trialData;

    // Start is called before the first frame update
    void Start()
    {
        isConnecting = false;
        inExperiment = false;

        // init params with GlobalController
        sender = GlobalController.Instance.client;

        curRepeateTime = 0;
        curTrialIndex = PublicLabFactors.trial_start_index;

        serverRefreshed = false;
        haveObjectOnScreen = false;
        prevTrialPhase = TrialPhase.block_end;
        curTrialPhase = TrialPhase.a_trial_ready;

        curTrial = new Trial();
        GlobalController.Instance.curLab0Trial = new lab0Factors.Trial();
    }

    // Update is called once per frame
    void Update()
    {
        serverRefreshed = GlobalController.Instance.serverRefreshTrialData;
        if (isConnecting & inExperiment)
        {
            if (prevTrialPhase != curTrialPhase)
            {
                Debug.Log("TrialPhase: " + prevTrialPhase + "->" + curTrialPhase);
                prevTrialPhase = curTrialPhase;
                GlobalController.Instance.curLab0TrialPhase = curTrialPhase;
            }

            if(serverRefreshed)
            {
                curTrialPhase = GlobalController.Instance.curLab0TrialPhase;
                if(curTrialPhase == TrialPhase.a_trial_ongoing_p1)
                {
                    long tmpTime = CurrentTimeMillis();
                    curRepeateTime = GlobalController.Instance.curLab0Repeateid;
                    curTrialIndex = GlobalController.Instance.curLab0Trialid;
                    curTrial = GlobalController.Instance.curLab0Trial;
                    trialData = new TrialDataWithLocalTime();
                    trialData.init(curTrial.index, curTrial.secondid);
                    trialData.localTime.clientReceiveDataStamp = tmpTime;
                } else if (curTrialPhase == TrialPhase.block_end)
                {
                    // wait
                }
                GlobalController.Instance.serverRefreshTrialData = false;
            }

            if (curTrialPhase == TrialPhase.a_trial_ready)
            {

            }
            else if(curTrialPhase == TrialPhase.a_trial_ongoing_p1)
            {
                if (!haveObjectOnScreen)
                {
                    target2Controller.updateTarget2OnScreen(curTrial.secondid);
                    trialData.localTime.t2ShowupStamp = CurrentTimeMillis();
                    haveObjectOnScreen = true;
                }
#if UNITY_ANDROID && UNITY_EDITOR
                if (Input.GetMouseButtonUp(0))
                {
                    trialData.tp2Count++;
                    bool touchSuccess = process1Touch4Target2(Input.mousePosition, curTrial.secondid);
                    if (touchSuccess)
                    {
                        trialData.localTime.tp2SuccessStamp = CurrentTimeMillis();
                        trialData.tp2SuccessPosition = Input.mousePosition;
                        target2Controller.updateTarget2TouchedStatus(curTrial.secondid);
                        haveObjectOnScreen = false;
                        curTrialPhase = TrialPhase.a_trial_ongoing_p2;
                    }
                    else
                    {
                        trialData.tp2FailPositions.Add(Input.mousePosition);
                    }

                }

#elif UNITY_IOS || UNITY_ANDROID
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Ended)
                    {
                        trialData.tp2Count++;
                        bool touchSuccess = process1Touch4Target2(touch.position, curTrial.secondid);
                        if (touchSuccess)
                        {
                            trialData.localTime.tp2SuccessStamp = CurrentTimeMillis();
                            trialData.tp2SuccessPosition = touch.position;
                            target2Controller.updateTarget2TouchedStatus(curTrial.secondid);
                            haveObjectOnScreen = false;
                            curTrialPhase = TrialPhase.a_trial_ongoing_p2;
                        }
                        else
                        {
                            trialData.tp2FailPositions.Add(touch.position);
                        }
                    }
                }
#endif
            }
            else if (curTrialPhase == TrialPhase.a_trial_ongoing_p2)
            {
                trialData.localTime.clientSendDataStamp = CurrentTimeMillis();
                GlobalController.Instance.curLab0TrialData = trialData;
                sender.prepareNewMessage4Server(PublicLabFactors.MessageType.Trial);
                curTrialPhase = TrialPhase.a_trial_ongoing_p3;
            }
            else if (curTrialPhase == TrialPhase.a_trial_ongoing_p3 ||
                     curTrialPhase == TrialPhase.a_trial_output_data)
            {
                // server is rensponsible for this phase
                // client just go on
            }
            else if (curTrialPhase == TrialPhase.block_end)
            {
                phaseController.moveToPhase(Lab0Phase.end_experiment);
            }
            else
            {
                Debug.LogError("Something bad happened: ");
            }
        }
    }

    private bool process1Touch4Target2(Vector2 pos, int targetid)
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
        if (con != isConnecting)
        {
            isConnecting = con;
        }
    }
    #endregion
}
