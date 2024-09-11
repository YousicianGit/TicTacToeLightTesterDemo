using System;
using UnityEngine;

namespace com.marufhow.tictactoe.script
{
    [CreateAssetMenu(fileName = "Cell", menuName = "Game/Cell")]
    public class Cell : ScriptableObject
    {
        public int Id;
        public int Value; // 0: blank, 1: X, 2: O
        public bool IsInteractive { get; private set; }
        public event Action<int, int> OnValueChanged;
        public event Action<bool> OnGameFinished;
        public void SetValue(int newValue)
        {
            Value = newValue;
            OnValueChanged?.Invoke(Id, Value);
        }
        public void SetResult(bool isWin)
        {
            OnGameFinished?.Invoke(isWin);
            IsInteractive = false;
        }

        public void Reset()
        {
            IsInteractive = true;
            SetValue(0);
        }

    }

}