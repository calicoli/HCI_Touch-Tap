using UnityEngine;
using static lab1Factors;

public class lab1PhaseController : MonoBehaviour
{
    public GameObject uiController;
    public GameObject trialController;

    private ServerController sender;
    private Lab1Phase curPhase;
    private bool updatedSceneToClient;

    // Start is called before the first frame update
    void Start()
    {
        sender = GlobalController.Instance.server.GetComponent<ServerController>();
        GlobalController.Instance.curServerScene = PublicLabFactors.LabScene.Lab1_tap_33_33;
        updatedSceneToClient = false;
        if (GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(PublicLabFactors.MessageType.Scene);
            updatedSceneToClient = true;
        }
        switchPhase(Lab1Phase.in_lab1_scene);
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToClient && GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Client(PublicLabFactors.MessageType.Scene);
            updatedSceneToClient = true;
        }


        if (curPhase == Lab1Phase.in_lab1_scene)
        {
            switchPhase(Lab1Phase.check_connection);
        }
        else if(curPhase == Lab1Phase.check_connection)
        {
            // check if client is in the lab scene
            if (GlobalController.Instance.getConnectionStatus())
            {
                trialController.GetComponent<lab1TrialController>().setConnectionStatus(true);
                switchPhase(Lab1Phase.check_client_scene);
            }
        }
        else if(curPhase == Lab1Phase.check_client_scene)
        {
            if(GlobalController.Instance.curClientScene == PublicLabFactors.LabScene.Lab1_tap_33_33)
            {
                switchPhase(Lab1Phase.in_experiment);
            }
        }
        else if(curPhase == Lab1Phase.in_experiment)
        {
            trialController.GetComponent<lab1TrialController>().setExperimentStatus(true);
        }
        else if (curPhase == Lab1Phase.end_experiment)
        {
            trialController.GetComponent<lab1TrialController>().setExperimentStatus(false);
            curPhase = Lab1Phase.wait_to_back_to_entry;
        }
        else if(curPhase == Lab1Phase.wait_to_back_to_entry)
        {
            uiController.GetComponent<lab1UIController>().btnBack.gameObject.SetActive(true);
        }
        else if(curPhase == Lab1Phase.out_lab1_scene)
        {
            if(GlobalController.Instance.haveNextBlock())
            {
                GlobalController.Instance.moveToNextBlock();
            } else
            {
                sender.prepareNewMessage4Client(PublicLabFactors.MessageType.Command, 
                    PublicLabFactors.ServerCommand.server_say_end_lab);
                uiController.GetComponent<lab1UIController>().ShowTheEndText();
                GlobalController.Instance.writeAllBlocksFinishedFlagToFile();
            }
        }
    }

    private void switchPhase(Lab1Phase p1)
    {
        curPhase = p1;
        GlobalController.Instance.curLab1Phase = curPhase;
    }

    #region Public Method
    public void moveToPhase(Lab1Phase p1)
    {
        switchPhase(p1);
    }
    #endregion
}
