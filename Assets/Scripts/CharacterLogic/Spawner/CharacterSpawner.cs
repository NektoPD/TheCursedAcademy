using UnityEngine;

namespace CharacterLogic.Spawner
{
    public class CharacterSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        public void Spawn(Character character)
        {
            character.transform.position = _spawnPoint.position;
        }
    
    }
}
