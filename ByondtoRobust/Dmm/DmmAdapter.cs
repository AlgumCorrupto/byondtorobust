using System.Text.Json;
using System.IO;

namespace Dmm
{
    struct adapterJson {
        public Dictionary<string, string> Tiles;
        public Dictionary<string, string> Entities;
    }
    public sealed class DmmAdapter
    {
        string TilePath = "";
        string EntityPath = "";

        public Dictionary<string, string> EntityMap = new Dictionary<string, string>();
        // SS14 to byond's tile mappings
        public Dictionary<string, string> TileMap = new Dictionary<string, string>();
        // Byond to SS14 tile mappings
        public Dictionary<string, string> ReverseMap = new Dictionary<string, string>();
        public DmmAdapter(String tile, string entity)
        {
            TilePath = tile;
            EntityPath = entity;
            ReadFile();
        }

        private void ReadFile()
        {
            TileMap = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(TilePath));
            EntityMap = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(EntityPath));
        }
    }
}
