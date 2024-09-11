using com.marufhow.tictactoe.script.utility;
public class TurnManager : PersistentMonoSingleton<TurnManager>
{
    private bool xUserTurn;
    private void Start()
    {
        xUserTurn = true;  
    }
    public bool GetTurn()
    {
        bool turn = xUserTurn;
        xUserTurn = !xUserTurn; // Switch turns
        return turn;
    }
    protected override void Initialize()
    {
    }
}
