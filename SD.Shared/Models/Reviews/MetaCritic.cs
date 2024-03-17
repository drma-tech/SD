namespace SD.Shared.Models.Reviews
{
    public class DataMC
    {
        public Title? title { get; set; }
    }

    public class Edge
    {
        public Node? node { get; set; }
    }

    public class Metacritic
    {
        public string? __typename { get; set; }
        public Metascore? metascore { get; set; }
        public string? url { get; set; }
        public Reviews? reviews { get; set; }
    }

    public class Metascore
    {
        public int reviewCount { get; set; }
        public int score { get; set; }
    }

    public class Node
    {
        public Quote? quote { get; set; }
        public string? reviewer { get; set; }
        public string? site { get; set; }
        public int score { get; set; }
        public string? url { get; set; }
    }

    public class Quote
    {
        public string? language { get; set; }
        public string? value { get; set; }
    }

    public class Reviews
    {
        public List<Edge> edges { get; set; } = [];
    }

    public class RootMetacritic
    {
        public DataMC? data { get; set; }
    }

    public class Title
    {
        public Metacritic? metacritic { get; set; }
    }
}