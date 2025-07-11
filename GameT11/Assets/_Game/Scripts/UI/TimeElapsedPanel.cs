using UnityEngine;
using TMPro;

public class TimeElapsedPanel : MonoBehaviour
{


    [Header("UI Elements")]
    [SerializeField] private TMP_Text timeText;

    [Header("Time Data")]
    [SerializeField] private FloatVariable timeElapsed;


    private const string TIME_FORMAT = "{0:00}:{1:00}";


    private void Update()
    {
        timeElapsed.SetValue(Time.timeSinceLevelLoad);
        timeText.text = FormatTime(timeElapsed.Value);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
        return string.Format(TIME_FORMAT, minutes, seconds);
    }
}
