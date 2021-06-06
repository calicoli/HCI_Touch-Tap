using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using static LabFactors;


public class enUIController : MonoBehaviour
{
    public GameObject phaseController;

    public InputField inputUserid;
    public Text txtUserid;

    public Dropdown dpLabOptions;
    public Text txtLabName;

    public GameObject blockConditions;
    public Text txtBlockInfo;
    public Text txtCurrentBlockTitle, txtAngleInfo, txtAngleValid;

    public Button btnConfirmNameAndLab;
    public Button btnConfirmBlockCondition;
    public Button btnEnterLab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        txtLabName.text = "in " + ((LabScene)1).ToString();
    }

    #region Public Method
    public void setUserLabInfoVisibility(bool isInfoSet)
    {
        inputUserid.gameObject.SetActive(!isInfoSet);
        dpLabOptions.gameObject.SetActive(!isInfoSet);
        btnConfirmNameAndLab.gameObject.SetActive(!isInfoSet);
        txtLabName.gameObject.SetActive(isInfoSet);
        txtUserid.gameObject.SetActive(isInfoSet);
    }

    public void setBlockInfoVibility(bool inPhase, bool isAngleVaild, bool isConditionConfirmed)
    {
        if(!inPhase)
        {
            blockConditions.SetActive(false);
            btnConfirmBlockCondition.gameObject.SetActive(false);
        } else
        {
            blockConditions.SetActive(true);
            if (isConditionConfirmed)
            {
                txtAngleValid.text = "Confirmed";
                txtAngleValid.color = Color.green;
                btnConfirmBlockCondition.gameObject.SetActive(false);
            }
            else if (isAngleVaild && !isConditionConfirmed)
            {
                txtAngleValid.text = "Valid";
                txtAngleValid.color = Color.green;
                btnConfirmBlockCondition.gameObject.SetActive(true);
            }
            else if (!isAngleVaild && !isConditionConfirmed)
            {
                txtAngleValid.text = "Invalid";
                txtAngleValid.color = Color.red;
                btnConfirmBlockCondition.gameObject.SetActive(false);
            }
        }
        
    }

    public void setEnterLabBtnVisibility(bool readyToEnterLab)
    {
        btnEnterLab.gameObject.SetActive(readyToEnterLab);
    }

    public void setBlockInfoContent(int blockid, BlockCondition info)
    {
        string strCondition = info.getAllDataForSceneDisplay();
        txtBlockInfo.text = strCondition;

        string strBlockid = "#" + blockid.ToString() + " Block Condition";
        txtCurrentBlockTitle.text = strBlockid;
    }

    public void setAngleInfoContent(int angle)
    {
        string strAngle = angle.ToString();
        txtAngleInfo.text = "Current angle: " + strAngle + "°";
    }
    #endregion

    #region Public UI Method

    public void ConfirmUserAndLabInfo()
    {
        // user info
        int userid = int.Parse(inputUserid.text);
        txtUserid.text = "Hi, user" + userid.ToString();
        phaseController.GetComponent<enPhaseController>().setUserid(userid);
        // lab info
        int labid = dpLabOptions.value + 1;
        LabScene labName = (LabScene)labid;
        txtLabName.text = "in " + labName.ToString();
        phaseController.GetComponent<enPhaseController>().setLabName(labName);

        phaseController.GetComponent<enPhaseController>().moveToPhase(WelcomePhase.assign_block_conditions);
    }

    public void ConfirmLabConditions()
    {
        phaseController.GetComponent<enPhaseController>().moveToPhase(WelcomePhase.ready_to_enter_lab);
    }

    public void EnterSelectedLab()
    {
        phaseController.GetComponent<enPhaseController>().moveToPhase(WelcomePhase.ready_to_enter_lab);
    }
    #endregion
}
