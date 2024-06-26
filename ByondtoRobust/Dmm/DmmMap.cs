namespace Dmm
{
    public sealed class DmmMap
    {
        public readonly List<DmmTile> Tiles = new List<DmmTile>();
        public readonly int MaxX;
        public readonly int MaxY;
        public readonly int MaxZ;

        public DmmMap(List<DmmTile> tiles, int maxX, int maxY, int maxZ)
        {
            Tiles = tiles;
            MaxX = maxX;
            MaxY = maxY;
            MaxZ = maxZ;
        }
    }
}
