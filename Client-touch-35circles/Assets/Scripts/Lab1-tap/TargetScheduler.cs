using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScheduler : MonoBehaviour
{

    public GameObject targets2;
    //public GameObject startBtn;

    const int repeatTime = 1;
    const int columnCnt= 5, rowCnt = 7;
    const int target2Cnt = 35;

    struct Trial
    {
        public int index;
        public int secondid;

        public void setParams(int idx, int id2)
        {
            index = idx;
            secondid = id2;
        }

        public void printParams()
        {
            Debug.Log("no." + index + " id2: " + secondid);
        }
    }

    enum TargetStatus
    {
        NORMAL, CORRECT, WRONG,
    }
    struct Target
    {
        public int id;
        public int remainTouchCnt;
        public bool visible;
        public TargetStatus status;

        public Target(int idx, int cnt, bool vis, TargetStatus st)
        {
            id = idx;
            remainTouchCnt = cnt;
            visible = vis;
            status = st;
        }
    }

    int curTrialIndex;
    Trial[] trials = new Trial[100 + 1];     // trial[0] is empty
    int totalCubes2;
    Target[] cubes2 = new Target[50];
    ArrayList arrayRemainTargets2 = new ArrayList();
    //Vector3[] posCubes2 = new Vector3[50];

    // Start is called before the first frame update
    void Start()
    {
        curTrialIndex = 1;

        // target2 set
        arrayRemainTargets2.Clear();
        totalCubes2 = target2Cnt;
        for (int i = 0; i < totalCubes2; i++)
        {
            GameObject child = targets2.transform.GetChild(i).gameObject;
            if (child.name.Length == 11)
            {
                int id2 = Convert.ToInt32(child.name.Substring(9, 2));
                cubes2[id2] = new Target(id2, repeatTime, false, TargetStatus.NORMAL);
                arrayRemainTargets2.Add(id2);
                //posCubes2[id2] = child.transform.position;
            }
        }

        Debug.Log("cnt set 2: " + arrayRemainTargets2.Count);
        resetAllCubes2();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void resetAllCubes2()
    {
        for (int id2 = 0; id2 < totalCubes2; id2++)
        {
            cubes2[id2].visible = false;
            cubes2[id2].status = TargetStatus.NORMAL;
            targets2.transform.GetChild(id2).gameObject.SetActive(cubes2[id2].visible);
        }
    }

    public void scheduleTargetsWithServerData(int sTrialIndex, int sTarget2id)
    {
        curTrialIndex = sTrialIndex;
        updateTarget2Array(sTarget2id);
        trials[curTrialIndex].setParams(sTrialIndex, sTarget2id);
        trials[curTrialIndex].printParams();
    }

    void updateTarget2Array(int id2)
    {
        cubes2[id2].remainTouchCnt--;
        if (cubes2[id2].remainTouchCnt <= 0)
        {
            arrayRemainTargets2.Remove(id2);
        }
        Debug.Log("arrayCount2/id/cntTouch: " 
            + arrayRemainTargets2.Count + " / " 
            + id2 + " / " 
            + cubes2[id2].remainTouchCnt);
    }

    public void updateTarget2(bool isActive)
    {
        int secondid = trials[curTrialIndex].secondid;
        cubes2[secondid].visible = isActive;
        targets2.transform.GetChild(secondid).gameObject.SetActive(cubes2[secondid].visible);
    }

    public int getCurrentTarget2id()
    {
        return trials[curTrialIndex].secondid;
    }
}
