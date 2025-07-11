using UnityEngine;

public class VariableResetter : MonoBehaviour
{

    
    [Header("Variables to Reset")]
    [SerializeField] private FloatVariable timeElapsed;
    [SerializeField] private FloatVariable playerScore;


    private void Start()
    {
        ResetVariables();
    }

    public void ResetVariables()
    {
        timeElapsed.SetValue(0f);
        playerScore.SetValue(0f);
    }
}
