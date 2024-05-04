using Sirenix.OdinInspector;
using UnityEngine;

namespace Main.Scripts
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private GameObject _visualPart;

        [Button]
        public void Open()
        {
            _visualPart.SetActive(false);
        }
        
        [Button]
        public void Close()
        {
            _visualPart.SetActive(true);
        }
    }
}
