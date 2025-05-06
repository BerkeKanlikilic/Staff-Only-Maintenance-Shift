using _Scripts.Game.Objective;
using UnityEngine;

namespace _Scripts.Game.GameState
{
    public class InGameState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Game STARTED. Timer and objectives begin!");

            float duration = GameManager.Instance.GameDuration;
            TimeManager.Instance.StartTimer(duration);

            ObjectiveManager.Instance?.InitializeObjectives();
        }

        public void Exit()
        {
            Debug.Log("Exiting IN-GAME state.");
        }
    }
}
