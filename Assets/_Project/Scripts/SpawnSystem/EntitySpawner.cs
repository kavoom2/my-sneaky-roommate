using UnityEngine;

namespace LittleDinoLini
{
    public class EntitySpawner<T>
        where T : Entity
    {
        IEntityFactory<T> _entityFactory;
        ISpawnPointStrategy _spawnPointStrategy;

        public EntitySpawner(
            IEntityFactory<T> entityFactory,
            ISpawnPointStrategy spawnPointStrategy
        )
        {
            _entityFactory = entityFactory;
            _spawnPointStrategy = spawnPointStrategy;
        }

        public T Spawn()
        {
            Transform spawnPoint = _spawnPointStrategy.NextSpawnPoint();
            return _entityFactory.Create(spawnPoint);
        }
    }
}
