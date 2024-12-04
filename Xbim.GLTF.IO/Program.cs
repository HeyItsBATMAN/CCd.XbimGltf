using Xbim.GLTF;
using Xbim.GLTF.SemanticExport;
using Xbim.Ifc;
using Xbim.Common.Geometry;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: Xbim.GLTF.IO <input.ifc> [output.gltf]");
            return;
        }

        var inputFile = args[0];
        var outputFile = args.Length > 1 ? args[1] : Path.ChangeExtension(inputFile, ".gltf");

        try
        {
            using (var model = IfcStore.Open(inputFile))
            {
                Console.WriteLine($"Converting {inputFile} to GLTF...");

                var builder = new Builder();
                var gltf = builder.BuildInstancedScene(model, XbimMatrix3D.Identity);

                glTFLoader.Interface.SaveModel(gltf, outputFile);

                Console.WriteLine($"Successfully converted to {outputFile}");

                // Optionally export semantic data
                var jsonFileName = Path.ChangeExtension(outputFile, "json");
                var bme = new BuildingModelExtractor();
                var rep = bme.GetModel(model);
                rep.Export(jsonFileName);

                Console.WriteLine($"Semantic data exported to {jsonFileName}");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
