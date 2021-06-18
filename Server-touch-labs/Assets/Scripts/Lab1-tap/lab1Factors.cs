using System;
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

    public struct InternetTime
    {
        public long t1ShowupStamp, t2ShowupStamp;
        public long tp1SuccessStamp, tp2SuccessStamp;
        public long serverSendDataStamp, clientReceivedDataStamp;
        public long clientSendDataStamp, serverReceivedDataStamp;
    }

    public struct LocalTime
    {
        public long t1ShowupStamp, tp1SuccessStamp;     // server
        public long t2ShowupStamp, tp2SuccessStamp;     // client
        public long serverSendDataStamp, serverReceiveDataStamp;    // server
        public long clientReceiveDataStamp, clientSendDataStamp;    // client
    }

    public struct TrialDataWithLocalTime
    {
        // assign
        int trialid;
        int firstid, secondid;
        string prefix;
        // raw
        public int tp1Count, tp2Count;
        public Vector2 tp1SuccessPosition, tp2SuccessPosition;
        public ArrayList tp1FailPositions;
        public string    tp2FailPositions;
        //public InternetTime interTime;
        public LocalTime    localTime;
        // calculate
        public bool isTrialSuccessWithNoError;
        public bool isTarget1SuccessWithNoError, isTarget2SuccessWithNoError;
        public long loCompleteTime, loServerIntervalTime, loClientIntervalTime, loDataTransferForthBackTime;
        public long loTarget1CompleteTime; 
        public long loTarget2CompleteTime;  
        public long loTarget1ShowTime; // tp1SuccessStamp - t1ShowupStamp
        public long loTarget2ShowTime; // tp2SuccessStamp - t2ShowupStamp

        public void init(int idx, int id1, int id2)
        {
            trialid = idx;
            firstid = id1;
            secondid = id2;
            tp1Count = 0;
            tp2Count = 0;
            tp1FailPositions = new ArrayList(); tp1FailPositions.Clear();
            tp2FailPositions = null;
            localTime = new LocalTime();
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
            loCompleteTime = calTimeSpan(localTime.serverReceiveDataStamp, localTime.t1ShowupStamp);
            loServerIntervalTime = calTimeSpan(localTime.serverReceiveDataStamp, localTime.serverSendDataStamp);
            loClientIntervalTime = calTimeSpan(localTime.clientSendDataStamp, localTime.clientReceiveDataStamp);
            loDataTransferForthBackTime = calTimeSpan(loServerIntervalTime, loClientIntervalTime);
            //loTarget1CompleteTime = calTimeSpan(localTime.tp1SuccessStamp, localTime.t1ShowupStamp);
            //loTarget2CompleteTime = calTimeSpan(localTime.tp2SuccessStamp, localTime.clientReceivedDataStamp);
            loTarget1ShowTime = calTimeSpan(localTime.tp1SuccessStamp, localTime.t1ShowupStamp);
            loTarget2ShowTime = calTimeSpan(localTime.tp2SuccessStamp, localTime.t2ShowupStamp);
        }

        public void setPrefix(string pre)
        {
            prefix = pre;
        }

        public string getAllDataForFile()
        {
            calMoreData();
            string str;
            // assign
            str = prefix + ";"
                + trialid.ToString() + ";" + firstid.ToString() + ";" + secondid.ToString() + ";"
                // calculate
                + isTrialSuccessWithNoError.ToString() + ";"
                + isTarget1SuccessWithNoError.ToString() + ";" + isTarget2SuccessWithNoError.ToString() + ";"
                + loCompleteTime.ToString() + ";" + loDataTransferForthBackTime.ToString() + ";"
                + loServerIntervalTime.ToString() + ";" + loClientIntervalTime.ToString() + ";"
                //+ loTarget1CompleteTime.ToString() + ";" + loTarget2CompleteTime.ToString() + ";"
                + loTarget1ShowTime.ToString() + ";" + loTarget2ShowTime.ToString() + ";"
                // raw: success position
                + tp1SuccessPosition.ToString() + ";" + tp2SuccessPosition.ToString() + ";"
                // raw: other data
                + localTime.serverSendDataStamp.ToString() + ";" + localTime.clientReceiveDataStamp.ToString() + ";"
                + localTime.clientSendDataStamp.ToString() + ";" + localTime.serverReceiveDataStamp.ToString() + ";"
                + tp1Count.ToString() + ";"
                + localTime.t1ShowupStamp.ToString() + ";" + localTime.tp1SuccessStamp.ToString() + ";"
                + tp2Count.ToString() + ";"
                + localTime.t2ShowupStamp.ToString() + ";" + localTime.tp2SuccessStamp.ToString() + ";"
                ;
            if (tp1Count > 1)
            {
                for (int i = 0; i < tp1FailPositions.Count; i++)
                {
                    str += tp1FailPositions[i].ToString() + "*";
                }
            }
            else
            {
                str += "T1NoError";
            }
            str += ";";
            str += (tp2Count > 1) ? tp2FailPositions : "T2NoError";
            str += ";";
            return str;
        }
    }

    public struct TrialData
    {
        // assign
        int trialid;
        int firstid, secondid;
        string prefix;
        // raw
        public int tp1Count, tp2Count;
        public long      t1ShowupStamp, t2ShowupStamp;
        public long      tp1SuccessStamp, tp2SuccessStamp;
        public Vector2   tp1SuccessPosition, tp2SuccessPosition;
        public ArrayList tp1FailPositions;
        public string    tp2FailPositions;
        public long serverSendDataStamp, clientReceivedDataStamp;
        public long clientSendDataStamp, serverReceivedDataStamp;
        // calculate
        public bool isTrialSuccessWithNoError;
        public bool isTarget1SuccessWithNoError, isTarget2SuccessWithNoError;
        public long completeTime, dataTransfer1Time, dataTransfer2Time;
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
            completeTime = calTimeSpan(tp2SuccessStamp, t1ShowupStamp);
            dataTransfer1Time = calTimeSpan(clientReceivedDataStamp, serverSendDataStamp);
            dataTransfer2Time = calTimeSpan(serverReceivedDataStamp, clientSendDataStamp);
            target1CompleteTime = calTimeSpan(tp1SuccessStamp, t1ShowupStamp);
            target2CompleteTime = calTimeSpan(tp2SuccessStamp, tp1SuccessStamp);
            target1ShowTime = calTimeSpan(tp1SuccessStamp, t1ShowupStamp);
            target2ShowTime = calTimeSpan(tp2SuccessStamp, t2ShowupStamp);
        }

        public void setPrefix(string pre)
        {
            prefix = pre + ";";
        }

        public string getAllDataForFile()
        {
            calMoreData();
            string str;
            // assign
            str = prefix
                + trialid.ToString() + ";" + firstid.ToString() + ";" + secondid.ToString() + ";"
                // calculate
                + isTrialSuccessWithNoError.ToString() + ";"
                + isTarget1SuccessWithNoError.ToString() + ";" + isTarget2SuccessWithNoError.ToString() + ";"
                + completeTime.ToString() + ";" 
                + dataTransfer1Time.ToString() + ";" + dataTransfer2Time.ToString() + ";"
                + target1CompleteTime.ToString() + ";" + target2CompleteTime.ToString() + ";"
                + target1ShowTime.ToString() + ";" + target2ShowTime.ToString() + ";"
                // raw: success position
                + tp1SuccessPosition.ToString() + ";" + tp2SuccessPosition + ";"
                // raw: other data
                + serverSendDataStamp.ToString() + ";" + clientReceivedDataStamp.ToString() + ";"
                + clientSendDataStamp.ToString() + ";" + serverReceivedDataStamp.ToString() + ";"
                + tp1Count.ToString() + ";"
                + t1ShowupStamp.ToString() + ";" + tp1SuccessStamp.ToString() + ";"
                + tp2Count.ToString() + ";"
                + t2ShowupStamp.ToString() + ";" + tp2SuccessStamp.ToString() + ";"
                ;
            if (tp1Count > 1)
            {
                for (int i = 0; i < tp1FailPositions.Count; i++)
                {
                    str += tp1FailPositions[i].ToString() + "*";
                }
            }
            else
            {
                str += "T1NoError";
            }
            str += ";";
            str += (tp2Count > 1) ? tp2FailPositions : "T2NoError";
            str += ";";
            return str;
        }
    }

    public struct TrialSequence
    {
        int lenTrial;
        string prefix;
        //PublicLabFactors.Posture posture;
        public List<int> seqRamdon;
        public List<int> seqTarget1;
        public List<int> seqTarget2;
        public List<(int firstid, int secondid)> seqAllTargets;

        public void setTrialLength(PublicLabFactors.LabScene name)
        {
            lenTrial = GlobalController.Instance.curLabInfos.totalTrialCount;
            seqRamdon = new List<int>();
            seqTarget1 = new List<int>();
            seqTarget2 = new List<int>();
            seqAllTargets = new List<(int firstid, int secondid)>();
        }
        public void setPrefix(string pre)
        {
            prefix = pre + ";";
        }

        public void setAllQuence(PublicLabFactors.Lab1_tap_99.Posture p)
        {
            var positions = new (int firstid, int secondid)[comPositions.Length];
            //positions = comPositions4postures[p];
            positions = comPositions;
            Debug.Log(p.ToString());
            Debug.Log(positions.Length);
            seqRamdon = randomSequence(positions.Length);
            for (int i = 0; i < lenTrial; i++)
            {
                int pid = seqRamdon[i];
                seqTarget1.Add(positions[pid].firstid);
                seqTarget2.Add(positions[pid].secondid);
                seqAllTargets.Add(positions[pid]);
            }
        }

        public void printSequence(List<int> list)
        {
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + "-";
            }
            Debug.Log(res);
        }

        public string getAllDataForFile()
        {
            string res = "";
            string str = getSequenceString(seqRamdon);
            string st1 = getSequenceString(seqTarget1);
            string st2 = getSequenceString(seqTarget2);
            string sall = getSequenceString(seqAllTargets);
            res = string.Format(
                "{0}Target1;{2}{1}" +
                "{0}Target2;{3}{1}" +
                "{0}AllTargets;{4}{1}" +
                "{0}RamdonSeq;{5}{1}",
                prefix, Environment.NewLine, st1, st2, sall, str
                );
            return res;
        }

        private string getSequenceString(List<int> list)
        {
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + ";";
            }
            return res;
        }
        private string getSequenceString(List<(int firstid, int secondid)> list)
        {
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i].ToString() + ";";
            }
            return res;
        }

        private List<int> randomSequence(int len)
        {
            List<int> array = new List<int>();
            for (int i = 0; i < len; i++)
            {
                array.Add(i);
            }
            System.Random rd = new System.Random();
            List<int> result = new List<int>();
            while (len > 0)
            {
                int rdnum = rd.Next(0, len);
                int target = (int)array[rdnum];
                array.Remove(target);
                len--;
                result.Add(target);
            }
            return result;
        }
    }

    public struct Trial
    {
        public int index;
        //public int startid;
        public int firstid, secondid;

        public void setParams(int idx, int id1, int id2)
        {
            index = idx;
            firstid = id1;
            secondid = id2;
        }

        public void printParams()
        {
            Debug.Log("Trial no." + index + " tid1/2: " + firstid + "/" + secondid);
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

    public static readonly Dictionary<PublicLabFactors.Lab1_tap_99.Posture, (int firstid, int secondid)[]> 
        comPositions4postures =
        new Dictionary<PublicLabFactors.Lab1_tap_99.Posture, (int firstid, int secondid)[]>
        {
            { PublicLabFactors.Lab1_tap_99.Posture.none_right1,   comPositions },
            { PublicLabFactors.Lab1_tap_99.Posture.left1_right1,  comPositions },
            { PublicLabFactors.Lab1_tap_99.Posture.left1_right2,  comPositions },
            { PublicLabFactors.Lab1_tap_99.Posture.right1_right2, comPositions },
            { PublicLabFactors.Lab1_tap_99.Posture.right2_right3, comPositions },
        };

    static private (int firstid, int secondid)[] comPositions = new[] {
        (0, 0), (0, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8),
        (1, 0), (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8),
        (2, 0), (2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (2, 7), (2, 8),
        (3, 0), (3, 1), (3, 2), (3, 3), (3, 4), (3, 5), (3, 6), (3, 7), (3, 8),
        (4, 0), (4, 1), (4, 2), (4, 3), (4, 4), (4, 5), (4, 6), (4, 7), (4, 8),
        (5, 0), (5, 1), (5, 2), (5, 3), (5, 4), (5, 5), (5, 6), (5, 7), (5, 8),
        (6, 0), (6, 1), (6, 2), (6, 3), (6, 4), (6, 5), (6, 6), (6, 7), (6, 8),
        (7, 0), (7, 1), (7, 2), (7, 3), (7, 4), (7, 5), (7, 6), (7, 7), (7, 8),
        (8, 0), (8, 1), (8, 2), (8, 3), (8, 4), (8, 5), (8, 6), (8, 7), (8, 8),
    };


    public struct BlockCondition
    {
        string prefix;
        int blockid;
        PublicLabFactors.Lab1_tap_99.Posture posture;
        PublicLabFactors.Lab1_tap_99.Orientation orientation;
        PublicLabFactors.Lab1_tap_99.Shape shape;
        float angle;

        public BlockCondition(int bid, int p, int a, int s, int o)
        {
            blockid = bid;
            prefix = string.Format("B{0:D2}", bid);
            posture = (PublicLabFactors.Lab1_tap_99.Posture)p;
            orientation = (PublicLabFactors.Lab1_tap_99.Orientation)o;
            shape = (PublicLabFactors.Lab1_tap_99.Shape)s;
            angle = PublicLabFactors.Lab1_tap_99.AngleBetweenScreens[(int)a];
        }

        public int getBlockid()
        {
            return blockid;
        }

        public PublicLabFactors.Lab1_tap_99.Posture getPosture()
        {
            return posture;
        }

        public float getAngle()
        {
            return angle;
        }

        public PublicLabFactors.Lab1_tap_99.Shape getShape()
        {
            return shape;
        }

        public PublicLabFactors.Lab1_tap_99.Orientation getOrientation()
        {
            return orientation;
        }

        public string getAllDataForSceneDisplay()
        {
            string str;
            str = "Posture: " + posture.ToString() + Environment.NewLine
                + "Shape: " + shape.ToString() + Environment.NewLine
                + "Angle: " + angle.ToString() + "°" + Environment.NewLine
                + "Orientation: " + orientation.ToString()
                ;
            return str;
        }

        public string getAllDataForFile()
        {
            string str;
            str = prefix + ";"
                + posture.ToString() + ";"
                + shape.ToString() + ";" 
                + angle.ToString() + ";"
                + orientation.ToString() + ";"
                ;
            //+ targetSize.ToString() + ";"
            //;
            return str;
        }
    }
}
