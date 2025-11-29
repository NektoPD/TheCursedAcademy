using System;
using UnityEngine;
using UnityEngine.UI;
#if TMP_YG2
using TMPro;
#endif

namespace YG
{
    public class PurchaseYG : MonoBehaviour
    {
        public bool showCurrencyCode;
        public ImageLoadYG purchaseImageLoad;
        public ImageLoadYG currencyImageLoad;

        [Serializable]
        public struct TextLegasy
        {
            public Text title, description, priceValue;
        }
        public TextLegasy textLegasy;

#if TMP_YG2
        [Serializable]
        public struct TextMP
        {
            public TextMeshProUGUI title, description, priceValue;
        }
        public TextMP textMP;
#endif
        public class Purchase
        {
            public string id;
            public string title;
            public string description;
            public string imageURI;
            public string price;
            public string priceValue;
            public string priceCurrencyCode;
            public string currencyImageURL;
            public bool consumed;
        }
        public Purchase data = new Purchase();

        public void BuyPurchase() { }
    }
}
