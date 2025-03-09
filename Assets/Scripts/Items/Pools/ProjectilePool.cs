using System.Collections.Generic;
using Items.ItemVariations;
using UnityEngine;

namespace Items.Pools
{
    public class ProjectilePool : MonoBehaviour
    {
        private Queue<Projectile> _objectPool = new();
        private Projectile _prefab;

        public void Initialize(Projectile prefab, int initialSize)
        {
            _prefab = prefab;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewPoolObject();
            }
        }

        private Projectile CreateNewPoolObject()
        {
            Projectile newObject = Instantiate(_prefab);
            newObject.transform.SetParent(transform);
            newObject.gameObject.SetActive(false);
            _objectPool.Enqueue(newObject);
            return newObject;
        }

        public Projectile GetFromPool(Vector3 position, Quaternion rotation)
        {
            Projectile objectToUse;

            if (_objectPool.Count == 0)
            {
                objectToUse = CreateNewPoolObject();
            }
            else
            {
                objectToUse = _objectPool.Dequeue();
            }

            objectToUse.transform.position = position;
            objectToUse.transform.rotation = rotation;
            objectToUse.gameObject.SetActive(true);

            return objectToUse;
        }

        public void ReturnToPool(Projectile objectToReturn)
        {
            objectToReturn.gameObject.SetActive(false);
            _objectPool.Enqueue(objectToReturn);
        }

        private void OnDestroy()
        {
            foreach (Projectile obj in _objectPool)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }

            _objectPool.Clear();
        }
    }
}