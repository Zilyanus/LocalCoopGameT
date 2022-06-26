using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    bool isPaused;
    [SerializeField] GameObject PausePanel;
    [SerializeField] UnityEvent OnPausedAndDepaused;
    // Update is called once per frame
    void Update()
    {
        PausePanel.SetActive(isPaused);
    }

    public void Pause()
    {
        OnPausedAndDepaused.Invoke();
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }
}
