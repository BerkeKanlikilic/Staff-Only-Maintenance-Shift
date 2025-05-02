using TMPro;
using UnityEngine;

public class PlayerListEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;

    public void SetName(string playerName)
    {
        nameText.text = playerName;
    }
}
