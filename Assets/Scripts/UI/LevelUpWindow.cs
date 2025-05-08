using CharacterLogic.Initializer;
using Data;
using Items.ItemHolder;
using System.Collections.Generic;
using System.Linq;
using UI.Applicators;
using UnityEngine;
using Zenject;

namespace UI 
{ 
    public class LevelUpWindow : Window
    {
        private const int CountItems = 3;

        [SerializeField] private List<ItemView> _itemsVisual;
        [SerializeField] private ItemApplicator _applicator;

        private ItemsHolder _itemsHolder;
        private CharacterInitializer _initializer;

        [Inject]
        private void Construct(ItemsHolder holder, CharacterInitializer characterInitializer)
        {
            _itemsHolder = holder;
            _initializer = characterInitializer;
        }

        private void OnEnable()
        {
            _initializer.Character.LevelUp += LevelUp;
        }

        private void OnDisable()
        {
            _initializer.Character.LevelUp -= LevelUp;
        }

        public override void Show()
        {
            base.Show();

            List<ItemVisualData> datas = new();

            _itemsHolder.GetVisualDatas(CountItems, out datas);

            for (int i = 0; i < CountItems; i++)
                _itemsVisual[i].Initialize(datas[i]);

            _applicator.SetdDefaultItem(datas.First());
        }

        private void LevelUp() => Show();
    }
}