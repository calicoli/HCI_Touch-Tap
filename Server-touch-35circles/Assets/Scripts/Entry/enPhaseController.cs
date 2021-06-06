using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LabFactors;

public class enPhaseController : MonoBehaviour
{
    public GameObject uiController;

    private WelcomePhase wphase;
    private int userid;
    //private LabScene labName;

    // Start is called before the first frame update
    void Start()
    {
        wphase = WelcomePhase.in_entry_scene;
        switchPhase(wphase);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            uiController.GetComponent<enUIController>().setBlockInfoVibility(false, false, false);
        }
        else if (wp == WelcomePhase.assign_block_conditions)
        {
            // GlobalController assigns block conditions
            uiController.GetComponent<enUIController>().setBlockInfoContent(GlobalController.Instance.curBlockid, GlobalController.Instance.curBlockCondition);
            
            wphase = WelcomePhase.adjust_block_conditions;
            switchPhase(wphase);
        }
        else if (wp == WelcomePhase.adjust_block_conditions)
        {
            uiController.GetComponent<enUIController>().setUserLabInfoVisibility(true);
            uiController.GetComponent<enUIController>().setBlockInfoVibility(true, false, false);
        }
        else if (wp == WelcomePhase.confirm_block_conditions)
        {
            uiController.GetComponent<enUIController>().setEnterLabBtnVisibility(true);
        }
        else if (wp == WelcomePhase.ready_to_enter_lab)
        {
            uiController.GetComponent<enUIController>().setBlockInfoVibility(true, true, true);
            uiController.GetComponent<enUIController>().btnEnterLab.gameObject.SetActive(true);
        }
        else if (wp == WelcomePhase.in_lab_scene)
        {
            GlobalController.Instance.writeCurrentBlockConditionToFile();
            string sceneToLoad = GlobalController.Instance.getLabSceneToEnter();
            SceneManager.LoadScene(sceneToLoad);
        }
    }


    #region Public Region
    public void moveToPhase(WelcomePhase wp)
    {
        wphase = wp;
        switchPhase(wphase);
    }
    public void setUserid(int id)
    {
        userid = id;
        GlobalController.Instance.userid = userid;
    }
    public void setLabName(LabScene name)
    {
        bool finished = GlobalController.Instance.setLabParams(name);
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
