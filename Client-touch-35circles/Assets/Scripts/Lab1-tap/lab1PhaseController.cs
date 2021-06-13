using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static lab1Factors;


public class lab1PhaseController : MonoBehaviour
{
    public GameObject uiController;
    public GameObject trialController;

    private ClientController sender;

    private Lab1Phase curPhase, prevPhase;

    private PublicLabFactors.ServerCommand curServerCmd;
    private bool isExcutingCmd;
    private int testnumber = 0;

    private bool updatedSceneToServer;

    // Start is called before the first frame update
    void Start()
    {
        curServerCmd = PublicLabFactors.ServerCommand.no_server_command;
        isExcutingCmd = false;

        sender = GlobalController.Instance.client.GetComponent<ClientController>();
        GlobalController.Instance.curClientScene = PublicLabFactors.LabScene.Lab1_tap_33_33;
        updatedSceneToServer = false;
        if (GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(PublicLabFactors.MessageType.Scene);
            updatedSceneToServer = true;
        }
        prevPhase = Lab1Phase.out_lab1_scene;
        switchPhase(Lab1Phase.in_lab1_scene);
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToServer && GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(PublicLabFactors.MessageType.Scene);
            updatedSceneToServer = true;
        }

        if (curServerCmd == PublicLabFactors.ServerCommand.no_server_command
            && !isExcutingCmd
            && GlobalController.Instance.serverCmdQueue.Count > 0)
        {
            isExcutingCmd = true;
            curServerCmd = GlobalController.Instance.serverCmdQueue.Dequeue();
            testnumber++;
            Debug.Log("C excute: " + testnumber.ToString() + curServerCmd.ToString());

        }

        if(curPhase != prevPhase)
        {
            GlobalController.Instance.curLab1Phase = curPhase;
            Debug.Log("Lab1Phase changed: " + prevPhase + " -> " + curPhase);
            prevPhase = curPhase;
        }

        if (curPhase == Lab1Phase.in_lab1_scene)
        {
            switchPhase(Lab1Phase.check_connection);
        }
        else if (curPhase == Lab1Phase.check_connection)
        {
            // check if client is in the lab scene
            if (GlobalController.Instance.getConnectionStatus())
            {
                trialController.GetComponent<lab1TrialController>().setConnectionStatus(true);
                switchPhase(Lab1Phase.check_client_scene);
            }
        }
        else if (curPhase == Lab1Phase.check_client_scene)
        {
            if (GlobalController.Instance.curServerScene == PublicLabFactors.LabScene.Lab1_tap_33_33)
            {
                switchPhase(Lab1Phase.in_experiment);
            }
        }
        else if (curPhase == Lab1Phase.in_experiment)
        {
            trialController.GetComponent<lab1TrialController>().setExperimentStatus(true);
        }
        else if (curPhase == Lab1Phase.end_experiment)
        {
            trialController.GetComponent<lab1TrialController>().setExperimentStatus(false);
            curPhase = Lab1Phase.wait_to_back_to_entry;
        }
        else if (curPhase == Lab1Phase.wait_to_back_to_entry)
        {
            if (curServerCmd == PublicLabFactors.ServerCommand.server_say_exit_lab)
            {
                finishCurrentServerCmdExcution();
                switchPhase(Lab1Phase.out_lab1_scene);
            } else if(curServerCmd == PublicLabFactors.ServerCommand.server_say_end_lab)
            {
                finishCurrentServerCmdExcution();
                uiController.GetComponent<lab1UIController>().ShowTheEndText();
            }
        }
        else if (curPhase == Lab1Phase.out_lab1_scene)
        {
            GlobalController.Instance.curEntryPhase = PublicLabFactors.WelcomePhase.detect_connect_status;
            Debug.Log("lab1Phase: back to entry scene soon");
            string entrySceneName = ((PublicLabFactors.LabScene)0).ToString();
            SceneManager.LoadScene(entrySceneName);
        }
    }

    private void switchPhase(Lab1Phase p1)
    {
        curPhase = p1;
        GlobalController.Instance.curLab1Phase = curPhase;
    }

    private void finishCurrentServerCmdExcution()
    {
        testnumber++;
        Debug.Log("C finish: " + testnumber.ToString() + curServerCmd.ToString());
        curServerCmd = PublicLabFactors.ServerCommand.no_server_command;
        isExcutingCmd = false;
    }

    #region Public Method
    public void moveToPhase(Lab1Phase p1)
    {
        switchPhase(p1);
    }
    #endregion
}
