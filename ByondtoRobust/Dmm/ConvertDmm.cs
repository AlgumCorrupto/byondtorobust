using System.Collections;

namespace Dmm
{
    public sealed class DmmComverter
    {
        private Dictionary<int, string> TileIdMap = new Dictionary<int, string>();
        private Dictionary<string, int> IdTileMap = new Dictionary<string, int>();
        public DmmComverter()
        {
        }

        public string ConvertToYaml(DmmMap map, DmmAdapter adapter)
        {
            var root =
            "meta:\n"
            + "  format: 6\n"
            + "  postmapinit: false\n"
            + "tilemap:\n";
            // write tile map section
            int i = 0;
            TileIdMap = new Dictionary<int, string>();
            IdTileMap = new Dictionary<string, int>();
            string[] values = new string[adapter.TileMap.Count];
            adapter.TileMap.Keys.CopyTo(values, 0);
            foreach (var tile in values)
            {
                // i've spent 2 fucking hours looking for this...
                //var tileDef = _tiles.Where(s => s.ID.Equals(tile)).First();
                TileIdMap.Add(i, tile);
                IdTileMap.Add(tile, i);
                i++;
            }
            string tileMap = "";
            var floorToId = new Dictionary<string, int>();
            foreach (var (idNum, idStr) in TileIdMap.OrderBy(x => x.Key))
            {
                tileMap += $"  {idNum.ToString()}: {idStr.ToString()}\n";
                floorToId.Add(idStr, idNum);
            }
            root += tileMap;

            int[,] currTileBuffer = new int[16, 16];
            var nChunksX = (int) (map.MaxX / 16) + 1;
            var nChunksY = (int) (map.MaxX / 16) + 1;
            int[][,] chunkIds = new int[nChunksX * nChunksY][,];

            root +=
            "entities:\n"
            + "- proto: \"\"\n"
            + "  entities:\n"
            + "  - uid: 1\n"
            + "    components:\n"
            + "    - type: MetaData\n"
            + "      name: Test\n"
            + "    - type: Transform\n"
            + "      parent: invalid\n"
            + "    - type: MapGrid\n"
            + "      chunks:\n";
            var entityStr = "";
            // write the floor tiles
            for(var j = 0; j < chunkIds.Length - 1; j++) {
                chunkIds[j] = new int[16,16];
                //Console.WriteLine("aa");
            }
            int uid = 2;
            foreach (var tile in map.Tiles)
            {
                foreach (var obj in tile.Objs)
                {
                    foreach (var entitymap in adapter.EntityMap)
                    {
                        if (entitymap.Value == obj)
                        {
                            entityStr +=
                              $"- proto: {entitymap.Key}\n"
                            + $"  entities:\n"
                            + $"  - uid: {uid}\n"
                            + $"    components:\n"
                            + $"    - type: Transform\n"
                            + $"      pos: {tile.X + 0.5},{tile.Y + 0.5}\n"
                            + $"      parent: 1\n";
                            int chunkX = tile.X / 16;
                            int chunkY = tile.Y / 16;
                            chunkIds[chunkX * nChunksX + chunkY][tile.X % 16, tile.Y % 16] = 1;

                            //entities.Add(protoNode);
                            uid++;
                        }
                    }
                    foreach (var floormap in adapter.TileMap)
                    {
                        if (floormap.Value == obj)
                        {
                            int chunkX = tile.X / 16;
                            int chunkY = tile.Y / 16;
                            chunkIds[chunkX * nChunksX + chunkY][tile.X % 16, tile.Y % 16] = floorToId[floormap.Key];
                        }
                    }
                }
            }
            var chunks = string.Empty;
            for (var x = 0; x < nChunksX; x++)
                for (var y = 0; y < nChunksY; y++)
                    chunks += CreateChunk(chunkIds[x * nChunksX + y], x, y);
            root += chunks;
            root +=
            "- type: Broadphase\n"
            + "- type: Physics\n"
            + "  bodyStatus: InAir\n"
            + "  angularDamping: 0.05\n"
            + "  linearDamping: 0.05\n"
            + "  fixedRotation: False\n"
            + "  bodyType: Dynamic\n"
            + "- type: Fixtures\n"
            + "  fixtures: {}\n"
            + "- type: BecomesStation\n"
            + "  id: test\n"
            + "- type: Gravity\n"
            + "  gravityShakeSound: !type:SoundPathSpecifier\n"
            + "    path: /Audio/Effects/alert.ogg\n";
            root += entityStr;
            root += "...\n";
            return root;
        }

        private string CreateChunk(int[,] tileIDs, int xOffs, int yOffs)
        {
            const int structSize = 6;
            const int chunkSize = 16;
            var nTiles = chunkSize * chunkSize;
            var barr = new byte[nTiles * structSize];
            var typeId = 0;

            using (var stream = new MemoryStream(barr))
            using (var writer = new BinaryWriter(stream))
            {
                for (ushort y = 0; y < chunkSize; y++)
                {
                    for (ushort x = 0; x < chunkSize; x++)
                    {
                        try { typeId = tileIDs[x, y]; }
                        catch { typeId = 0; };
                        writer.Write(typeId);
                        writer.Write((byte) 0);
                        writer.Write((byte) 0);
                    }
                }
            }
            string chunkStr =
            $"        {xOffs},{yOffs}:\n"
            + $"          ind: {xOffs},{yOffs}\n"
            + $"          tiles: {Convert.ToBase64String(barr)}\n"
            + $"          version: 6\n";
            return chunkStr;
        }

    }
}
