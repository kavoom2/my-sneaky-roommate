using UnityEngine;

namespace LittleDinoLini
{
    public interface ISpawnPointStrategy
    {
        public Transform NextSpawnPoint();
    }
}
