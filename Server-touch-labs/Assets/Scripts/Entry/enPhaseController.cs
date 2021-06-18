using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicLabFactors;

public class enPhaseController : MonoBehaviour
{
    public bool debugOnPC = false;

    public enUIController uiController;

    private ServerController sender;

    private WelcomePhase curPhase, prevPhase;
    private int userid;


    private bool updatedSceneToClient;

    // Start is called before the first frame update
    void Start()
    {
        uiController.setStartUIInvisible();
        sender = GlobalController.Instance.server;
        GlobalController.Instance.curServerScene = LabScene.Entry_scene;
        updatedSceneToClient = false;
        if(GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(MessageType.Scene);
            updatedSceneToClient = true;
        }

        curPhase = GlobalController.Instance.curEntryPhase;
        prevPhase = WelcomePhase.in_lab_scene;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToClient && GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(MessageType.Scene);
            updatedSceneToClient = true;
        }

        uiController.debugText.text = curPhase.ToString();
        uiController.setServerip();

        if (curPhase != prevPhase)
        {
            GlobalController.Instance.curEntryPhase = curPhase;
            Debug.Log("EntryPhase changed: " + prevPhase + " -> " + curPhase);
            prevPhase = curPhase;
        }

        if (curPhase == WelcomePhase.in_entry_scene)
        {
            uiController.setEnterLabBtnVisibility(false);
            switchPhase(WelcomePhase.wait_for_input_information);

        }
        else if (curPhase == WelcomePhase.wait_for_input_information)
        {
            uiController.setUserLabInfoVisibility(false);
            uiController.setBlockInfoVisibility(false, false, false);
        }
        else if (curPhase == WelcomePhase.set_target_lab)
        {
            if(GlobalController.Instance.getConnectionStatus())
            {
                GlobalController.Instance.isUserLabInfoSet = true;
                sender.prepareNewMessage4Client(MessageType.Block);
                GlobalController.Instance.excuteCommand(ServerCommand.server_set_target_lab);
                switchPhase(WelcomePhase.check_client_scene);
            }
        }
        else if (curPhase == WelcomePhase.check_client_scene)
        {
            if (GlobalController.Instance.curClientScene == LabScene.Entry_scene)
            {
                switchPhase(WelcomePhase.assign_block_conditions);
            }
        }
        else if (curPhase == WelcomePhase.assign_block_conditions)
        {
            uiController.setBlockInfoContent(GlobalController.Instance.curBlockid);
            switchPhase(WelcomePhase.accept_acc_from_now);
        }
        else if (curPhase == WelcomePhase.accept_acc_from_now)
        {
            GlobalController.Instance.excuteCommand(ServerCommand.server_begin_to_receive_acc);
            GlobalController.Instance.angleProcessor.setReceivingAccStatus(true);
            uiController.setUserLabInfoVisibility(true);
            uiController.setBlockInfoVisibility(true, false, false);
            uiController.setEnterLabBtnVisibility(false);
            switchPhase(WelcomePhase.adjust_block_conditions);
        }
        else if (curPhase == WelcomePhase.adjust_block_conditions)
        {
            uiController.setAngleInfoContent(GlobalController.Instance.curAngle);
            if (debugOnPC || checkAngleValidation())
            {
                uiController.setBlockInfoVisibility(true, true, false);
            }
            else
            {
                uiController.setBlockInfoVisibility(true, false, false);
            }
        }
        else if(curPhase == WelcomePhase.confirm_block_conditions)
        {
            GlobalController.Instance.excuteCommand(ServerCommand.server_confirm_block_conditions);
            GlobalController.Instance.angleProcessor.setReceivingAccStatus(false);
            uiController.setBlockInfoVisibility(true, true, true);
            uiController.btnEnterLab.gameObject.SetActive(true);
            switchPhase(WelcomePhase.ready_to_enter_lab);
        }
        else if (curPhase == WelcomePhase.ready_to_enter_lab)
        {
            
            // then wait to click the "Enter Lab" Button
        }
        else if (curPhase == WelcomePhase.in_lab_scene)
        {
            GlobalController.Instance.excuteCommand(ServerCommand.server_say_enter_lab);
            GlobalController.Instance.writeCurrentBlockConditionToFile();
            string sceneToLoad = GlobalController.Instance.getLabSceneToEnter();
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private bool checkAngleValidation()
    {
        float conditionAngle = 0;
        switch(GlobalController.Instance.curLabInfos.labName)
        {
            case LabScene.Lab0_tap_5_5:
                conditionAngle = GlobalController.Instance.curLab0BlockCondition.getAngle();
                break;
            case LabScene.Lab1_tap_33_33:
                conditionAngle = GlobalController.Instance.curLab1BlockCondition.getAngle();
                break;
            default:
                break;
        }
        float currentAngle = GlobalController.Instance.curAngle;
        if (Mathf.Abs(conditionAngle - currentAngle) < 5f)
        {
            return true;
        }
        return false;
    }

    private void switchPhase(WelcomePhase wp)
    {
        curPhase = wp;
    }

    #region Public Region
    public void moveToPhase(WelcomePhase wp)
    {
        
        //GlobalController.Instance.curEntryPhase = wphase;
        switchPhase(wp);
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
