using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEndingPanel : MonoBehaviour
{


    [Header("UI Elements")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject starsParentObject;

    [Header("Player Data")]
    [SerializeField] private FloatVariable playerScore;

    [Header("Panel Sprites")]
    [SerializeField] private Sprite winPanelSprite;
    [SerializeField] private Sprite losePanelSprite;

    [Header("Star Sprites")]
    [SerializeField] private Sprite starSprite;
    [SerializeField] private Sprite emptyStarSprite;


    private const string winText = "TESLİMAT BAŞARILI";
    private const string loseText = "TESLİMAT BAŞARISIZ";

    private float levelPassScore;

    public void InitializeEndingPanel(FloatVariable levelPassScore)
    {
        this.levelPassScore = levelPassScore.Value;
        
        UpdateBasePanel();
        UpdateScoreText();
        UpdateStarDisplay();
    }

    private void UpdateBasePanel()
    {
        if (playerScore.Value >= levelPassScore)
        {
            titleText.text = winText;
            backgroundImage.sprite = winPanelSprite;
        }
        else
        {
            titleText.text = loseText;
            backgroundImage.sprite = losePanelSprite;
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = ((int) playerScore.Value).ToString();
    }

    private void UpdateStarDisplay()
    {
        int starCount = GetStarCount(levelPassScore);
        for (int i = 0; i < starsParentObject.transform.childCount; i++)
        {
            Image starImage = starsParentObject.transform.GetChild(i).GetComponent<Image>();
            if (i < starCount)
            {
                starImage.sprite = starSprite;
            }
            else
            {
                starImage.sprite = emptyStarSprite;
            }
        }
    }

    private int GetStarCount(float levelPassScore)
    {
        int starCount = 0;

        if (playerScore.Value >= levelPassScore * 1.25f)
        {
            starCount = 3;
        }
        else if (playerScore.Value >= levelPassScore * 1.15f)
        {
            starCount = 2;
        }
        else if (playerScore.Value >= levelPassScore)
        {
            starCount = 1;
        }

        return starCount;
    }
}
