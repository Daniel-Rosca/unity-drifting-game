using TMPro;
using UnityEngine;

namespace Content.Scripts.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI driftingScoreText;

        private float _scorePoints;

        public void AddDriftingScore(float score)
        {
            _scorePoints += score;
            driftingScoreText.text = $"Score: {_scorePoints:###,###,000}";
        }
    }
}
