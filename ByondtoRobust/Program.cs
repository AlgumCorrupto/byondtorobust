// See https://aka.ms/new-console-template for more information
using Dmm;


Console.Write("Byond to Robust tiles adapter path\n$ ");
var tiles = Console.ReadLine();
Console.WriteLine("");
if(!File.Exists(tiles)) {
    throw new Exception("File doesn't exist");
}

Console.Write("Byond to Robust entities adapter path\n$ ");
var entities = Console.ReadLine();
Console.WriteLine("");
if(!File.Exists(entities)) {
    throw new Exception("File doesn't exist");
}

Console.Write("DMM map path\n$ ");
var mapPath = Console.ReadLine();
Console.WriteLine("");
if(!File.Exists(mapPath)) {
    throw new Exception("File doesn't exist!");
}

Console.Write("File name\n$ ");
var outName = Console.ReadLine();
Console.WriteLine("");
if(outName == null) {
    throw new Exception("File name Empty");
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
