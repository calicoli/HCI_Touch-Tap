using System;
using UnityEngine;
using UnityEngine.UI;
using static lab1Factors;

public class lab1UIController : MonoBehaviour
{
    public lab1PhaseController phaseController;

    public Camera renderCamera;
    public Button btnBack;
    public Text txtFinishLab;

    public Text txtAngle;
    public Text txtDebug;

    private bool isConnecting;
    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        btnBack.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalController.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
        txtAngle.text = "Angle: " + Math.Round(GlobalController.Instance.curAngle, 1).ToString() + "°";
    }

    public void BackToEntrySceneSoon()
    {
        phaseController.moveToPhase(Lab1Phase.out_lab1_scene);
    }

    public void ShowTheEndText()
    {
        btnBack.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(true);
    }

    public void setDebugUIVisibility(bool debugging)
    {
        txtAngle.gameObject.SetActive(debugging);
        txtDebug.gameObject.SetActive(debugging);
    }

    public void setTrialInfo(string prefix, int id1, int id2)
    {
        txtDebug.text = string.Format("{0}: ({1:D2}, {2:D2})", prefix, id1, id2);
    }

}
