using System;
using UnityEngine;

namespace YG
{
    public class PaymentsCatalogYG : MonoBehaviour
    {
        public bool spawnPurchases = true;
        public Transform rootSpawnPurchases;
        public GameObject purchasePrefab;
        public enum UpdateListMethod { OnEnable, Start, DoNotUpdate };
        public UpdateListMethod updateListMethod;

        public PurchaseYG[] purchases = new PurchaseYG[0];

        public Action onUpdatePurchasesList;

        public void BuyPurchase(string id) { }
    }
}