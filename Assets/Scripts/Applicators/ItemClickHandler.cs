using Data;
using UnityEngine;

namespace Applicators
{
    [RequireComponent(typeof(ItemVisualData))]
    public class ItemClickHandler : BaseClickHandler<ItemVisualData>
    {
        private ItemVisualData _visualData;

        private void Awake()
        {
            _visualData = GetComponent<ItemVisualData>();
            SetData(_visualData);
        }
    }
}