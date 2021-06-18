using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class enUIController : MonoBehaviour
{
    public Camera renderCamera;
    private bool isConnecting;
    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    public Text debugText;

    public GameObject phaseController;

    public InputField inputServerip;
    public Text txtConnectStatus;

    public Text txtLabName;

    public Button btnConnect;

    void Update()
    {
        isConnecting = GlobalController.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
    }

    #region Public Method
    public void setConnectionInfoVisibility(bool isConnecting)
    {
        if (isConnecting)
        {
            txtConnectStatus.text = "Connecting with" + Environment.NewLine +
                GlobalController.Instance.serverip;
        }
        inputServerip.gameObject.SetActive(!isConnecting);
        btnConnect.gameObject.SetActive(!isConnecting);
        txtConnectStatus.gameObject.SetActive(isConnecting);
    }
    public void setLabInfoVisibility(bool isConnecting, bool haveSet)
    {
        txtLabName.gameObject.SetActive(isConnecting);
        if (haveSet)
        {
            txtLabName.text = "in " + GlobalController.Instance.getTargetLabName();
        }
       
    }

    #endregion

    #region Public UI Method
    public void ConfirmServerip()
    {
        string serverip = inputServerip.text;
        phaseController.GetComponent<enPhaseController>().tryToConnectServer(serverip);
    }
    public void setStartUIInvisible()
    {
        setConnectionInfoVisibility(GlobalController.Instance.getConnectionStatus());
        setLabInfoVisibility(GlobalController.Instance.getConnectionStatus(), GlobalController.Instance.isLabInfoSet);
    }
    #endregion

}
