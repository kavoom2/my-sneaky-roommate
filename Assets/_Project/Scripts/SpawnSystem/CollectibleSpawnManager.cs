using UnityEngine;
using Utilities;

namespace LittleDinoLini
{
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        [SerializeField]
        CollectibleData[] _collectibleData;

        [SerializeField]
        float _spawnInterval = 1f;

        EntitySpawner<Collectible> _spawner;

        CountdownTimer _spawnTimer;
        int counter;

        protected override void Awake()
        {
            base.Awake();
            _spawner = new EntitySpawner<Collectible>(
                new EntityFactory<Collectible>(_collectibleData),
                _spawnPointStrategy
            );

            _spawnTimer = new CountdownTimer(_spawnInterval);
            _spawnTimer.OnTimerStop += () =>
            {
                if (counter++ >= _spawnPoints.Length)
                {
                    _spawnTimer.Stop();
                    return;
                }

                Spawn();
                _spawnTimer.Start();
            };
        }

        void Start()
        {
            _spawnTimer.Start();
        }

        void Update()
        {
            _spawnTimer.Tick(Time.deltaTime);
        }

        public override void Spawn()
        {
            _spawner.Spawn();
        }
    }
}
