using UnityEngine;
using TMPro;

public class ScorePanel : MonoBehaviour
{


    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Player Data")]
    [SerializeField] private FloatVariable playerScore;


    private const string scoreBaseText = "Skor: ";


    private void Update()
    {
        scoreText.text = scoreBaseText + ((int) playerScore.Value).ToString();
    }
}
