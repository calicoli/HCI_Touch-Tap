using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static lab0Factors;

public class lab0Target1Controller : MonoBehaviour
{
    public GameObject targets1;

    int totalSpheres1;
    Target[] spheres1;
    Vector3[] posSpheres1;

    // Start is called before the first frame update
    void Start()
    {
        totalSpheres1 = targets1.transform.childCount;
        spheres1 = new Target[totalSpheres1];
        posSpheres1 = new Vector3[totalSpheres1];

        for(int i=0; i < totalSpheres1; i++)
        {
            GameObject child = targets1.transform.GetChild(i).gameObject;
            if (child.name.Length == 11)
            {
                int id1 = Convert.ToInt32(child.name.Substring(9, 2));
                spheres1[id1] = new Target(id1, false, false);
                posSpheres1[id1] = child.transform.position;
            }
        }

        resetAllTargets1();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private void updateTarget1Visibility(int id1)
    {
        targets1.transform.GetChild(id1).gameObject.SetActive(spheres1[id1].visible);
    }

    #region Public Method
    public void resetAllTargets1()
    {
        for (int id1 = 0; id1 < totalSpheres1; id1++)
        {
            spheres1[id1].touched = false;
            spheres1[id1].visible = false;
            updateTarget1Visibility(id1);
        }
    }
    public void updateTarget1OnScreen(int id1)
    {
        spheres1[id1].visible = true;
        updateTarget1Visibility(id1);
    }

    public void updateTarget1TouchedStatus(int id1)
    {
        spheres1[id1].touched = true;
        spheres1[id1].visible = false;
        updateTarget1Visibility(id1);
    }

   
    #endregion
}
