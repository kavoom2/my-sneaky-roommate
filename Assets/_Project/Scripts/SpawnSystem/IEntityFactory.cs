using UnityEngine;

namespace LittleDinoLini
{
    public interface IEntityFactory<T>
        where T : Entity
    {
        public T Create(Transform spawnPoint);
    }
}
