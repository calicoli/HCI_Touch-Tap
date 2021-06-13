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

    private ServerCommand curServerCmd;
    private bool isExcutingCmd;
    private int testnumber = 0;

    private bool updatedSceneToServer;

    // Start is called before the first frame update
    void Start()
    {
        curServerCmd = PublicLabFactors.ServerCommand.no_server_command;
        isExcutingCmd = false;

        sender = GlobalController.Instance.client.GetComponent<ClientController>();
        GlobalController.Instance.curClientScene = LabScene.Entry_scene;
        updatedSceneToServer = false;
        if (GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(MessageType.Scene);
            updatedSceneToServer = true;
        }

        curPhase = GlobalController.Instance.curEntryPhase;
        prevPhase = WelcomePhase.in_lab_scene;


    }

    // Update is called once per frame
    void Update()
    {
        if (!updatedSceneToServer && GlobalController.Instance.getConnectionStatus())
        {
            sender.prepareNewMessage4Server(MessageType.Scene);
            updatedSceneToServer = true;
        }


        if (curServerCmd == ServerCommand.no_server_command
            && !isExcutingCmd 
            && GlobalController.Instance.serverCmdQueue.Count > 0)
        {
            isExcutingCmd = true;
            curServerCmd = GlobalController.Instance.serverCmdQueue.Dequeue();
            testnumber++;
            Debug.Log("C excute: " + testnumber.ToString()+ curServerCmd.ToString());
            
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
            if (curServerCmd == ServerCommand.server_set_target_lab)
            {
                uiController.GetComponent<enUIController>().setLabInfoVisibility(true, true);
                finishCurrentServerCmdExcution();
                GlobalController.Instance.serverHaveSetLab = true;
                switchPhase(WelcomePhase.wait_for_server_accept_acc);
            }
            if (GlobalController.Instance.serverHaveSetLab)
            {
                uiController.GetComponent<enUIController>().setLabInfoVisibility(true, true);
                switchPhase(WelcomePhase.wait_for_server_accept_acc);
            }
        }
        else if (curPhase == WelcomePhase.wait_for_server_accept_acc)
        {
            if (curServerCmd == ServerCommand.server_begin_to_receive_acc)
            {
                finishCurrentServerCmdExcution();
                switchPhase(WelcomePhase.deliver_angle_info);
            }
        }
        else if (curPhase == WelcomePhase.deliver_angle_info)
        {
            GlobalController.Instance.setAngleDetectStatus(true);
            if (curServerCmd == ServerCommand.server_confirm_block_conditions)
            {
                finishCurrentServerCmdExcution();
                GlobalController.Instance.setAngleDetectStatus(false);
                switchPhase(WelcomePhase.ready_to_enter_lab);
            }
        }
        else if (curPhase == WelcomePhase.ready_to_enter_lab)
        {
            if (curServerCmd == ServerCommand.server_say_enter_lab)
            {
                switchPhase(WelcomePhase.in_lab_scene);
            }
        }
        else if (curPhase == WelcomePhase.in_lab_scene)
        {
            SceneManager.LoadScene(GlobalController.Instance.getTargetLabName());
        }
    }

    private void switchPhase(WelcomePhase wp)
    {
        GlobalController.Instance.curEntryPhase = wp;
        curPhase = wp;
    }

    private void finishCurrentServerCmdExcution()
    {
        testnumber++;
        Debug.Log("C finish: " + testnumber.ToString() + curServerCmd.ToString());
        curServerCmd = ServerCommand.no_server_command;
        isExcutingCmd = false;
    }


    #region public method
    public void tryToConnectServer(string ip) {

        GlobalController.Instance.serverip = ip;
        GlobalController.Instance.connectServer();
        switchPhase(WelcomePhase.detect_connect_status);
    }

    #endregion
}
