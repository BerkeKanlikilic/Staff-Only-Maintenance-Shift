using System.Collections;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Game.GameState
{
    public class PreGameState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Game in PRE-GAME state. Waiting for door to open.");
            // Optionally trigger a UI broadcast or fade-in message
            
            UIManager.Instance?.ShowPreGameMessage();
        }

        public void Exit()
        {
            Debug.Log("Exiting PRE-GAME state.");
        }
    }
}
