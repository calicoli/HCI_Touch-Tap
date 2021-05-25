using System;
using System.Collections;
using UnityEngine;

public class stTargetController : MonoBehaviour
{
    
    public Material selectNormal;
    public Material selectCorrect;
    public Material selectWrong;
    public Material selectTarget;

    public GameObject back;

    public GameObject targets;

    enum TargetPhase {
        NORMAL, CORRECT, WRONG, //SELECT,
    }

    struct Target
    {
        public int id;
        public int cntTouch;
        public bool visible;
        public TargetPhase phase;

        public Target(int idx, int cnt, bool vis, TargetPhase ph)
        {
            id = idx;
            cntTouch = cnt;
            visible = vis;
            phase = ph;
        }
    }

    Target[] cubes = new Target[30];
    int curTargetid;
    ArrayList arrayRemainTarget = new ArrayList();

    const int repeatTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        arrayRemainTarget.Clear();
        for(int i = 0; i < targets.transform.childCount; i++)
        {
            GameObject child = targets.transform.GetChild(i).gameObject;
            if (child.name.Length == 11)
            {
                int childid = Convert.ToInt32(child.name.Substring(9, 2));
                cubes[childid] = new Target(childid, repeatTime, false, TargetPhase.NORMAL);
                arrayRemainTarget.Add(childid);
            }
        }

        resetAllCubes();
        scheduleFirstTartget();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int randomTargetId()
    {
        if(arrayRemainTarget.Count > 0)
        {
            System.Random rd = new System.Random();
            int rdnum = rd.Next(0, arrayRemainTarget.Count);
            int id = Convert.ToInt32(arrayRemainTarget[rdnum]);
            //Debug.Log("arrayCount/rdnum/id/cntTouch: " + arrayRemainTarget.Count + " / " + rdnum + " / " + id + " / ");
            cubes[id].cntTouch--;
            if (cubes[id].cntTouch <= 0)
            {
                arrayRemainTarget.Remove(id);
                //arrayRemainTarget.RemoveAt(rdnum);
            }
            //Debug.Log("arrayCount/rdnum/id/cntTouch: " + arrayRemainTarget.Count + " / " + rdnum + " / " + id + " / " + cubes[id].cntTouch);
            return id;
        } else
        {
            return -2;
        }
        
    }

    void scheduleFirstTartget()
    {
        curTargetid = randomTargetId();
        Debug.Log("Cureent target: " + curTargetid + " / " + arrayRemainTarget.Count);
        cubes[curTargetid].visible = true;
        //updateCubeVisibility(curTargetid);
        targets.transform.GetChild(curTargetid).GetComponent<MeshRenderer>().material = selectTarget;
    }

    public void scheduleNextTarget(int touchedid, bool curCorrect)
    {
        if(curCorrect)
        {
            cubes[touchedid].phase = TargetPhase.CORRECT;
            updateCubeColor(touchedid, TargetPhase.CORRECT);
        } else
        {
            cubes[touchedid].phase = TargetPhase.WRONG;
            updateCubeColor(touchedid, TargetPhase.WRONG);
        }
        cubes[touchedid].visible = false;
        //updateCubeVisibility(touchedid);


        if(arrayRemainTarget.Count > 0)
        {
            curTargetid = randomTargetId();
            Debug.Log("Cureent target: " + curTargetid + " / " + arrayRemainTarget.Count);

            cubes[curTargetid].phase = TargetPhase.NORMAL;
            updateCubeColor(curTargetid, TargetPhase.NORMAL);
            cubes[curTargetid].visible = true;

            //updateCubeVisibility(curTargetid);
            targets.transform.GetChild(curTargetid).GetComponent<MeshRenderer>().material = selectTarget;
        } else
        {
            back.SetActive(true);
        }
    }

    public int getCurrentTargetId()
    {
        return curTargetid;
    }

    void resetAllCubes()
    {
        for(int i = 0; i < 25; i++)
        {
            cubes[i].visible = false;
            cubes[i].phase = TargetPhase.NORMAL;
            updateCubeColor(i, TargetPhase.NORMAL);
            //updateCubeVisibility(i);
        }
    }

    void updateCubeVisibility(int id)
    {
        targets.transform.GetChild(id).GetComponent<MeshRenderer>().enabled = cubes[id].visible;
    }

    void updateCubeColor(int id, TargetPhase phase)
    {
        switch (phase)
        {
            case TargetPhase.NORMAL:
                targets.transform.GetChild(id).GetComponent<MeshRenderer>().material = selectNormal;
                break;
            case TargetPhase.CORRECT:
                targets.transform.GetChild(id).GetComponent<MeshRenderer>().material = selectCorrect;
                break;
            case TargetPhase.WRONG:
                targets.transform.GetChild(id).GetComponent<MeshRenderer>().material = selectWrong;
                break;
            default:
                break;

        }

    }
}
