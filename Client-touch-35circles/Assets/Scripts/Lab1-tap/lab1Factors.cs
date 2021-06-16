using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lab1Factors : MonoBehaviour
{
    public enum Lab1Phase
    {
        in_lab1_scene = 0,
        check_connection = 1,
        check_client_scene = 2,
        in_experiment = 3,
        end_experiment = 4,
        wait_to_back_to_entry = 5,
        out_lab1_scene = 6,
    }

    public enum TrialPhase
    {
        block_start = 0,
        repeatition_start = 1,
        repeatition_scheduling = 2,
        a_trial_set_params = 3,
        a_trial_ready = 4,
        a_trial_ongoing_p1 = 51,
        a_trial_ongoing_p2 = 52,
        a_trial_ongoing_p3 = 53,
        a_trial_output_data = 6,
        a_trial_end = 7,
        block_end = 9
    }

    public struct Trial
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

    public struct Target
    {
        public int id;
        public bool touched;
        public bool visible;
        public Target(int idx, bool ted, bool vis)
        {
            id = idx;
            touched = ted;
            visible = vis;
        }
    }

    public struct LocalTime
    {
        public long t2ShowupStamp, tp2SuccessStamp;                 // client
        public long clientReceiveDataStamp, clientSendDataStamp;    // client
    }

    public struct TrialDataWithLocalTime
    {
        // assgin
        int trialid, secondid;
        public int tp2Count;
        
        public Vector2 tp2SuccessPosition;
        public ArrayList tp2FailPositions;
        public LocalTime localTime;

        public void init(int idx, int id2)
        {
            trialid = idx;
            secondid = id2;
            tp2Count = 0;
            tp2FailPositions = new ArrayList();
            tp2FailPositions.Clear();
            localTime = new LocalTime();
        }

        public string getAllData()
        {
            Debug.Log("Part data: " + tp2Count.ToString());
            string str;
            str = trialid.ToString() + "#" + secondid.ToString() + "#"
                + localTime.clientReceiveDataStamp + "#"
                + localTime.clientSendDataStamp + "#"
                + tp2Count.ToString() + "#" 
                + localTime.t2ShowupStamp.ToString() + "#"
                + localTime.tp2SuccessStamp.ToString() + "#"
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

    public struct TrialData
    {
        // assgin
        int trialid, secondid;
        public int tp2Count;
        public long t2ShowupStamp;
        public long tp2SuccessStamp;
        public Vector2 tp2SuccessPosition;
        public ArrayList tp2FailPositions;
        public long clientReceivedDataStamp, clientSendDataStamp;

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
                + clientSendDataStamp + "#"
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

}
