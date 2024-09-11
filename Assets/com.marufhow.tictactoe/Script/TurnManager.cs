using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private bool xUserTurn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        xUserTurn = true; // Start with X's turn
    }

    public bool GetTurn()
    {
        bool turn = xUserTurn;
        xUserTurn = !xUserTurn; // Switch turns
        return turn;
    }
}
