using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class lab1UIController : MonoBehaviour
{
    public Camera renderCamera;
    public Text txtFinishLab;
    private bool isConnecting;
    private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
    private Color connectColor = new Color(0f, 0f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        txtFinishLab.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        isConnecting = GlobalController.Instance.getConnectionStatus();
        renderCamera.backgroundColor = (isConnecting ? connectColor : disconnectColor);
    }

    public void SwitchToEntryScene()
    {
        SceneManager.LoadScene("Entry");
    }

    public void ShowTheEndText()
    {
        txtFinishLab.gameObject.SetActive(true);
    }
}
