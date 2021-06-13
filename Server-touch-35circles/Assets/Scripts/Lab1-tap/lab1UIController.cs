using UnityEngine;
using UnityEngine.UI;
using static lab1Factors;

public class lab1UIController : MonoBehaviour
{
    public GameObject phaseController;

    public Camera renderCamera;
    public Button btnBack;
    public Text txtFinishLab;

    private bool isConnecting;
    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        btnBack.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalController.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
    }

    public void BackToEntrySceneSoon()
    {
        phaseController.GetComponent<lab1PhaseController>().moveToPhase(Lab1Phase.out_lab1_scene);
    }

    public void ShowTheEndText()
    {
        btnBack.gameObject.SetActive(false);
        txtFinishLab.gameObject.SetActive(true);
    }
}
