using ProtoCasual.Core.ScriptableObjects;

namespace ProtoCasual.Core.Interfaces
{
    public interface IMapGenerator
    {
        void GenerateMap(MapConfig config);
        void ClearMap();
        void RegenerateMap();
    }
}
