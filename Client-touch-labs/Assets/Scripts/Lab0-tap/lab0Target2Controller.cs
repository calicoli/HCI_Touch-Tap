using System;
using UnityEngine;
using static lab0Factors;

public class lab0Target2Controller : MonoBehaviour
{
    public GameObject targets2;

    int totalSpheres2;
    Target[] spheres2;
    Vector3[] posSpheres2;

    // Start is called before the first frame update
    void Start()
    {
        totalSpheres2 = targets2.transform.childCount;
        spheres2 = new Target[totalSpheres2];
        posSpheres2 = new Vector3[totalSpheres2];

        for (int i = 0; i < totalSpheres2; i++)
        {
            GameObject child = targets2.transform.GetChild(i).gameObject;
            if (child.name.Length == 11)
            {
                int id2 = Convert.ToInt32(child.name.Substring(9, 2));
                spheres2[id2] = new Target(id2, false, false);
                posSpheres2[id2] = child.transform.position;
            }
        }

        resetAllTargets2();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void updateTarget1Visibility(int id2)
    {
        targets2.transform.GetChild(id2).gameObject.SetActive(spheres2[id2].visible);
    }

    #region Public Method
    public void resetAllTargets2()
    {
        for (int id2 = 0; id2 < totalSpheres2; id2++)
        {
            spheres2[id2].touched = false;
            spheres2[id2].visible = false;
            updateTarget1Visibility(id2);
        }
    }
    public void updateTarget2OnScreen(int id2)
    {
        spheres2[id2].visible = true;
        updateTarget1Visibility(id2);
    }

    public void updateTarget2TouchedStatus(int id2)
    {
        spheres2[id2].touched = true;
        spheres2[id2].visible = false;
        updateTarget1Visibility(id2);
    }
    #endregion 
}
