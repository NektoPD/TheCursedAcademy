using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timelines
{
    public class TimelineController : MonoBehaviour
    {
        [SerializeField] private List<Cutscene> _cutscens;

        private void Awake()
        {
            foreach (var cutscene in _cutscens)
                cutscene.CutsceneObject.SetActive(false);
        }

        public void StartCutscene(string name)
        {
            foreach(var cutscene in _cutscens)
                if(cutscene.Name == name)
                    cutscene.CutsceneObject.SetActive(true);
        }
    }

    [Serializable]
    public class Cutscene
    {
        [SerializeField] private GameObject _cutsceneObject;

        public string Name => _cutsceneObject.name;

        public GameObject CutsceneObject => _cutsceneObject;
    }
}
