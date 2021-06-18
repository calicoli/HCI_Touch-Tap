using UnityEngine;
using static lab0Factors;

public class lab0PhaseController : MonoBehaviour
{
    public lab0UIController uiController;
    public lab0TrialController trialController;

    private ServerController sender;
    private Lab0Phase curPhase;
    private bool updatedSceneToClient;

    // Start is called before the first frame update
    void Start()
    {
        sender = GlobalController.Instance.server;
        GlobalController.Instance.curServerScene = PublicLabFactors.LabScene.Lab0_tap_5_5;
        updatedSceneToClient = false;
        if (GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(PublicLabFactors.MessageType.Scene);
            updatedSceneToClient = true;
        }
        switchPhase(Lab0Phase.in_lab0_scene);

        bool inDebugMode = GlobalController.Instance.curLabInfos.labMode == PublicLabFactors.LabMode.Test
            ? true : false;
        uiController.setDebugUIVisibility(inDebugMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToClient && GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(PublicLabFactors.MessageType.Scene);
            updatedSceneToClient = true;
        }


        if (curPhase == Lab0Phase.in_lab0_scene)
        {
            switchPhase(Lab0Phase.check_connection);
        }
        else if(curPhase == Lab0Phase.check_connection)
        {
            // check if client is in the lab scene
            if (GlobalController.Instance.getConnectionStatus())
            {
                trialController.setConnectionStatus(true);
                switchPhase(Lab0Phase.check_client_scene);
            }
        }
        else if(curPhase == Lab0Phase.check_client_scene)
        {
            if(GlobalController.Instance.curClientScene == PublicLabFactors.LabScene.Lab0_tap_5_5)
            {
                switchPhase(Lab0Phase.in_experiment);
            }
        }
        else if(curPhase == Lab0Phase.in_experiment)
        {
            trialController.setExperimentStatus(true);
        }
        else if (curPhase == Lab0Phase.end_experiment)
        {
            trialController.setExperimentStatus(false);
            curPhase = Lab0Phase.wait_to_back_to_entry;
        }
        else if(curPhase == Lab0Phase.wait_to_back_to_entry)
        {
            uiController.btnBack.gameObject.SetActive(true);
        }
        else if(curPhase == Lab0Phase.out_lab0_scene)
        {
            if(GlobalController.Instance.haveNextBlock())
            {
                GlobalController.Instance.moveToNextBlock();
            } else
            {
                sender.prepareNewMessage4Client(PublicLabFactors.MessageType.Command, 
                    PublicLabFactors.ServerCommand.server_say_end_lab);
                uiController.ShowTheEndText();
                GlobalController.Instance.writeAllBlocksFinishedFlagToFile();
            }
        }
    }

    private void switchPhase(Lab0Phase p0)
    {
        curPhase = p0;
        GlobalController.Instance.curLab0Phase = curPhase;
    }

    #region Public Method
    public void moveToPhase(Lab0Phase p0)
    {
        switchPhase(p0);
    }
    #endregion
}
