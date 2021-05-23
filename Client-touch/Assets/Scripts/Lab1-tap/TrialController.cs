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
        inactive = 0,
        ready = 1,
        ongoing_p1 = 21,
        ongoing_p2 = 22,
        ongoing_p3 = 23,
        end = 3
    }
    private TrialPhase prevTrialPhase = TrialPhase.end;
    private TrialPhase curTrialPhase;

    bool serverRefreshData;
    bool clientTouchSuccess;

    void Start()
    {
        inSameLabWithServer = false;
        isConnecting = false;
        serverRefreshData = false;
        clientTouchSuccess = false;
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

            if (curTrialPhase == TrialPhase.inactive || curTrialPhase == TrialPhase.ready)
            {
                // server is rensponsible for this phase
                // client just go on
            }
            else if (curTrialPhase == TrialPhase.ongoing_p1)
            { 
                if(serverRefreshData)
                {
                    
                    targetScheduler.GetComponent<TargetScheduler>().updateTarget2(true);
                    //bool moveToNextPhase; // depend on the task completive condition
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Ended)
                        {
                            bool touchSuccess = process1Touch4Target2(touch.position);
                            targetScheduler.GetComponent<TargetScheduler>().updateTarget2(false);
                            curTrialPhase = TrialPhase.ongoing_p2;
                            serverRefreshData = false;
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        bool touchSuccess = process1Touch4Target2(Input.mousePosition);
                        clientTouchSuccess = touchSuccess;
                        targetScheduler.GetComponent<TargetScheduler>().updateTarget2(false);
                        curTrialPhase = TrialPhase.ongoing_p2;
                        serverRefreshData = false;
                    }

                }

            }
            else if (curTrialPhase == TrialPhase.ongoing_p2)
            {
                clientController.GetComponent<ClientController>().prepareNewMessage4Server(false, true);
                curTrialPhase = TrialPhase.ongoing_p3;
            }
            else if (curTrialPhase == TrialPhase.ongoing_p3 || curTrialPhase == TrialPhase.end)
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
        out bool cPhaseFinished, out bool cPhaseSuccess)
    {

        cLabName = curLabName;
        cTrialIndex = curTrialIndex;
        cTrialPhase = (int)curTrialPhase;
        cTarget2id = targetScheduler.GetComponent<TargetScheduler>().getCurrentTarget2id();
        cPhaseFinished = true;
        cPhaseSuccess = clientTouchSuccess;
    }
}
