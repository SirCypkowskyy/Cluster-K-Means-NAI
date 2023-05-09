using System.Text.Json.Serialization;
using Cluster_K_Means_NAI_Server.Structures;

namespace Cluster_K_Means_NAI_Server.DTOs;

public class ClusterDTO
{
    [JsonPropertyName("ClusterName")]
    public string Name { get; set; }
    
    [JsonPropertyName("Centroid")]
    public LabeledVector<double> Centroid { get; set; }
    
    [JsonPropertyName("Vectors")]
    public List<LabeledVector<double>> Vectors { get; set; }
    
    [JsonPropertyName("SumOfSquaredErrors")]
    public double SumOfSquaredErrors { get; set; }
}