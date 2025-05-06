using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.Player
{
    // Controls visual aspects of the player (e.g. shadows, animations)
    public class PlayerVisualController : MonoBehaviour
    {
        [SerializeField] private Renderer[] playerRenderers;    // Mesh renderers to control shadows
        [SerializeField] private Animator animator;             // Animator for movement states

        // Hides the player's mesh locally while still casting shadows
        public void HideForLocalPlayer()
        {
            foreach (var rend in playerRenderers)
            {
                rend.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }

        // Updates animator parameters based on movement input and grounded state. (Not Implemented Yet)
        public void UpdateMovementAnimation(Vector2 moveInput, bool isGrounded)
        {
            float speed = moveInput.magnitude;
            animator.SetFloat("Speed", speed);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }
}