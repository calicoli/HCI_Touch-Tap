using UnityEngine;
using static PublicLabFactors;

public class AngleProcessor : MonoBehaviour
{
    const float defaultAngle = Mathf.PI;
    private float angle;

    private Vector3 accThis;
    private Vector3 accOther;

    private bool inReceivingAccStatus;
    private bool inTrial;

    // Start is called before the first frame update
    void Start()
    {
        inTrial = false;
        inReceivingAccStatus = false;
        angle = defaultAngle;
    }

    // Update is called once per frame
    void Update()
    {
        if (inReceivingAccStatus || inTrial)
        {
            accThis = Input.acceleration;
            accThis.y = 0f;
            accOther = GlobalController.Instance.accClient;
            accOther.y = 0f;

            angle = Vector3.Angle(accThis, accOther);

            if(GlobalController.Instance.curBlockCondition.getShape() == Shape.concave)
            {
                GlobalController.Instance.curAngle = 180 - angle;
            } else
            {
                GlobalController.Instance.curAngle = 180 + angle;
            }

            if(inTrial)
            {
                inTrial = false;
            }
        }
        else
        {
            angle = defaultAngle;
        }
    }



    public void setReceivingAccStatus(bool open)
    {
        inReceivingAccStatus = open;
    }

    public void setTrialStatus(bool open)
    {
        inTrial = open;
    }
}
