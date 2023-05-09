using System.Globalization;
using Cluster_K_Means_NAI_Server.Algorithms;
using Cluster_K_Means_NAI_Server.Structures;
using Microsoft.AspNetCore.Mvc;
namespace Cluster_K_Means_NAI_Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClusterKMeansController : ControllerBase
{
    [HttpPost("init")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Init(IFormFile inputFile, int k, char csvSeparator = ';')
    {
        var fileLength = inputFile.Length;
        if (fileLength == 0)
            return BadRequest("Input file is empty");
        else if (fileLength > 1000000)
            return BadRequest("Input file is too large");
        
        var fileExtension = Path.GetExtension(inputFile.FileName);
        if (fileExtension != ".csv")
            return BadRequest("Input file must be a .csv file");
        
        var fileStreamReader = new StreamReader(inputFile.OpenReadStream());
        var fileContent = await fileStreamReader.ReadToEndAsync();
        var fileLines = fileContent.Split("\r\n");
        
        var vectors = new List<LabeledVector<double>>();
        var index = 0;
        foreach (var line in fileLines)
        {
            var lineValues = line.Split(csvSeparator);
            var label = $"Vector with index {++index}";
            var attributes = new List<double>();
            for (var i = 0; i < lineValues.Length; i++)
            {
                if (lineValues[i] == string.Empty)
                    continue;
                
                var attribute = double.Parse(lineValues[i], CultureInfo.InvariantCulture);
                attributes.Add(attribute);
            }
            if(attributes.Count == 0)
                continue;
            var vector = new LabeledVector<double>(label, attributes);
            vectors.Add(vector);
        }
        
        // vectors.ForEach(Console.WriteLine);
        
        if (vectors.Any(vector => vector.Attributes.Count != vectors[0].Attributes.Count))
            return BadRequest("All vectors must have the same number of attributes");
        
        var kMeans = new KMeans(k, vectors);
        
        await kMeans.RunAsync();
        
        return Ok(kMeans.GetResult());
    }
}