using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabFactors : MonoBehaviour
{
    public enum Posture {
        old_rightForefinger = 0,
        new_both_thumbs = 1,
        new_leftThumb_rightForefinger = 2,
        new_rightThumb_rightForefinger = 3,
        new_rightForefinger_rightMiddlefinger = 4,
    }
    public enum Orientation {
        protrait = 0,
        landscape = 1
    }

    public enum Morphology {
        concave = 0,
        convex = 1
    }

    public float[] AngleBetweenScreens = { 60, 90, 120, 150 };
    public float[] TargetSize = { 1 };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
