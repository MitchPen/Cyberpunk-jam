using UnityEngine;

namespace Main.Scripts.Player
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "Configuration/Player/Create new MovementConfig", order = 1)]
    public class MovementConfig : ScriptableObject
    {
        [Header("Movement")] [SerializeField] public float maxVelocity = 10f;
        [SerializeField] public float velocityMultiplier = 6f;
        [Header("Jumping")] [SerializeField] public float jumpForce = 2f;
        public float airMultiplier = 0.8f;
        public float gravity = -15f;
        public float steepSlopesGravityMultiplier = 40f;
    }
}
