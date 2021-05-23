using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleProcessor : MonoBehaviour
{
    public Text angleText;

    [HideInInspector]
    public bool isConnecting;

    const float defaultAngle = Mathf.PI;
    private float angle;

    private Vector3 accThis;
    private Vector3 accOther;

    // Start is called before the first frame update
    void Start()
    {
        angle = defaultAngle;
    }

    // Update is called once per frame
    void Update()
    {
        if (isConnecting)
        {
            accThis = Input.acceleration;
            accThis.y = 0f;
            //accOther = getComponent<>;
            accOther.y = 0f;

            angle = Vector3.Angle(accThis, accOther);
        }
        else
        {
            angle = defaultAngle;
        }
        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft ||
            Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            angleText.text = "L-";
        } else
        {
            angleText.text = "P-";
        }
        angleText.text += ("A: " + (180 * angle / Mathf.PI).ToString());
    }

    public float getAngle()
    {
        return angle;
    }
}
