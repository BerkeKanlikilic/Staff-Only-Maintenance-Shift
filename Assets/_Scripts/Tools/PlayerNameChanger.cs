using _Scripts.Network;
using TMPro;
using UnityEngine;

namespace _Scripts.Tools
{
    public class PlayerNameChanger : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _input;

        private void Awake()
        {
            _input.onSubmit.AddListener(InputOnSubmit);
        }

        private void InputOnSubmit(string text)
        {
            PlayerInfoTracker.SetName(text);
        }
    }
}
