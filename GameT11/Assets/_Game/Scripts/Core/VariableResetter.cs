using UnityEngine;

public class VariableResetter : MonoBehaviour
{

    
    [Header("Variables to Reset")]
    [SerializeField] private FloatVariable playerScore;
    [SerializeField] private FloatVariable timeElapsed;


    private const float INITIAL_PLAYER_SCORE = 100f;
    private const float INITIAL_TIME_ELAPSED = 0f;


    private void Start()
    {
        ResetVariables();
    }

    public void ResetVariables()
    {
        playerScore.SetValue(INITIAL_PLAYER_SCORE);
        timeElapsed.SetValue(INITIAL_TIME_ELAPSED);
    }
}
