using UnityEngine;
using TMPro;

public class ScorePanel : MonoBehaviour
{


    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Player Data")]
    [SerializeField] private FloatVariable playerScore;


    private const string SCORE_BASE_TEXT = "Skor: %";


    private void Update()
    {
        scoreText.text = SCORE_BASE_TEXT + ((int) playerScore.Value).ToString();
    }
}
