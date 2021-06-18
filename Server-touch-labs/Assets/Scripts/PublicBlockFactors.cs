using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicLabFactors;

public class PublicBlockFactors : MonoBehaviour
{
    public struct BlockSequence
    {
        LabScene labName;
        int lenBlock;
        public ArrayList seqPosture;        // lab 0/1
        public ArrayList seqShape;          // lab 0/1
        public ArrayList seqAngle;          // lab 0/1
        public ArrayList seqOrientation;    // lab 0/1
        public ArrayList seqTargetSize;

        #region Public Method
        public void setBlockLength(LabScene name)
        {
            labName = name;
            switch (name)
            {
                case LabScene.Lab0_tap_5_5:
                    lenBlock = Lab0_tap_55.totalBlockCount;
                    break;
                case LabScene.Lab1_tap_33_33:
                    lenBlock = Lab1_tap_99.totalBlockCount;
                    break;
                case LabScene.Lab2_tap_57_57:
                    lenBlock = Lab2_tap_3535.totalBlockCount;
                    break;
            }
            seqPosture = new ArrayList();
            seqAngle = new ArrayList();
            seqShape = new ArrayList();
            seqOrientation = new ArrayList();
            seqTargetSize = new ArrayList();
        }
        public void setAllSequence(int userid)
        {
            switch (labName)
            {
                case LabScene.Lab0_tap_5_5:
                    setLab0Sequance(userid);
                    break;
                case LabScene.Lab1_tap_33_33:
                    setLab1Sequance(userid);
                    break;
                case LabScene.Lab2_tap_57_57:
                    break;
                default:
                    break;
            }
            
        }
        public void printSequence(ArrayList list)
        {
            string res = "";
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + "-";
            }
            Debug.Log(res);
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
        public string getAllDataWithLabName()
        {
            string res = "";
            if (labName == LabScene.Lab0_tap_5_5 || labName == LabScene.Lab1_tap_33_33)
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
        #endregion

        private void setLab0Sequance(int userid)
        {
            ArrayList tmpPosture = getPostureSequence(Enum.GetNames(typeof(Lab0_tap_55.Posture)).Length),
                      tmpAngle = getDoubleAngleSequenceWithUserid(userid, Lab0_tap_55.AngleBetweenScreens.Length),
                      tmpOrientation = getFirstOrientationSequenceWithUserid(userid, Enum.GetNames(typeof(Lab0_tap_55.Orientation)).Length);
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
                sid = Lab0_tap_55.AngleBetweenScreens[aid] < 180 ? (int)Lab0_tap_55.Shape.concave : (int)Lab0_tap_55.Shape.convex;
                seqShape.Add(sid);
                // orientation id
                oid = (int)doubleOrientation[blockid % doubleOrientation.Count];
                seqOrientation.Add(oid);
            }
        }
        private void setLab1Sequance(int userid)
        {
            ArrayList tmpPosture = getPostureSequence(Enum.GetNames(typeof(Lab1_tap_99.Posture)).Length),
                      tmpAngle = getDoubleAngleSequenceWithUserid(userid,Lab1_tap_99.AngleBetweenScreens.Length),
                      tmpOrientation = getFirstOrientationSequenceWithUserid(userid, Enum.GetNames(typeof(Lab1_tap_99.Orientation)).Length);
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
                sid = Lab1_tap_99.AngleBetweenScreens[aid] < 180 ? (int)Lab1_tap_99.Shape.concave : (int)Lab1_tap_99.Shape.convex;
                seqShape.Add(sid);
                // orientation id
                oid = (int)doubleOrientation[blockid % doubleOrientation.Count];
                seqOrientation.Add(oid);
            }
        }

        private ArrayList getPostureSequence(int lenPosture)
        {
            ArrayList postures = new ArrayList();
            postures = randomSequence(lenPosture);
            return postures;
        }
        private ArrayList getDoubleAngleSequenceWithUserid(int userid, int lenAngle)
        {
            ArrayList angles = new ArrayList();
            if (userid % 4 == 1 || userid % 4 == 3)
            {
                for (int i = 0; i < lenAngle; i++)
                {
                    angles.Add(i); angles.Add(i);
                }
            }
            else if (userid % 4 == 0 || userid % 4 == 2)
            {
                for (int i = lenAngle - 1; i > -1; i--)
                {
                    angles.Add(i); angles.Add(i);
                }
            }
            return angles;
        }
        private ArrayList getFirstOrientationSequenceWithUserid(int userid, int lenOrientation)
        {
            ArrayList oris = new ArrayList();
            if (userid % 4 == 1 || userid % 4 == 2)
            {
                for (int i = 0; i < lenOrientation; i++)
                {
                    oris.Add(i);
                }

            }
            else if (userid % 4 == 3 || userid % 4 == 0)
            {
                for (int i = lenOrientation; i > -1; i--)
                {
                    oris.Add(i);
                }
            }
            return oris;
        }
        private ArrayList getPalindromeArrayList(ArrayList list)
        {
            ArrayList res = list;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                res.Add(list[i]);
            }
            return res;
        }

        private ArrayList randomSequence(int len)
        {
            ArrayList array = new ArrayList();
            for (int i = 0; i < len; i++)
            {
                array.Add(i);
            }
            System.Random rd = new System.Random();
            ArrayList result = new ArrayList();
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

        private string getSequenceString(ArrayList list)
        {
            string res = "";

            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + ";";
            }
            return res;
        }
        private string getSequenceString(float[] seq, ArrayList list)
        {
            string res = ""; ;
            for (int i = 0; i < list.Count; i++)
            {
                res += list[i] + "-" + seq[(int)list[i]] + "; ";
            }
            return res;
        }

        
    }

   
}
