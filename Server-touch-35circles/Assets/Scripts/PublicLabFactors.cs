﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PublicLabFactors : MonoBehaviour
{
    public const int block_start_index = 1;
    public const int trial_start_index = 1;

    // WelcomePhase for server
    public enum WelcomePhase
    {
        in_entry_scene = 0,
        wait_for_input_information = 1,
        assign_block_conditions = 2,
        adjust_block_conditions = 3,
        ready_to_enter_lab = 4,
        in_lab_scene = 5,
    }


    public enum LabScene
    {
        Entry_scene = 0,
        Lab1_tap_33_33 = 1,
        Lab2_tap_57_57 = 2,
    }

    public enum LabMode
    {
        Full = 0,
        Test = 1,
    }

    public enum MessageType
    {
        Command = 0,
        Block = 1,
        Angle = 2,
        Trial = 3,
        Scene = 4,
    }

    public enum ServerCommand
    {
        no_server_command = 0,
        server_set_target_lab = 1,
        server_begin_to_receive_acc = 2,
        server_confirm_block_conditions = 3,
        server_say_enter_lab = 4,
        server_say_exit_lab = 5,
        server_say_end_lab = 6,
    }

    public enum ClientCommand
    {
        no_client_command = 0,
        client_finish_lab_set = 1,
        client_begin_to_deliver_acc = 2,
        client_confirm_block_conditions = 3,
        client_enter_lab = 4,
    }

    #region Trial conditions
    enum Factors
    {
        Posture = 0,
        Orientation = 1,
        Shape = 2,
        AngleBetweenScreens = 3,
        TargetSize = 4
    }

    public enum Posture {
        none_right1   = 0,           // old_rightForefinger = 0,
        left1_right1  = 1,           // new_both_thumbs = 1,
        left1_right2  = 2,           // new_leftThumb_rightForefinger = 2,
        right1_right2 = 3,           // new_rightThumb_rightForefinger = 3,
        right2_right3 = 4,           // new_rightForefinger_rightMiddlefinger = 4,
    }
    public enum Orientation {
        protrait = 0,
        landscape = 1
    }

    public enum Shape {
        concave = 0,
        convex = 1
    }

    public static float[] AngleBetweenScreens = { 60, 90, 135, 225, 270, 300 };
    public static float[] TargetSize = { 1 };

    private static Dictionary<string, int> factorTotal = new Dictionary<string, int>();

    public class Lab1_tap_33
    {
        public const int
            totalBlockCount = 60,
            fullTrialCount = 9 * 9,
            testTrialCount = 5,
            repetitionCount = 1,
            s1PositionCount = 9,
            s2PositionCount = 9,
            s1ColumnCount = 3, s1RowCount = 3,
            s2ColumnCount = 3, s2RowCount = 3;
    }

    public class Lab2_tap_57
    {
        public const int
            totalBlockCount = 64,
            totalTrialCount = 35 * 35,
            repetitionCount = 3,
            s1PositionCount = 35,
            s2PositionCount = 35,
            s1ColumnCount = 5, s1RowCount = 7,
            s2ColumnCount = 5, s2RowCount = 7;
    }
    #endregion

    public struct LabInfos
    {
        public LabScene labName;
        public LabMode labMode;
        public int totalBlockCount, totalTrialCount, repetitionCount,
        s1PositionCount, s2PositionCount,
            s1ColumnCount, s1RowCount,
            s2ColumnCount, s2RowCount;

        public int postureCount, angleCount, orientationCount;

        public void setLabInfoWithName(LabScene name)
        {
            labName = name;
            switch (name)
            {
                case LabScene.Lab1_tap_33_33:
                    setParams(Lab1_tap_33.totalBlockCount, Lab1_tap_33.fullTrialCount,
                        Lab1_tap_33.repetitionCount, Lab1_tap_33.s1PositionCount, Lab1_tap_33.s2PositionCount,
                        Lab1_tap_33.s1ColumnCount, Lab1_tap_33.s1RowCount,
                        Lab1_tap_33.s2ColumnCount, Lab1_tap_33.s2RowCount,
                        Enum.GetNames(typeof(Posture)).Length, AngleBetweenScreens.Length, Enum.GetNames(typeof(Orientation)).Length);
                    break;
                case LabScene.Lab2_tap_57_57:
                    setParams(Lab2_tap_57.totalBlockCount, Lab2_tap_57.totalTrialCount,
                        Lab2_tap_57.s1ColumnCount, Lab2_tap_57.s1RowCount,
                        Lab2_tap_57.s2ColumnCount, Lab2_tap_57.s2RowCount,
                        Lab2_tap_57.repetitionCount, Lab2_tap_57.s1PositionCount, Lab2_tap_57.s2PositionCount,
                        5, 6, 2);
                    break;
                default:
                    break;
            }
        }
        private void setParams(int cntBlock, int cntTrial, int cntRepetition, int cntS1Pos, int cntS2Pos,
            int cntS1Column, int cntS1Row, int cntS2Column, int cntS2Row,
            int cntPosture, int cntAngle, int cntOrientation)
        {
            totalBlockCount = cntBlock;
            totalTrialCount = cntTrial;
            labMode = LabMode.Full;
            repetitionCount = cntRepetition;
            s1PositionCount = cntS1Pos;
            s2PositionCount = cntS2Pos;

            postureCount = cntPosture;
            angleCount = cntAngle;
            orientationCount = cntOrientation;
        }

        public void setTotalTrialCount(bool isFullMode)
        {
            switch (labName) {
                case LabScene.Lab1_tap_33_33:
                    labMode = isFullMode ? LabMode.Full : LabMode.Test;
                    totalTrialCount = isFullMode ? Lab1_tap_33.fullTrialCount : Lab1_tap_33.testTrialCount;
                    break;
                case LabScene.Lab2_tap_57_57:
                    // todo
                    break;
                default:
                    break;
            }
        }
        
    }

    public struct BlockSequence
    {
        int lenBlock;
        public ArrayList seqPosture;
        public ArrayList seqShape;
        public ArrayList seqAngle;
        public ArrayList seqOrientation;
        public ArrayList seqTargetSize;

        public void setBlockLength(LabScene name)
        {
            if(name == LabScene.Lab1_tap_33_33)
            {
                lenBlock = Lab1_tap_33.totalBlockCount;
            }
            else if (name == LabScene.Lab2_tap_57_57)
            {
                lenBlock = Lab2_tap_57.totalBlockCount;
            }
            seqPosture = new ArrayList();
            seqAngle = new ArrayList();
            seqShape = new ArrayList();
            seqOrientation = new ArrayList();
            seqTargetSize = new ArrayList();
        }

        public void printSequence(ArrayList list)
        {
            string res = "";
            for(int i=0; i< list.Count; i++)
            {
                res += list[i] + "-";
            }
            Debug.Log(res);
        }

        public void setAllSequence(int userid)
        {
            ArrayList tmpPosture = getPostureSequence(),
                      tmpAngle = getDoubleAngleSequenceWithUserid(userid),
                      tmpOrientation = getFirstOrientationSequenceWithUserid(userid);
            ArrayList quadrupleAngle = getPalindromeArrayList(tmpAngle),
                      doubleOrientation = getPalindromeArrayList(tmpOrientation);

            for (int blockid = 0; blockid < lenBlock; blockid++)
            {
                int pid, aid, sid, oid;
                // posture id
                pid = (int)tmpPosture[blockid / (lenBlock / tmpPosture.Count)];
                seqPosture.Add(pid);
                // angle id
                aid = (int)quadrupleAngle[(blockid % quadrupleAngle.Count)];
                seqAngle.Add(aid);
                // shape id
                sid = AngleBetweenScreens[aid] < 180 ? (int)Shape.concave : (int)Shape.convex;
                seqShape.Add(sid);
                // orientation id
                oid = (int)doubleOrientation[blockid % doubleOrientation.Count];
                seqOrientation.Add(oid);
            }
        }

        public ArrayList getPostureSequence()
        {
            ArrayList postures = new ArrayList();
            postures = randomSequence(Enum.GetNames(typeof(Posture)).Length);
            return postures;
        }
        public ArrayList getDoubleAngleSequenceWithUserid(int userid)
        {
            ArrayList angles = new ArrayList();
            if(userid % 4 == 1 || userid % 4 == 3)
            {
                for (int i = 0; i < AngleBetweenScreens.Length; i++)
                {
                    angles.Add(i); angles.Add(i);
                }

            } else if (userid % 4 == 0 || userid % 4 == 2)
            {
                for(int i = AngleBetweenScreens.Length-1; i > -1; i--)
                {
                    angles.Add(i); angles.Add(i);
                }
            }
            return angles;
        }
        public ArrayList getFirstOrientationSequenceWithUserid(int userid)
        {
            ArrayList oris = new ArrayList();
            if (userid % 4 == 1 || userid % 4 == 2)
            {
                for (int i = 0; i < Enum.GetNames(typeof(Orientation)).Length; i++)
                {
                    oris.Add(i);
                }

            }
            else if (userid % 4 == 3 || userid % 4 == 0)
            {
                for (int i = Enum.GetNames(typeof(Orientation)).Length - 1; i > -1; i--)
                {
                    oris.Add(i);
                }
            }
            return oris;
        }

        public string getAllDataWithID(int factorNumber)
        {
            string res = "";
            if (factorNumber == 4)
            {
                //Debug.Log("len sequence：" + seqPosture.Count);
                string sp = getSequenceString(seqPosture);
                string sa = getSequenceString(seqAngle);
                string sc = getSequenceString(seqShape);
                string so = getSequenceString(seqOrientation);
                res = string.Format(
                    "Posture:{0}{1}{0}Angle:{0}{2}{0}Shape: {0}{3}{0}Orientation: {0}{4}{0}",
                    Environment.NewLine, sp, sa, sc, so
                    );
            }
            return res;
        }

        /*
        public string getAllDataWithIDAndName(int factorNumber)
        {
            string res = "";
            if(factorNumber == 4)
            {
                string sp = getSequenceString((int)Factors.Posture, seqPosture),
                   so = getSequenceString((int)Factors.Orientation, seqOrientation),
                   sc = getSequenceString((int)Factors.Shape, seqShape);
                string sa = getSequenceString(AngleBetweenScreens, seqAngle);
                res = string.Format(
                    "Posture: {0}{1}Angle1stBlock: {2}{3}Shape1st1st: {4}{5}Orientation1stBlock: {6}",
                    sp, Environment.NewLine, sa, Environment.NewLine,
                    sc, Environment.NewLine, so
                    );
            }
            else if(factorNumber == 5)
            {
                string sp = getSequenceString((int)Factors.Posture, seqPosture),
                   so = getSequenceString((int)Factors.Orientation, seqOrientation),
                   sc = getSequenceString((int)Factors.Shape, seqShape);
                string sa = getSequenceString(AngleBetweenScreens, seqAngle),
                       sts = getSequenceString(TargetSize, seqTargetSize);
                res = string.Format(
                    "Posture: {0}{1}Angle: {4}{5}Shape: {6}{7}Orientation: {2}{3}TargetSize: {8}",
                    sp, Environment.NewLine, sa, Environment.NewLine,
                    sc, Environment.NewLine, so, Environment.NewLine, sts);
            }
            return res;
            
        }
        */
        private ArrayList getPalindromeArrayList(ArrayList list)
        {
            ArrayList res = list;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                res.Add(list[i]);
            }
            return res;
        }

        private string getSequenceString(ArrayList list)
        {
            string res = "";
            
            for(int i=0; i< list.Count; i++)
            {
                res += list[i] + ";";
            }
            return res;
        }

        private string getSequenceString(float[] seq, ArrayList list)
        {
            string res = "";;
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + "-" + seq[(int)list[i]] + "; ";
            }
            return res;
        }

        private string getSequenceString(int num, ArrayList list)
        {
            string res = "";
            switch (num)
            {
                case 0:
                    for (int i = 0; i< list.Count; i++)
                    {
                        res += list[i] + "-" + (Posture)list[i] + "; ";
                    }
                    break;
                case 1:
                    for (int i = 0; i < list.Count; i++)
                    {
                        res += list[i] + "-" + (Orientation)list[i] + "; ";
                    }
                    break;
                case 2:
                    for (int i = 0; i < list.Count; i++)
                    {
                        res += list[i] + "-" + (Shape)list[i] + "; ";
                    }
                    break;
                case 3:
                    for (int i = 0; i < list.Count; i++)
                    {
                        res += list[i] + "-" + AngleBetweenScreens[(int)list[i]] + "; ";
                    }
                    break;
                case 4:
                    for (int i = 0; i < list.Count; i++)
                    {
                        res += list[i] + "-" + TargetSize[(int)list[i]] + "; ";
                    }
                    break;
                default:
                    break;
            }
            return res;
        }

        private string getSequenceString(Type type, ArrayList list)
        {
            // https://docs.microsoft.com/zh-cn/dotnet/api/system.enum?view=net-5.0
            if (!type.IsEnum)
                throw new System.ComponentModel.InvalidEnumArgumentException();
            
            int idx = 1;
            Factors factor = (Factors)idx;
            string name = factor.ToString();
            //enum x = (enum)name;
            string res = "";
            for(int i=0; i< list.Count; i++)
            {
                //res += list[i] + "-" + Enum.GetName(type).ToString();
            }
            return res;
        }

        private ArrayList randomSequence(int len)
        {
            ArrayList array = new ArrayList();
            for(int i = 0; i < len; i++)
            {
                array.Add(i);
            }
            System.Random rd = new System.Random();
            ArrayList result = new ArrayList();
            while(len > 0)
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

    public struct BlockCondition
    {
        int blockid;
        Posture posture;
        Orientation orientation;
        Shape shape;
        float angle;
        //float targetSize;

        public BlockCondition(int bid, int p, int a, int s, int o)
        {
            blockid = bid;
            posture = (Posture)p;
            orientation = (Orientation)o;
            shape = (Shape)s;
            angle = AngleBetweenScreens[(int)a];
            //targetSize = TargetSize[(int)t];
        }

        public int getBlockid()
        {
            return blockid;
        }

        public Posture getPosture()
        {
            return posture;
        }

        public float getAngle()
        {
            return angle;
        }

        public Shape getShape()
        {
            return shape;
        }

        public Orientation getOrientation()
        {
            return orientation;
        }

        public string getAllDataForSceneDisplay()
        {
            string str;
            str = "Posture: " + posture.ToString() + Environment.NewLine 
                + "Shape: "   + shape.ToString()   + Environment.NewLine
                + "Angle: "   + angle.ToString() + "°" + Environment.NewLine
                + "Orientation: " + orientation.ToString()
                ;
            return str;
        }

        public string getAllDataForFile()
        {
            string str;
            str = "Block " + (blockid).ToString() + ": "
                + posture.ToString() + ";"
                + shape.ToString() + ";" + angle.ToString() + ";"
                + orientation.ToString() + ";"
                ;
                //+ targetSize.ToString() + ";"
                //;
            return str;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}