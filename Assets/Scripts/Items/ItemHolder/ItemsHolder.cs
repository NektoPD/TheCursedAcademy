using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Items.BaseClass;
using Items.ItemData;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items.ItemHolder
{
    public class ItemsHolder : MonoBehaviour
    {
        [SerializeField] private List<Item> _itemPrefabs;
        [SerializeField] private Transform _container;

        private readonly List<Item> _createdItems = new();

        private void Awake()
        {
            CreateAllItems();
        }

        public Item GetItemByType(Enums.ItemVariations itemVariations)
        {
            return _createdItems.FirstOrDefault(prefab => prefab.Data.ItemVariation == itemVariations);
        }

        public ItemVisualData GetVisualData()
        {
            int randomIndex = Random.Range(0, _createdItems.Count);
            return _createdItems[randomIndex].VisualData;
        }

        private void CreateAllItems()
        {
            foreach (var item in _itemPrefabs.Select(itemPrefab => Instantiate(itemPrefab, _container)))
            {
                item.gameObject.SetActive(false);
                _createdItems.Add(item);
            }
        }
    }
}