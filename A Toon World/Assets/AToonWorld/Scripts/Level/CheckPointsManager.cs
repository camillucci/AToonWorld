using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class CheckPointsManager : MonoBehaviour
    { 
        private List<CheckPoint> _checkPoints = new List<CheckPoint>();
        private int _checkPointIndex = 0;


        // Initialization
        private void Awake()
        {
            var checkPoints = FindObjectsOfType<CheckPoint>();
            AddCheckPoints(checkPoints);
        }


        // Public Properties
        public IReadOnlyList<CheckPoint> CheckPoints => _checkPoints;
        public CheckPoint LastCheckPoint => _checkPoints[_checkPointIndex];
        


        public void AddCheckPoint(CheckPoint checkPoint)
        {
            SetupCheckPoint(checkPoint);
            _checkPoints.Add(checkPoint);
            OrderCheckPoints();
        }

        public void AddCheckPoints(IEnumerable<CheckPoint> checkPoints)
        {
            foreach (var checkPoint in checkPoints)
                SetupCheckPoint(checkPoint);
            _checkPoints.AddRange(checkPoints);
            OrderCheckPoints();
        }


        // Private Methods
        private void OrderCheckPoints() => _checkPoints = _checkPoints.OrderBy(cp => cp.CheckPointNumber).ToList();

        private void SetupCheckPoint(CheckPoint checkPoint)
        {
            checkPoint.PlayerHit += CheckPoint_PlayerHit;
        }

        private void CheckPoint_PlayerHit(CheckPoint checkPoint)
        {
            _checkPointIndex = Mathf.Max(_checkPoints.IndexOf(checkPoint), _checkPointIndex);
        }
    }
}
