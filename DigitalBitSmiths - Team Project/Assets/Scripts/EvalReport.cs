using TMPro;
using UnityEngine;

public class EvalReport : MonoBehaviour
{  //script is meant to try and offload score values and bring them to display

    [System.Serializable]
    public class LevelReportRow
    {
        public string sceneName;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI gradeText;
    }

    [SerializeField] private LevelReportRow[] levelRows;

    private void OnEnable()
    {
        RefreshReport();
    }

    public void RefreshReport()
    {
        foreach (LevelReportRow row in levelRows)
        {
            int bestScore = PlayerPrefs.GetInt("BestScore_" + row.sceneName, 0);
            string bestGrade = PlayerPrefs.GetString("BestRank_" + row.sceneName, "D");

            if (row.scoreText != null)
            {
                row.scoreText.text = bestScore.ToString("#,0");
            }

            if (row.gradeText != null)
            {
                row.gradeText.text = bestGrade;
            }
        }

        Debug.Log("Evaluation Report refreshed with level best scores.");
    }
}
