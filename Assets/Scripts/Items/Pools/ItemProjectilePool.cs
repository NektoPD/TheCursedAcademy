using System.Collections.Generic;
using Items.ItemVariations;
using UnityEngine;

namespace Items.Pools
{
    public class ItemProjectilePool : MonoBehaviour
    {
        private Queue<ItemProjectile> _objectPool = new Queue<ItemProjectile>();
        private ItemProjectile _prefab;

        public void Initialize<T>(T prefab, int initialSize) where T : ItemProjectile
        {
            _prefab = prefab;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewPoolObject();
            }
        }

        private ItemProjectile CreateNewPoolObject()
        {
            ItemProjectile newObject = Instantiate(_prefab);
            newObject.transform.SetParent(transform);
            newObject.gameObject.SetActive(false);
            _objectPool.Enqueue(newObject);
            return newObject;
        }

        public T GetFromPool<T>(Vector3 position, Quaternion rotation) where T : ItemProjectile
        {
            ItemProjectile objectToUse;

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

            return objectToUse as T;
        }

        public void ReturnToPool(ItemProjectile objectToReturn)
        {
            objectToReturn.gameObject.SetActive(false);
            _objectPool.Enqueue(objectToReturn);
        }

        private void OnDestroy()
        {
            foreach (ItemProjectile obj in _objectPool)
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