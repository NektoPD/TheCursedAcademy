using System;
using System.Collections.Generic;
using System.Linq;
using Items.BaseClass;
using Items.ItemData;
using UnityEngine;

namespace Items.ItemHolder
{
    public class ItemsHolder : MonoBehaviour
    {
        [SerializeField] private List<Item> _itemPrefabs;

        public Item GetItemByType(Enums.ItemVariations itemVariations)
        {
            return Instantiate(_itemPrefabs.FirstOrDefault(prefab => prefab.Data.ItemVariation == itemVariations));
        }
    }
}