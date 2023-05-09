using System.Text.Json.Serialization;

namespace Cluster_K_Means_NAI_Server.DTOs;

public class ResultDTO
{
    [JsonPropertyName("Clusters")]
    public List<ClusterDTO> Clusters { get; set; } = new();
    
    [JsonPropertyName("Iterations")]
    public int Iterations { get; set; }
    
    [JsonPropertyName("History")]
    public List<HistoryDTO> History { get; set; } = new();
}