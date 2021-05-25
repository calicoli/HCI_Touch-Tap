using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToShowTapScene()
    {
        SceneManager.LoadScene("Show-tap");
    }

    public void SwitchToShowSwipeScene()
    {
        SceneManager.LoadScene("Show-swipe");
    }

    public void SwitchToLab1TapScene()
    {
        SceneManager.LoadScene("Lab1-tap");
    }
}
