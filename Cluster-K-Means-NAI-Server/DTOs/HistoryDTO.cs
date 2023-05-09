using System.Text.Json.Serialization;

namespace Cluster_K_Means_NAI_Server.DTOs;

public class HistoryDTO
{
    [JsonPropertyName("Iteration")]
    public int Iteration { get; set; }
    
    [JsonPropertyName("Clusters")]
    public List<ClusterDTO> Clusters { get; set; } = new();
    
}