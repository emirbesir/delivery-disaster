using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    private void Start()
    {
        UnpauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
    }
    
    public void SetTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }
}
