using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.Player
{
    public class PlayerVisualController : MonoBehaviour
    {
        [SerializeField] private Renderer[] playerRenderers;
        [SerializeField] private Animator animator;

        public void HideForLocalPlayer()
        {
            foreach (var rend in playerRenderers)
            {
                rend.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }

        public void UpdateMovementAnimation(Vector2 moveInput, bool isGrounded)
        {
            float speed = moveInput.magnitude;
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }
}