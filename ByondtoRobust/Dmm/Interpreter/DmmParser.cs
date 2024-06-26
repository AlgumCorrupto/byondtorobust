using System.Text;
namespace Dmm
{
    /// <summary>
    /// The class that parses Byond's map files and spills a list tile objects.
    /// it's messy, i know... But i just wanted to do smth dead simple.
    /// </summary>
    public static class DmmParser
    {
        //  private var reprToDef = new Dictionary<string, string>();

        /// <summary>
        /// Method that takes a DMM string and parses it (OBS: you need to read the content of the files first).
        /// </summary>
        /// <param name="content"></param>
        public static DmmMap Parse(byte[] content)
        {
            int nLine = 1;

            var dmmData = new List<DmmTile>();
            bool inCommentL = false;
            bool commentTrigger = false;
            bool inQuoteBlock = false;
            bool assigning = false;
            bool inTileDataBlock = false;
            bool inTilesCoordBlock = false;
            bool inObjArgs = false;
            string currNum = "";
            int baseX = 0;

            int currKLength = 0;
            int currDataIndex = 0;

            int currX = 0;
            int currY = 0;
            int currZ = 0;

            int maxX = 0;
            int maxY = 0;
            int maxZ = 0;


            string currKey = "";

            ReadingAxis axis = ReadingAxis.X;

            // stuff like (/obj/button/alpha_rendersource_test,/turf,/area)
            bool inObjDef = false;
            string currQuote = String.Empty;
            int i = 0;

            // should i use string too?
            string currObj = "";
            string currCoding = "";

            var codingsToObjMap = new Dictionary<string, string>();

            // csharpscript lel
            var flushCodingsToObjMap = () =>
            {
                codingsToObjMap.Add(currCoding, currObj);
                currObj = "";
            };

            var flushData = () =>
            {
                var key = codingsToObjMap[currKey];
                var objs = key.Split(',');
                dmmData.Add(new DmmTile(objs, [currX, currY, currZ]));
            };

            var parseNum = () =>
            {
                return Int32.Parse(currNum);
            };

            while (i < content.Length)
            {
                if (currKLength > 3)
                {
                    int d;
                }
                if (content[i] == '\n' && !inTileDataBlock)
                {
                    nLine++;
                    i++;
                    inCommentL = false;
                    commentTrigger = false;
                    continue;
                }
                if (content[i] == '\t' && !inTileDataBlock)
                {
                    i++;
                    continue;
                }
                else if (content[i] == '\r' && !inTileDataBlock)
                {
                    i++;
                    continue;
                }
                if (content[i] == '=' && !inObjDef)
                {
                    assigning = true;
                    i++;
                    continue;
                }
                if (content[i] == '/' && !inQuoteBlock)
                {
                    if (commentTrigger)
                    {
                        inCommentL = true;
                        commentTrigger = false;
                        i++;
                        continue;
                    }
                    else commentTrigger = true;
                }
                else commentTrigger = false;
                if (content[i] == '"' && !inObjDef)
                {
                    if (!inQuoteBlock && !assigning)
                    {
                        inQuoteBlock = true;
                        currCoding = "";
                        currKLength = 0;
                    }
                    else if (inQuoteBlock && !assigning)
                    {
                        inQuoteBlock = false;
                    }

                    if (inTileDataBlock && assigning)
                    {
                        inTileDataBlock = false;
                        currDataIndex = 0;
                        assigning = false;
                    }
                    else if (!inTileDataBlock && assigning)
                    {
                        inTileDataBlock = true;
                    }
                    i++;
                    continue;
                }
                if (content[i] == '(' && !inQuoteBlock && !inObjArgs)
                {
                    if (assigning) inObjDef = true;
                    else inTilesCoordBlock = true;
                    i++;
                    continue;
                }
                if (content[i] == ')' && !inQuoteBlock && !inObjArgs)
                {
                    if (assigning)
                    {
                        assigning = false;
                        inObjDef = false;
                        flushCodingsToObjMap();
                    }
                    else
                    {
                        inTilesCoordBlock = false;
                        currZ = parseNum();
                        maxZ = Math.Max(maxZ, currZ);
                        currNum = "";
                        axis = ReadingAxis.X;
                    }
                    i++;
                    continue;
                }
                if (content[i] == '{' && !inObjDef)
                {
                    currDataIndex = 0;
                    i++;
                    continue;
                }
                if (content[i] == '}' && !inObjDef)
                {
                    assigning = false;
                    inTileDataBlock = false;
                    i++;
                    continue;
                }

                if (inCommentL)
                {
                    i++;
                    continue;
                }
                if (inQuoteBlock)
                {
                    currCoding += Encoding.ASCII.GetString(new[]{content[i]});
                    i++;
                    currKLength += 1;
                    continue;
                }
                if (inTilesCoordBlock)
                {
                    if (content[i] == ',')
                    {
                        if (axis == ReadingAxis.X)
                        {
                            baseX = currX = parseNum();
                            currNum = "";
                            axis = ReadingAxis.Y;
                        }
                        else if (axis == ReadingAxis.Y)
                        {
                            currY = parseNum();
                            currNum = "";
                            axis = ReadingAxis.Z;
                        }
                    }
                    //shouuuulld be a num
                    else
                    {
                        currNum += Encoding.ASCII.GetString(new[]{content[i]});
                    }
                    i++;
                    continue;
                }
                if (inTileDataBlock)
                {
                    if (content[i] == '\n')
                    {
                        if (currDataIndex != 0) currY++;
                        maxX = Math.Max(currX - 1, maxX);
                        maxY = Math.Max(currY - 1, maxY);
                        currX = baseX;
                        i++;
                        continue;
                    }
                    currDataIndex++;
                    currKey += Encoding.ASCII.GetString(new[]{content[i]});
                    if (currDataIndex % currKLength == 0)
                    {
                        flushData();
                        currKey = string.Empty;
                        currX++;
                    }
                }
                if (inObjDef)
                {
                    if (content[i] == '{')
                    {
                        inObjArgs = true;
                    }
                    if (!inObjArgs) currObj += Encoding.ASCII.GetString(new[]{content[i]});
                    if (content[i] == '}')
                    {
                        inObjArgs = false;
                    }
                    i++;
                    continue;
                }
                i++;
                continue;
            }
            return new DmmMap(dmmData, maxX, maxY, maxZ);
        }
    }
    enum ReadingAxis { X, Y, Z };
}
