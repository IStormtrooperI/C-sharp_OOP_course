using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace FluentApi.Graph
{
    public enum NodeShape
    {
        Box,
        Ellipse
    }

    public interface IGraphBuilder
    {
        GraphEdgeBuilder AddEdge(string sourceNode, string destinationNode);

        GraphNodeBuilder AddNode(string nodeName);

        string Build();
    }

    public class DotGraphBuilder : IGraphBuilder
    {
        private Graph Graph { get; }

        private DotGraphBuilder(string graphName, bool directed) => 
            Graph = new Graph(graphName, directed, false);

        public GraphEdgeBuilder AddEdge(string sourceNode, string destinationNode) =>
            new GraphEdgeBuilder(Graph.AddEdge(sourceNode, destinationNode), this);

        public GraphNodeBuilder AddNode(string nodeName) => 
            new GraphNodeBuilder(Graph.AddNode(nodeName), this);

        public string Build() => 
            Graph.ToDotFormat();

        public static IGraphBuilder DirectedGraph(string graphName) => 
            new DotGraphBuilder(graphName, directed: true);

        public static IGraphBuilder UndirectedGraph(string graphName) => 
            new DotGraphBuilder(graphName, directed: false);
    }

    public class GraphBuilder : IGraphBuilder
    {
        protected IGraphBuilder Parent { get; }

        public GraphBuilder(IGraphBuilder parent) => 
            Parent = parent;

        public GraphEdgeBuilder AddEdge(string sourceNode, string destinationNode) =>
            Parent.AddEdge(sourceNode, destinationNode);

        public GraphNodeBuilder AddNode(string nodeName) => 
            Parent.AddNode(nodeName);

        public string Build() => 
            Parent.Build();
    }

    public class GraphEdgeBuilder : GraphBuilder
    {
        private GraphEdge Edge { get; }

        public GraphEdgeBuilder(GraphEdge edge, IGraphBuilder parent) : base(parent) => 
            Edge = edge;

        public IGraphBuilder With(Action<EdgeCommonAttributesConfig> attributesToApply)
        {
            attributesToApply(new EdgeCommonAttributesConfig(Edge));
            return Parent;
        }
    }

    public class GraphNodeBuilder : GraphBuilder
    {
        private GraphNode Node { get; }

        public GraphNodeBuilder(GraphNode node, IGraphBuilder parent) : base(parent) => 
            Node = node;

        public IGraphBuilder With(Action<NodeCommonAttributesConfig> attributesToApply)
        {
            attributesToApply(new NodeCommonAttributesConfig(Node));
            return Parent;
        }
    }

    public class CommonAttributesConfig<TConfig>
        where TConfig : CommonAttributesConfig<TConfig>
    {
        private IDictionary<string, string> Attributes { get; }

        public CommonAttributesConfig(IDictionary<string, string> attributes) => 
            Attributes = attributes;

        public TConfig Label(string label)
        {
            Attributes["label"] = label;
            return (TConfig)this;
        }

        public TConfig FontSize(float sizeInPt)
        {
            Attributes["fontsize"] = sizeInPt.ToString(CultureInfo.InvariantCulture);
            return (TConfig)this;
        }

        public TConfig Color(string color)
        {
            Attributes["color"] = color;
            return (TConfig)this;
        }
    }

    public class EdgeCommonAttributesConfig : CommonAttributesConfig<EdgeCommonAttributesConfig>
    {
        private GraphEdge Edge { get; }

        public EdgeCommonAttributesConfig(GraphEdge edge) : base(edge.Attributes) =>
            Edge = edge;

        public EdgeCommonAttributesConfig Weight(double weight)
        {
            Edge.Attributes["weight"] = weight.ToString(CultureInfo.InvariantCulture);
            return this;
        }
    }

    public class NodeCommonAttributesConfig : CommonAttributesConfig<NodeCommonAttributesConfig>
    {
        private GraphNode Node { get; }

        public NodeCommonAttributesConfig(GraphNode node) : base(node.Attributes) => 
            Node = node;

        public NodeCommonAttributesConfig Shape(NodeShape shape)
        {
            Node.Attributes["shape"] = shape.ToString().ToLowerInvariant();
            return this;
        }
    }
}