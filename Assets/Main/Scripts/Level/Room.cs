using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts.Level
{
    public class Room : MonoBehaviour
    {
        public event Action OnRoomPass;
        [SerializeField] private List<Door> _entrances;
        [SerializeField] private PlayerTrigger _playerTrigger;
    }
}
