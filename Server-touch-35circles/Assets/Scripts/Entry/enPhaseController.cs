using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicLabFactors;

public class enPhaseController : MonoBehaviour
{
    public GameObject uiController;

    private ServerController sender;

    private WelcomePhase wphase;
    private int userid;


    private bool updatedSceneToClient;

    // Start is called before the first frame update
    void Start()
    {
        sender = GlobalController.Instance.server.GetComponent<ServerController>();
        GlobalController.Instance.curServerScene = LabScene.Entry_scene;
        updatedSceneToClient = false;
        if(GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(MessageType.Scene);
            updatedSceneToClient = true;
        }

        wphase = GlobalController.Instance.curEntryPhase;
        switchPhase(wphase);
    }

    // Update is called once per frame
    void Update()
    {
        uiController.GetComponent<enUIController>().debugText.text = wphase.ToString();
        uiController.GetComponent<enUIController>().setServerip();

        if (!updatedSceneToClient && GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(MessageType.Scene);
            updatedSceneToClient = true;
        }

        if (wphase == WelcomePhase.adjust_block_conditions)
        {
            uiController.GetComponent<enUIController>().setAngleInfoContent(GlobalController.Instance.curAngle);
            if(checkAngleValidation() /* true */)
            {
                uiController.GetComponent<enUIController>().setBlockInfoVisibility(true, true, false);

            } else
            {
                uiController.GetComponent<enUIController>().setBlockInfoVisibility(true, false, false);
            }
        }
    }
    private bool checkAngleValidation()
    {
        float conditionAngle = GlobalController.Instance.curBlockCondition.getAngle();
        float currentAngle = GlobalController.Instance.curAngle;
        if (Mathf.Abs(conditionAngle - currentAngle) < 5f)
        {
            return true;
        }
        return false;
    }

    private void switchPhase(WelcomePhase wp)
    {
        int curphase = (int)wp;
        WelcomePhase prevPhase = curphase > 0 ?
            (WelcomePhase)curphase - 1 :
            (WelcomePhase)(Enum.GetNames(typeof(WelcomePhase)).Length - 1);
        Debug.Log("Phase changed: " + prevPhase + " -> " + wp);

        GlobalController.Instance.curEntryPhase = wp;

        if (wp == WelcomePhase.in_entry_scene)
        {
            uiController.GetComponent<enUIController>().setEnterLabBtnVisibility(false);
            wphase = WelcomePhase.wait_for_input_information;
            switchPhase(wphase);

        }
        else if (wp == WelcomePhase.wait_for_input_information)
        {
            uiController.GetComponent<enUIController>().setUserLabInfoVisibility(false);
            uiController.GetComponent<enUIController>().setBlockInfoVisibility(false, false, false);
        }
        else if (wp == WelcomePhase.assign_block_conditions)
        {
            // GlobalController assigns block conditions
            uiController.GetComponent<enUIController>().setBlockInfoContent(GlobalController.Instance.curBlockid, GlobalController.Instance.curBlockCondition);
            sender.prepareNewMessage4Client(MessageType.Block);
            GlobalController.Instance.excuteCommand(ServerCommand.server_set_target_lab);
            wphase = WelcomePhase.adjust_block_conditions;
            switchPhase(wphase);
        }
        else if (wp == WelcomePhase.adjust_block_conditions)
        {
            GlobalController.Instance.excuteCommand(ServerCommand.server_begin_to_receive_acc);
            GlobalController.Instance.angleProcessor.GetComponent<AngleProcessor>().setReceivingAccStatus(true);
            uiController.GetComponent<enUIController>().setUserLabInfoVisibility(true);
            uiController.GetComponent<enUIController>().setBlockInfoVisibility(true, false, false);
            uiController.GetComponent<enUIController>().setEnterLabBtnVisibility(false);
        }
        else if (wp == WelcomePhase.ready_to_enter_lab)
        {
            GlobalController.Instance.excuteCommand(ServerCommand.server_confirm_block_conditions);
            GlobalController.Instance.angleProcessor.GetComponent<AngleProcessor>().setReceivingAccStatus(false);
            uiController.GetComponent<enUIController>().setBlockInfoVisibility(true, true, true);
            uiController.GetComponent<enUIController>().btnEnterLab.gameObject.SetActive(true);
        }
        else if (wp == WelcomePhase.in_lab_scene)
        {
            GlobalController.Instance.excuteCommand(ServerCommand.server_say_enter_lab);
            GlobalController.Instance.writeCurrentBlockConditionToFile();
            string sceneToLoad = GlobalController.Instance.getLabSceneToEnter();
            SceneManager.LoadScene(sceneToLoad);
        }
    }


    #region Public Region
    public void moveToPhase(WelcomePhase wp)
    {
        wphase = wp;
        GlobalController.Instance.curEntryPhase = wphase;
        switchPhase(wphase);
    }
    public void setUserid(int id)
    {
        userid = id;
        GlobalController.Instance.userid = userid;
    }
    public void setLabInfo(LabScene name, bool isFullMode)
    {
        bool finished = GlobalController.Instance.setLabParams(name, isFullMode);
        if (finished)
        {
            GlobalController.Instance.writeAllBlockConditionsToFile();
        } else
        {
            Debug.Log("Do not finish Func:writeAllBlockConditionsToFile()");
        }
    }

    #endregion
}
