using System.Text;
using Cluster_K_Means_NAI_Server.DTOs;
using Cluster_K_Means_NAI_Server.Structures;

namespace Cluster_K_Means_NAI_Server.Algorithms;

public class KMeans
{
    private readonly List<LabeledVector<double>> _vectors;
    private readonly List<Cluster> _clusters = new();
    private readonly int _k;
    private int _finishedIterations = 0;
    
    // history
    private readonly List<HistoryDTO> _history = new();

    public KMeans(int k, List<LabeledVector<double>> vectors)
    {
        _vectors = vectors;
        _k = k;
    }
    
    public async Task RunAsync()
    {
        Run();
        await Task.CompletedTask;
    }

    public void Run()
    {
        InitializeClusters();
        AssignVectorsToClusters();
    }


    private void InitializeClusters()
    {
        var random = new Random();
        var randomIndexes = new List<int>();

        for (var i = 0; i < _k; i++)
        {
            var randomIndex = random.Next(0, _vectors.Count);
            while (randomIndexes.Contains(randomIndex))
                randomIndex = random.Next(0, _vectors.Count);
            
            randomIndexes.Add(randomIndex);
            
            var centroid = _vectors[randomIndex].Clone();
            var cluster = new Cluster($"Cluster {i+1}", centroid);
            _clusters.Add(cluster);
        }

        Console.WriteLine($"[{DateTime.Now:s}][K-means] Clusters initialized");
    }
    
    private void AssignVectorsToClusters()
    {
        var isCentroidChanged = true;
        var currentIteration = 0;
        while (isCentroidChanged)
        {
            Console.WriteLine($"[{DateTime.Now:s}][K-means] Iteration {currentIteration}");
            _history.Add(new HistoryDTO()
            {
                Iteration = currentIteration,
                Clusters = _clusters.Select(cl =>
                {
                    var cluster = cl.Copy();
                    return new ClusterDTO()
                    {
                        Name = cluster.Name,
                        Centroid = cluster.Centroid,
                        Vectors = cluster.Vectors,
                        SumOfSquaredErrors = cluster.SumOfSquaredErrors
                    };
                }).ToList()
            });
            var sses = new StringBuilder();
            var numberOfVecs = new StringBuilder();
            foreach (var cluster in _clusters)
            {
                sses.Append($"{cluster.Name}: {cluster.SumOfSquaredErrors:F} | ");
                numberOfVecs.Append($"{cluster.Name}: {cluster.Vectors.Count} | ");
            }
            Console.WriteLine($"[{DateTime.Now:s}][K-means] Sum of squared errors: {sses}");
            Console.WriteLine($"[{DateTime.Now:s}][K-means] Number of vectors: {numberOfVecs}");
            
            _clusters.ForEach(cluster => cluster.ClearCluster());
            _vectors.ForEach(vector => _clusters[FindClosestClusterIndex(vector)].addVector(vector));
            isCentroidChanged = _clusters.Any(cluster => cluster.IsCentroidChanged());
            currentIteration++;
        }
        _finishedIterations = currentIteration;
        Console.WriteLine($"[{DateTime.Now:s}][K-means] Clusterization finished!");
    }

    private int FindClosestClusterIndex(LabeledVector<double> vector)
    {
        return _clusters
            .Select((cluster, index) => new {cluster, index})
            .OrderBy(pair => EuclideanDistance(vector, pair.cluster.Centroid))
            .First()
            .index;
    }
    

    public static double EuclideanDistance(in LabeledVector<double> vecA, in LabeledVector<double> vecB)
    {
        // C# "in" to odpowiednik "const&" z C++
        
        // if(vecA.Attributes.Count != vecB.Attributes.Count)
        //     throw new ArgumentException("Vectors must have the same number of attributes");
        
        
        var distance = 0.0;

        for (var i = 0; i < vecA.Attributes.Count; i++)
            distance += Math.Pow(vecA.Attributes[i] - vecB.Attributes[i], 2);
        
        // return Math.Sqrt(distance);
        return distance;
    }
    
    public ResultDTO GetResult()
    {
        var clustersDTO = new List<ClusterDTO>();
        
        _clusters.ForEach(cluster => clustersDTO.Add(new ClusterDTO()
        {
            Name = cluster.Name,
            Centroid = cluster.Centroid,
            Vectors = cluster.Vectors,
            SumOfSquaredErrors = cluster.SumOfSquaredErrors
        }));

        return new ResultDTO()
        {
            Clusters = clustersDTO,
            History = _history,
            Iterations = _finishedIterations
        };
    }
    
}