using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LabFactors;

public class BlockAssigner : MonoBehaviour
{
    public GameObject fileProcessor;

    const int repetition = 5;
    const int blockCount = 2;

    struct Experiemnt
    {
        int usernum;
        string userid;
        BlockSequence seqBlock;
    }

    struct BlockSequence
    {
        int[] seqPosture;
    }

    struct BlockCondition
    {
        //ArrayList idxCondt;
        int[] idxCondt;
        Posture posture;
        Orientation orientation;
        Morphology morphology;
        float angle;
        float targetSize;

        void setOtherParams()
        {
            posture = (Posture)idxCondt[0];
            orientation = (Orientation)idxCondt[1];
            morphology = (Morphology)idxCondt[2];
            angle = AngleBetweenScreens[(int)idxCondt[3]];
            targetSize = TargetSize[(int)idxCondt[4]];
        }
        public void init(int p, int o, int m, int a, int t)
        {
            /*
            idxCondt = new ArrayList();
            idxCondt.Add(p);
            idxCondt.Add(o);
            idxCondt.Add(m);
            idxCondt.Add(a);
            idxCondt.Add(t);
            */
            idxCondt = new int[5];
            idxCondt[0] = p;
            idxCondt[1] = o;
            idxCondt[2] = m;
            idxCondt[3] = a;
            idxCondt[4] = t;
            setOtherParams();
        }
        public string getAllData()
        {
            string str = "";
            for(int i = 0; i<idxCondt.Length; i++)
            {
                str += idxCondt[i].ToString() + ";";
            }
            str += posture.ToString() + ";" + orientation.ToString() + ";"
                + morphology.ToString() + ";" + angle.ToString() + ";"
                + targetSize.ToString() + ";"
                ;
            return str;
        }
    }

    private string userid;
    private int curBlockid;
    private BlockCondition[] blockConditions = new BlockCondition[blockCount + 1];

    // Start is called before the first frame update
    void Start()
    {
        userid = "user1";
        curBlockid = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void scheduleSequence ()
    {
        // todo
        blockConditions[curBlockid].init(0, 0, 0, 0, 0);
    }

    #region Public method
    public void writeBlockDataToFile(out bool writeFinished)
    {
        string filename = userid + ".txt";
        string strContent = blockConditions[curBlockid].getAllData();
        fileProcessor.GetComponent<FileProcessor>().
            writeNewDataToFile(filename, strContent, out writeFinished);
        writeFinished = true;
    }

    public void moveToNextBlock()
    {
        curBlockid++;
        scheduleSequence();
    }

    public bool haveNextBlock()
    {
        if(curBlockid + 1 <= blockCount)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public string getFilename()
    {
        return userid + ".txt";
    }
    #endregion
}
