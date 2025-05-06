using System.Collections;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Game.GameState
{
    // Represents the waiting state before game starts (e.g., before exit door is opened)
    public class PreGameState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Game in PRE-GAME state. Waiting for door to open.");
            UIManager.Instance?.ShowPreGameMessage();
        }

        public void Exit()
        {
            Debug.Log("Exiting PRE-GAME state.");
        }
    }
}
