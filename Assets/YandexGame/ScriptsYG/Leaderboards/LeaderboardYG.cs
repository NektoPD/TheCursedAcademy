using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

namespace YG
{
    public class LeaderboardYG : MonoBehaviour
    {
        public string nameLB;

        public int maxQuantityPlayers = 20;

        [Range(1, 20)]
        public int quantityTop = 3;

        [Range(1, 10)]
        public int quantityAround = 6;

        public enum UpdateLBMethod { Start, OnEnable, DoNotUpdate };
        public UpdateLBMethod updateLBMethod = UpdateLBMethod.OnEnable;

        public Text entriesText;

        public bool advanced;

        public Transform rootSpawnPlayersData;
        public GameObject playerDataPrefab;

        public enum PlayerPhoto
        { NonePhoto, Small, Medium, Large };
        public PlayerPhoto playerPhoto = PlayerPhoto.Small;
        public Sprite isHiddenPlayerPhoto;

        public bool timeTypeConvert;

        [Range(0, 3)]
        public int decimalSize = 1;

        public UnityEvent onUpdateData;

        private string photoSize;
        private LBPlayerDataYG[] players = new LBPlayerDataYG[0];

        public void UpdateLB() { }

        public void NewScore(long score) { }

        public void NewScoreTimeConvert(float score) { }

        public string TimeTypeConvert(int score)
        {
            return string.Empty;
        }

        public void SetNameLB(string name)
        {
            nameLB = name;
        }
    }
}
