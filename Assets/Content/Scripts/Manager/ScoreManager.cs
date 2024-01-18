using TMPro;
using UnityEngine;

namespace Content.Scripts.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI driftingScoreText;

        public void AddDriftingScore(float score)
        {
            driftingScoreText.text += $"Score: {score}";
        }
    }
}
