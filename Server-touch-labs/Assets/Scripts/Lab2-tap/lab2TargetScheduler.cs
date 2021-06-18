using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lab2TargetScheduler : MonoBehaviour
{

    const int repeatTime = 35;
    const int columnCnt= 5, rowCnt = 7;
    const int target1Cnt = 35, target2Cnt = 35;

    struct Trial
    {
        public int index;
        public int startid;
        public int firstid, secondid;
        public int prevFirstid, prevSecondid;

        public void setParams(int idx, int ids, int id1, int id2)
        {
            prevFirstid = firstid;
            prevSecondid = secondid;
            index = idx;
            startid = ids;
            firstid = id1;
            secondid = id2;
        }

        public void printParams()
        {
            Debug.Log("no." + index
                + " id0/1/2: " + startid + "/" + firstid + "/" + secondid
                + " prev1/2: " + prevFirstid + "/" + prevSecondid);
        }
    }

    public GameObject targets1;
    public GameObject startBtn;

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
    Trial[] trials = new Trial[100 + 1];    // trial[0] is empty
    int totalCubes1, totalCubes2;
    Target[] cubes1 = new Target[50];
    int[] cubes2 = new int[50];
    ArrayList arrayRemainTargets1 = new ArrayList(), 
              arrayRemainTargets2 = new ArrayList();
    Vector3[] posCubes1 = new Vector3[50];

    // Start is called before the first frame update
    void Start()
    {
        curTrialIndex = 0;

        // target1 set
        arrayRemainTargets1.Clear();
        totalCubes1 = target1Cnt;
        for (int i = 0; i < totalCubes1; i++)
        {
            GameObject child = targets1.transform.GetChild(i).gameObject;
            if (child.name.Length == 11)
            {
                int id1 = Convert.ToInt32(child.name.Substring(9, 2));
                cubes1[id1] = new Target(id1, repeatTime, false, TargetStatus.NORMAL);
                arrayRemainTargets1.Add(id1);
                posCubes1[id1] = child.transform.position;
            }
        }

        // target2 set
        arrayRemainTargets2.Clear();
        totalCubes2 = target2Cnt;
        for(int id2 = 0; id2 < totalCubes2; id2++)
        {
            cubes2[id2] = repeatTime;
            arrayRemainTargets2.Add(id2);
        }

        Debug.Log("cnt 1/2: " + arrayRemainTargets1.Count + "/" + arrayRemainTargets2.Count);
        resetAllCubes1();
        //resetAllCube2(); do this in client
        // scheduleTargets(); let TrialController to control targets-scheduler
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void resetAllCubes1()
    {
        for (int id1 = 0; id1 < totalCubes1; id1++)
        {
            cubes1[id1].visible = false;
            cubes1[id1].status = TargetStatus.NORMAL;
            targets1.transform.GetChild(id1).gameObject.SetActive(cubes1[id1].visible);
        }
    }

    void scheduleTargets(bool isPortrait)
    {
        int idx = curTrialIndex;
        /*
         * int id1 = randomTargetId(1);
         * int id2 = randomTargetId(2);
         */
        int id1, id2;
        randomTargetId(out id1, out id2);
        int ids = 0;
#if UNITY_ANDROID && UNITY_EDITOR
        ids = columnCnt * (rowCnt - 1);
#endif
#if UNITY_IOS || UNITY_ANDROID
        if (isPortrait)
        {
            ids = columnCnt * (rowCnt - 1);
        }
        else
        {
            ids = rowCnt * (columnCnt - 1);
        }
#endif
        trials[curTrialIndex].setParams(idx, ids, id1, id2);
        trials[curTrialIndex].printParams();
    }

    void randomTargetId(out int id1, out int id2)
    {
        id1 = -2;
        id2 = -2;
        System.Random rd = new System.Random();
        if (arrayRemainTargets1.Count > 0)
        {
            int rdnum = rd.Next(0, arrayRemainTargets1.Count);
            id1 = Convert.ToInt32(arrayRemainTargets1[rdnum]);
            cubes1[id1].remainTouchCnt--;
            if (cubes1[id1].remainTouchCnt <= 0)
            {
                arrayRemainTargets1.Remove(id1);
            }
            Debug.Log("arrayCount1/rdnum/id/cntTouch: " + arrayRemainTargets1.Count + " / " + rdnum + " / " + id1 + " / " + cubes1[id1].remainTouchCnt);
        }

        if (arrayRemainTargets2.Count > 0)
        {
            int rdnum = rd.Next(0, arrayRemainTargets2.Count);
            id2 = Convert.ToInt32(arrayRemainTargets2[rdnum]);
            cubes2[id2]--;
            if (cubes2[id2] <= 0)
            {
                arrayRemainTargets2.Remove(id2);
            }
            Debug.Log("arrayCount2/rdnum/id/cntTouch: " + arrayRemainTargets2.Count + " / " + rdnum + " / " + id2 + " / " + cubes2[id2]);
        }
    }

    int randomTargetId(int num)
    {
        //Debug.Assert(num == 1 || num == 2);
        if (num == 1)
        {
            if (arrayRemainTargets1.Count > 0)
            {
                System.Random rd = new System.Random();
                int rdnum = rd.Next(0, arrayRemainTargets1.Count);
                int id1 = Convert.ToInt32(arrayRemainTargets1[rdnum]);
                cubes1[id1].remainTouchCnt--;
                if (cubes1[id1].remainTouchCnt <= 0)
                {
                    arrayRemainTargets1.Remove(id1);
                }
                Debug.Log("arrayCount1/rdnum/id/cntTouch: " + arrayRemainTargets1.Count + " / " + rdnum + " / " + id1 + " / " + cubes1[id1].remainTouchCnt);
                return id1;
            }

        }
        else if(num == 2)
        {
            if (arrayRemainTargets2.Count > 0)
            {
                System.Random rd = new System.Random();
                int rdnum = rd.Next(0, arrayRemainTargets2.Count);
                int id2 = Convert.ToInt32(arrayRemainTargets2[rdnum]);
                if (cubes2[id2] <= 0)
                {
                    arrayRemainTargets2.Remove(id2);
                }
                Debug.Log("arrayCount2/rdnum/id/cntTouch: " + arrayRemainTargets2.Count + " / " + rdnum + " / " + id2 + " / " + cubes2[id2]);
                return id2;
            }
        }
        return -2;
    }
    #region public method
    public void updateStartBtn(bool isActive)
    {
        if(isActive)
        {
            int startid = trials[curTrialIndex].startid;
            //startBtn.transform.position = targets1.transform.GetChild(startid).position;
            startBtn.transform.position = posCubes1[startid];
        }
        startBtn.SetActive(isActive);
    }

    public void updateTarget1(bool isActive)
    {
        int firstid = trials[curTrialIndex].firstid;
        cubes1[firstid].visible = isActive;
        targets1.transform.GetChild(firstid).gameObject.SetActive(cubes1[firstid].visible);
    }

    public void increaseTrialIndex()
    {
        curTrialIndex++;
        
    }
    public void scheduleNewTrial(bool isPortrait)
    {
        scheduleTargets(isPortrait);
    }

    public int getCurrentTarget1id()
    {
        return trials[curTrialIndex].firstid;
    }

    public int getCurrentTarget2id()
    {
        return trials[curTrialIndex].secondid;
    }
    #endregion
}
