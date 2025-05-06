using TMPro;
using UnityEngine;

namespace _Scripts.UI
{ 
    // Represents a single entry in the player list UI.
    public class PlayerListEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;

        public void SetName(string playerName)
        {
            nameText.text = playerName;
        }
    }
}
