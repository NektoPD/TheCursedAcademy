using UnityEngine;

namespace Data
{
    public interface IVisualData
    {
        public string Name { get; }

        public string Description { get; }

        public Sprite Sprite { get; }
    }
}

