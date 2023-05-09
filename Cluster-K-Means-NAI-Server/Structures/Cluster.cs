using Cluster_K_Means_NAI_Server.Algorithms;

namespace Cluster_K_Means_NAI_Server.Structures;

public class Cluster
{
    public string Name { get; private set; }
    public List<LabeledVector<double>> Vectors { get; private set; } = new();
    public LabeledVector<double> Centroid { get; private set; }
    public double SumOfSquaredErrors { get; private set; }

    public Cluster(string name, LabeledVector<double> centroid)
    {
        Name = name;
        Centroid = centroid;
    }
    

    public void addVector(LabeledVector<double> vector)
    {
        Vectors.Add(vector);
    }
    
    public void ClearCluster()
    {
        Vectors.Clear();
    }
    
    public double CalculateSumOfSquaredErrors()
    {
        var sum = Vectors.Sum(vector => KMeans.EuclideanDistance(vector, Centroid));

        return sum / Vectors.Count;
    }
    
    public bool IsCentroidChanged()
    {
        var newCentroid = UpdatedCentroid();
        var isCentroidChanged = !Centroid.Attributes.SequenceEqual(newCentroid.Attributes);
        Centroid = newCentroid;
        SumOfSquaredErrors = CalculateSumOfSquaredErrors();
        return isCentroidChanged;
    }

    public Cluster Copy()
    {
        return new Cluster(Name, Centroid.Clone())
        {
            Vectors = Vectors.Select(vector => vector.Clone()).ToList(),
            SumOfSquaredErrors = SumOfSquaredErrors
        };
    }

    private LabeledVector<double> UpdatedCentroid()
    {
        var mostCommonLabel = GetMostCommonLabel();
        var dimensions = Vectors[0].Attributes.Count;
        var newCentroid = new LabeledVector<double>(mostCommonLabel);
        
        for (var i = 0; i < dimensions; i++)
        {
            var sum = Vectors.Sum(vector => vector.Attributes[i]);
            var average = sum / Vectors.Count;
            newCentroid.AddAttribute(average);
        }
        
        return newCentroid;
    }

    private string GetMostCommonLabel()
    {
        return Vectors.GroupBy(element => element.Label)
            // sortujemy najpierw po ilości elementów w grupie, a potem po nazwie grupy
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key)
            // na końcu bierzemy klucz pierwszego zgrupowania
            .First().Key;
    }
    
    public override string ToString()
    {
        return $"Cluster {Name}: ({string.Join(", ", Vectors)})";
    }
}