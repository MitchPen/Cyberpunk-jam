using System;
using UnityEngine;

namespace Main.Scripts.Level
{
    public class PlayerTrigger : MonoBehaviour
    {
        public event Action<Player.Player> OnPlayerReachTrigger;

        public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out Player.Player player))
                OnPlayerReachTrigger?.Invoke(player);
        }
    }
}
