using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stTouchDetector : MonoBehaviour
{
    public GameObject fileProcessor;
    public GameObject targetController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Ended)
            {
                process1Touch(touch.position);
                //Debug.Log("dy- END");
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            process1Touch(Input.mousePosition);
        }
    }
    
    void process1Touch(Vector2 pos)
    {
        int hitid = -1;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hit))
        {
            hitid = Convert.ToInt32(hit.collider.gameObject.name.Substring(9, 2));
            //Debug.Log("info: " + hitid.ToString() + " " + hit.collider.gameObject.name);
            Debug.DrawLine(ray.origin, hit.point, Color.yellow);
        }

        int targetid = targetController.GetComponent<stTargetController>().getCurrentTargetId();
        if(hitid == targetid)
        {
            targetController.GetComponent<stTargetController>().scheduleNextTarget(targetid, true);
        }
        else
        {
            targetController.GetComponent<stTargetController>().scheduleNextTarget(targetid, false);
        }
    }
    

}
