using UnityEngine;
using UnityEngine.SceneManagement;
using static lab0Factors;


public class lab0PhaseController : MonoBehaviour
{
    public lab0UIController uiController;
    public lab0TrialController trialController;

    private ClientController sender;

    private Lab0Phase curPhase, prevPhase;

    private bool updatedSceneToServer;

    // Start is called before the first frame update
    void Start()
    {
        sender = GlobalController.Instance.client;
        GlobalController.Instance.curClientScene = PublicLabFactors.LabScene.Lab0_tap_5_5;
        updatedSceneToServer = false;
        if (GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(PublicLabFactors.MessageType.Scene);
            updatedSceneToServer = true;
        }
        prevPhase = Lab0Phase.out_lab0_scene;
        switchPhase(Lab0Phase.in_lab0_scene);
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToServer && GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(PublicLabFactors.MessageType.Scene);
            updatedSceneToServer = true;
        }

        if(curPhase != prevPhase)
        {
            GlobalController.Instance.curLab0Phase = curPhase;
            Debug.Log("Lab0Phase changed: " + prevPhase + " -> " + curPhase);
            prevPhase = curPhase;
        }

        if (curPhase == Lab0Phase.in_lab0_scene)
        {
            switchPhase(Lab0Phase.check_connection);
        }
        else if (curPhase == Lab0Phase.check_connection)
        {
            // check if client is in the lab scene
            if (GlobalController.Instance.getConnectionStatus())
            {
                trialController.setConnectionStatus(true);
                switchPhase(Lab0Phase.check_client_scene);
            }
        }
        else if (curPhase == Lab0Phase.check_client_scene)
        {
            if (GlobalController.Instance.curServerScene == PublicLabFactors.LabScene.Lab0_tap_5_5)
            {
                switchPhase(Lab0Phase.in_experiment);
            }
        }
        else if (curPhase == Lab0Phase.in_experiment)
        {
            trialController.setExperimentStatus(true);
        }
        else if (curPhase == Lab0Phase.end_experiment)
        {
            trialController.setExperimentStatus(false);
            curPhase = Lab0Phase.wait_to_back_to_entry;
        }
        else if (curPhase == Lab0Phase.wait_to_back_to_entry)
        {
            if (GlobalController.Instance.serverCmdQueue.Count != 0 && 
                GlobalController.Instance.serverCmdQueue.Peek() == PublicLabFactors.ServerCommand.server_say_exit_lab)
            {
                finishCurrentServerCmdExcution();
                switchPhase(Lab0Phase.out_lab0_scene);
            } else if(GlobalController.Instance.serverCmdQueue.Count != 0 && 
                GlobalController.Instance.serverCmdQueue.Peek() == PublicLabFactors.ServerCommand.server_say_end_lab)
            {
                finishCurrentServerCmdExcution();
                uiController.ShowTheEndText();
            }
        }
        else if (curPhase == Lab0Phase.out_lab0_scene)
        {
            GlobalController.Instance.curEntryPhase = PublicLabFactors.WelcomePhase.check_server_scene;
            Debug.Log("lab0Phase: back to entry scene soon");
            string entrySceneName = (PublicLabFactors.LabScene.Entry_scene).ToString();
            SceneManager.LoadScene(entrySceneName);
        }
    }

    private void switchPhase(Lab0Phase p0)
    {
        curPhase = p0;
        GlobalController.Instance.curLab0Phase = curPhase;
    }

    private void finishCurrentServerCmdExcution()
    {
        PublicLabFactors.ServerCommand sc = GlobalController.Instance.serverCmdQueue.Dequeue();
        Debug.Log("C excuted: " + sc.ToString());
    }

    #region Public Method
    public void moveToPhase(Lab0Phase p0)
    {
        switchPhase(p0);
    }
    #endregion
}
