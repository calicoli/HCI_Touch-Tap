using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PublicLabFactors;

public class enPhaseController : MonoBehaviour
{
    public GameObject uiController;

    private ClientController sender;

    private WelcomePhase curPhase, prevPhase;

    private bool updatedSceneToServer;

    // Start is called before the first frame update
    void Start()
    {
        uiController.GetComponent<enUIController>().setStartUIInvisible();
        sender = GlobalController.Instance.client.GetComponent<ClientController>();
        GlobalController.Instance.curClientScene = LabScene.Entry_scene;
        updatedSceneToServer = false;
        if (GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(MessageType.Scene);
            updatedSceneToServer = true;
        }

        curPhase = GlobalController.Instance.curEntryPhase;
        prevPhase = WelcomePhase.out_entry_scene;
    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToServer && GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(MessageType.Scene);
            updatedSceneToServer = true;
        }
        
        uiController.GetComponent<enUIController>().debugText.text = curPhase.ToString();
        if(curPhase != prevPhase)
        {
            GlobalController.Instance.curEntryPhase = curPhase;
            Debug.Log("EntryPhase changed: " + prevPhase + " -> " + curPhase);
            prevPhase = curPhase;
        }

        if(curPhase == WelcomePhase.in_entry_scene)
        {
            uiController.GetComponent<enUIController>().setConnectionInfoVisibility(false);
            uiController.GetComponent<enUIController>().setLabInfoVisibility(false, false);
            switchPhase(WelcomePhase.wait_for_input_serverip);

        }
        else if (curPhase == WelcomePhase.wait_for_input_serverip)
        {
            // wait
        }
        else if (curPhase == WelcomePhase.detect_connect_status)
        {
            if (GlobalController.Instance.getConnectionStatus())
            {
                uiController.GetComponent<enUIController>().setConnectionInfoVisibility(true);
                switchPhase(WelcomePhase.wait_for_server_set_lab);
                uiController.GetComponent<enUIController>().setLabInfoVisibility(true, false);
            }
        }
        else if (curPhase == WelcomePhase.wait_for_server_set_lab)
        {
            if (GlobalController.Instance.serverCmdQueue.Count != 0 && 
                GlobalController.Instance.serverCmdQueue.Peek() == ServerCommand.server_set_target_lab)
            {
                uiController.GetComponent<enUIController>().setLabInfoVisibility(true, true);
                finishCurrentServerCmdExcution();
                GlobalController.Instance.isLabInfoSet = true;
                switchPhase(WelcomePhase.check_server_scene);
            }
        }
        else if(curPhase == WelcomePhase.check_server_scene)
        {
            uiController.GetComponent<enUIController>().setLabInfoVisibility(true, true);
            if (GlobalController.Instance.curServerScene == LabScene.Entry_scene)
            {
                switchPhase(WelcomePhase.wait_for_server_accept_acc);
            }
        }
        else if (curPhase == WelcomePhase.wait_for_server_accept_acc)
        {
            if (GlobalController.Instance.serverCmdQueue.Count != 0 && 
                GlobalController.Instance.serverCmdQueue.Peek() == ServerCommand.server_begin_to_receive_acc)
            {
                finishCurrentServerCmdExcution();
                switchPhase(WelcomePhase.deliver_angle_info);
            }
        }
        else if (curPhase == WelcomePhase.deliver_angle_info)
        {
            GlobalController.Instance.setAngleDetectStatus(true);
            if (GlobalController.Instance.serverCmdQueue.Count != 0 &&
                GlobalController.Instance.serverCmdQueue.Peek() == ServerCommand.server_confirm_block_conditions)
            {
                finishCurrentServerCmdExcution();
                GlobalController.Instance.setAngleDetectStatus(false);
                switchPhase(WelcomePhase.ready_to_enter_lab);
            }
        }
        else if (curPhase == WelcomePhase.ready_to_enter_lab)
        {
            if (GlobalController.Instance.serverCmdQueue.Count != 0 && 
                GlobalController.Instance.serverCmdQueue.Peek() == ServerCommand.server_say_enter_lab)
            {
                finishCurrentServerCmdExcution();
                switchPhase(WelcomePhase.out_entry_scene);
            }
        }
        else if (curPhase == WelcomePhase.out_entry_scene)
        {
            SceneManager.LoadScene(GlobalController.Instance.getTargetLabName());
        }
    }

    private void switchPhase(WelcomePhase wp)
    {
        curPhase = wp;
    }

    private void finishCurrentServerCmdExcution()
    {
        ServerCommand sc = GlobalController.Instance.serverCmdQueue.Dequeue();
        Debug.Log("C excuted: " + sc.ToString());
    }


    #region public method
    public void tryToConnectServer(string ip) {

        GlobalController.Instance.serverip = ip;
        GlobalController.Instance.connectServer();
        switchPhase(WelcomePhase.detect_connect_status);
    }

    #endregion
}
