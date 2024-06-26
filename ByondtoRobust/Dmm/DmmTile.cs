

namespace Dmm
{
    public sealed class DmmTile
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;
        public readonly string[] Objs = Array.Empty<string>();

        ///<summary>
        ///Object type that the DMM Parser spills
        ///</summary>
        ///<param name="coords">XYZ coords of the tile.</param>
        ///<param name="objs">The DM objects which the tile contains.</param>
        public DmmTile(string[] objs, int[] coords)
        {
            X = coords[0];
            Y = coords[1];
            Z = coords[2];

            Objs = objs;
        }
        public int[] Getxyz()
        {
            int[] temp = { X, Y, Z };
            return temp;
        }

        public int[] Getxy()
        {
            int[] temp = { X, Y };
            return temp;
        }

        public string[] GetObjs()
        {
            return Objs;
        }

    }
}
