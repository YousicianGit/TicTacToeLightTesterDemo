using UnityEngine;
using UnityEngine.UI;

namespace com.marufhow.tictactoe.script
{
  
    public class UIBehaviour : MonoBehaviour
    {
       // [SerializeField] private TextMeshProUGUI gameStatusText; 
        [SerializeField] private Text gameStatusText; 
        [SerializeField] private Button restartButton;
        [SerializeField] private GameObject containerPanel;
        private void OnEnable()
        {
            BoardManager.Instance.OnGameFinished += HandleGameFinished;
            BoardManager.Instance.OnReset += ResetUI;
        }
        private void OnDisable()
        {
            BoardManager.Instance.OnGameFinished -= HandleGameFinished;
            BoardManager.Instance.OnReset -= ResetUI;
        }
        private void Start()
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        private void HandleGameFinished(int value, bool isWin)
        {
            if (isWin)
            {
                string winner = value == 1 ? "X" : "O"; // 1 for X, 2 for O
                gameStatusText.text = $"{winner} Player Wins!";
            }
            else
            {
                gameStatusText.text = "It's a Draw! Try Again.";
            }
            containerPanel.gameObject.SetActive(true);
        }
        private void ResetUI()
        {
            gameStatusText.text = "";
            containerPanel.gameObject.SetActive(false);
        }
        private void RestartGame()
        {
            BoardManager.Instance.ResetGame();
        }
    }

}