using Dmm;

class Program 
{
    static void Main(string[] args)
    {
        if(args.Length != 4) {
            showHelp();
            return;
        }
        var tiles = args[0];
        if(!File.Exists(tiles)) {
            throw new Exception("Tile dictinary file doesn't exist!");
        }
        var entities = args[1];
        if(!File.Exists(entities)) {
            throw new Exception("Entity dictionary file doesn't exist!");
        }

        Console.Write("DMM map path\n$ ");
        var mapPath = args[2];
        if(!File.Exists(mapPath)) {
            throw new Exception("the DMM file doesn't exist");
        }

        Console.Write("File name\n$ ");
        var outName = args[3];
        Console.WriteLine("");
        if(outName == null) {
            throw new Exception("filename empty!");
        }

        var adapter = new DmmAdapter(tiles, entities);
        Console.WriteLine("adapter json read");
        var mapFile = DmmParser.Parse(File.ReadAllBytes(mapPath));
        Console.WriteLine("DMM map read");

        var converter = new DmmComverter();

        var converted = converter.ConvertToYaml(mapFile, adapter);

        Console.WriteLine("Writing output...");
        using (StreamWriter outputFile = new StreamWriter(Path.Combine("./", outName)))
        {
                outputFile.Write(converted);
        }
        Console.WriteLine("Wrote!");
        Console.ReadKey();
    }

    private static void showHelp() 
    {
        Console.WriteLine("[USAGE]:\nByondtoRobust tile-dict entity-dict dmm-file outfilename");
        Console.WriteLine("[EXAMPLE]:\nByondtoRobust ./goon-dict-tiles.json ./goon-dict-entities.json ./cogmap.dmm cogmap.yml");
    }
}

