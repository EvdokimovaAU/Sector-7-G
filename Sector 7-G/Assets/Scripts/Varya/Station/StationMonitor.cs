using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class StationMonitor : MonoBehaviour
    {
        [Serializable]
        public class SectorView
        {
            public StationSector sector;
            public Image image;
            public Sprite spriteNormal;
            public Sprite spriteProblem;
        }

        [Header("Sectors")]
        [SerializeField] private List<SectorView> sectors = new List<SectorView>();

        private readonly Dictionary<StationSector, bool> _isProblem =
            new Dictionary<StationSector, bool>();

        private void Start()
        {
            foreach (var s in sectors)
                _isProblem[s.sector] = false;

            RefreshAll();

            //тут проверка
            //SetSectorProblem(StationSector.ReactorShop, true);  
        }

        public void SetSectorProblem(StationSector sector, bool hasProblem)
        {
            if (!_isProblem.ContainsKey(sector))
            {
                Debug.LogWarning($"[StationMonitor] Неизвестный сектор: {sector}");
                return;
            }

            _isProblem[sector] = hasProblem;
            RefreshSector(sector);
        }

        public void ResetAllProblems()
        {
            foreach (var key in new List<StationSector>(_isProblem.Keys))
                _isProblem[key] = false;

            RefreshAll();
        }

        public bool IsProblem(StationSector sector)
        {
            return _isProblem.TryGetValue(sector, out var v) && v;
        }

        private void RefreshAll()
        {
            foreach (var s in sectors)
                RefreshSector(s.sector);
        }

        private void RefreshSector(StationSector sector)
        {
            var view = sectors.Find(x => x.sector == sector);
            if (view == null || view.image == null) return;

            bool problem = _isProblem.TryGetValue(sector, out var v) && v;
            view.image.sprite = problem ? view.spriteProblem : view.spriteNormal;
            view.image.color = problem ? Color.red : Color.white;
        }
    }
}