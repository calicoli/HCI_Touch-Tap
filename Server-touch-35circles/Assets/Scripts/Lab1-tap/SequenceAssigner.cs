using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LabFactors;

public class SequenceAssigner : MonoBehaviour
{
    const int repetition = 5;
    struct Experiemnt
    {
        int usernum;
        string userid;
        ExperimentSequence seqExperiment;
    }

    struct ExperimentSequence
    {
        int[] seqPosture;
    }

    struct ExperimentCondition
    {
        Posture posture;
        Orientation orientation;
        Morphology morphology;
        float angle;
        float targetSize;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void scheduleSequence ()
    {
        // todo
    }
}
