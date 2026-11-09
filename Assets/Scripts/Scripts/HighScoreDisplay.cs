using UnityEngine;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI highscoreText;

    private void Update()
    {

        UpdateHighScore();

       
    }

    public void UpdateHighScore()
        {
        
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        highscoreText.text = "HighScore: " + highScore;

        }


}
