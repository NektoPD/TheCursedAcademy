using UnityEngine;

namespace CharacterLogic.Spawner
{
    public class CharacterSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        public void Spawn(CharacterController character)
        {
            Instantiate(character, _spawnPoint);
        }
    
    }
}
