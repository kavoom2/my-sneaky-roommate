using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LittleDinoLini
{
    public class RandomSpawnPointStrategy : ISpawnPointStrategy
    {
        Transform[] _spawnPoints;
        List<Transform> _unusedSpawnPoints;

        public RandomSpawnPointStrategy(Transform[] spawnPoints)
        {
            _spawnPoints = spawnPoints;
            _unusedSpawnPoints = new List<Transform>(spawnPoints);
        }

        public Transform NextSpawnPoint()
        {
            if (!_unusedSpawnPoints.Any())
            {
                _unusedSpawnPoints = new List<Transform>(_spawnPoints);
            }

            int randomIndex = Random.Range(0, _unusedSpawnPoints.Count);
            Transform nextSpawnPoint = _unusedSpawnPoints[randomIndex];
            _unusedSpawnPoints.RemoveAt(randomIndex);
            return nextSpawnPoint;
        }
    }
}
