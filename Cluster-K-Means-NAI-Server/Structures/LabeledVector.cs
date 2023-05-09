
namespace Cluster_K_Means_NAI_Server.Structures;

public class LabeledVector<T>  
{
    public string Label { get; private set; }
    public List<T> Attributes { get; private set; } = new List<T>();
    
    public LabeledVector(string label)
    {
        Label = label;
    }
    
    public LabeledVector(string label, IEnumerable<T> attributes)
    {
        Label = label;
        Attributes = attributes.ToList();
    }
    
    public LabeledVector(string label, params T[] attributes)
    {
        Label = label;
        Attributes = attributes.ToList();
    }
    
    public LabeledVector<T> Clone()
    {
        return new LabeledVector<T>(Label, Attributes);
    }
    
    public void AddAttribute(T attribute)
    {
        Attributes.Add(attribute);
    }
    
    public override string ToString()
    {
        return $"Vector with label \"{Label}\": ({string.Join(", ", Attributes)})";
    }

}