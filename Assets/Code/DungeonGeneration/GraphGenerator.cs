using System;
using System.Collections;
using System.Collections.Generic;
using EpForceDirectedGraph.cs;
using UnityEngine;

public class GraphGenerator : MonoBehaviour {

    public Transform forceDirectedGraphVisParent;
    [HideInInspector]
    public bool isGeneratingGraph = false;

    protected Graph m_fdgGraph;
    protected ForceDirected2D m_fdgPhysics;
    protected bool visualizeGeneration;

    public void GenerateGraph(bool visualize){
        visualizeGeneration = visualize;
        isGeneratingGraph = true;
        StartCoroutine(GenerateGraphCoroutuine());
    }

    public IEnumerator GenerateGraphCoroutuine() {

        m_fdgGraph = GetGraph();

        float stiffness = 50.76f;
        float repulsion = 70000.0f;
        float damping   = 0.2f;

        m_fdgPhysics
         = new(m_fdgGraph, // instance of Graph
                stiffness, // stiffness of the spring
                repulsion, // node repulsion rate 
                damping);  // damping rate 
        
        m_fdgPhysics.Threadshold = 150.0f;

        var m_fdgRenderer = new ForceDirectedGraphRenderer(m_fdgPhysics);
        m_fdgRenderer.parentTransform = forceDirectedGraphVisParent;

        for (int i = 0; i < 800 && !m_fdgPhysics.WithinThreashold; i++){
            m_fdgRenderer.Draw(0.2f);
            if (visualizeGeneration){
                Debug.Log("Total Energy: " + m_fdgPhysics.TotalEnergy);
                yield return new WaitForEndOfFrame();
            }
        }

        const float DegreesToRadians = (float)(180f / Math.PI);

        float ScoreEdges()
        {
            float score = 0;

            foreach (var edge in m_fdgGraph.edges)
            {
                var point1 = m_fdgPhysics.m_nodePoints[edge.Source.ID];
                var point2 = m_fdgPhysics.m_nodePoints[edge.Target.ID];

                float dx = Mathf.Abs(point2.position.x - point1.position.x);
                float dy = Mathf.Abs(point2.position.y - point1.position.y);

                if (dx == 0 || dy == 0)
                {
                    score += 1; // Perfectly horizontal or vertical
                }
                else
                {
                    float ratio = Math.Min(dx, dy) / Math.Max(dx, dy);
                    score += (1 - ratio); // Higher score for more horizontal/vertical edges
                }
            }

            return score;
        }

        void RotateGraph(float angleDegrees)
        {
            float angleRadians = angleDegrees / DegreesToRadians;
            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            foreach (var node in m_fdgGraph.nodes)
            {
                var point = m_fdgPhysics.m_nodePoints[node.ID];

                float xNew = point.position.x * cosAngle - point.position.y * sinAngle;
                float yNew = point.position.x * sinAngle + point.position.y * cosAngle;

                point.position.x = xNew;
                point.position.y = yNew;
            }
        }

        float RoundToInterval(float value, float interval)
        {
            return (float)Math.Round(value / interval) * interval;
        }

        void SnapPointsToGrid(){
            foreach (var node in m_fdgGraph.nodes)
            {
                var point = m_fdgPhysics.m_nodePoints[node.ID];

                point.position.x = RoundToInterval(point.position.x, 50.0f);
                point.position.y = RoundToInterval(point.position.y, 50.0f);
            }
        }
            
        int steps = 360;
        float bestScore = float.MinValue;
        float bestAngle = 0;

        Debug.Log("Starting rotation!");

        for (int i = 0; i < steps; i++)
        {
            float angle = i * (360f / steps);
            RotateGraph(angle);
            float score = ScoreEdges();

            //m_fdgRenderer.Draw(0.1f, false);
            // if (visualizeGeneration)
            //     yield return new WaitForEndOfFrame();

            RotateGraph(-angle); // Rotate back to original position

            if (score > bestScore)
            {
                bestScore = score;
                bestAngle = angle;
            }
        }

        // Apply the best rotation
        RotateGraph(bestAngle);

        Debug.Log("Best angle is " + bestAngle);
        m_fdgRenderer.Draw(0.1f, false);


        Debug.Log("Starting snapping!");
        if (visualizeGeneration)
            yield return new WaitForSeconds(1.0f);

        SnapPointsToGrid();
        m_fdgRenderer.Draw(0.1f, false);

        if (visualizeGeneration)
            yield return new WaitForSeconds(1.0f);
        m_fdgRenderer.Clear();

        if (DoAnyEdgesCross()){
            Debug.Log("Restarting due to edge crosses!");
            GenerateGraph(visualizeGeneration);
            yield break;
        }


        isGeneratingGraph = false;
        yield return null;

        bool DoAnyEdgesCross(){
            foreach (var edgeA in m_fdgGraph.edges){
                foreach (var edgeB in m_fdgGraph.edges){
                    if (DoEdgesCross(edgeA, edgeB)){
                        return true;
                    }
                }
            }
            return false;
        }

        bool DoEdgesCross(Edge a, Edge b){
            var a1 = m_fdgPhysics.m_nodePoints[a.Source.ID];
            var a2 = m_fdgPhysics.m_nodePoints[a.Target.ID];
            var b1 = m_fdgPhysics.m_nodePoints[b.Source.ID];
            var b2 = m_fdgPhysics.m_nodePoints[b.Target.ID];

            if (a.Source.ID == b.Source.ID || a.Source.ID == b.Target.ID ||
                a.Target.ID == b.Source.ID || a.Target.ID == b.Target.ID)
            {
                return false;
            }

            double denom = (b2.position.y - b1.position.y) * (a2.position.x - a1.position.x) - (b2.position.x - b1.position.x) * (a2.position.y - a1.position.y);
            if (denom == 0)
            {
                return false; // Parallel lines
            }

            double ua = ((b2.position.x - b1.position.x) * (a1.position.y - b1.position.y) - (b2.position.y - b1.position.y) * (a1.position.x - b1.position.x)) / denom;
            double ub = ((a2.position.x - a1.position.x) * (a1.position.y - b1.position.y) - (a2.position.y - a1.position.y) * (a1.position.x - b1.position.x)) / denom;

            bool edgesCross = (ua >= 0 && ua <= 1) && (ub >= 0 && ub <= 1);
            if (edgesCross){
                Debug.Log("edges cross");
            }
            return edgesCross;
        }
    }
    
    public MapLayout GetGraphAsMapLayout(Vector2Int mapSize, Vector2Int maxRoomSize){
        var layout = new MapLayout(mapSize);
        Vector2Int safeSize = mapSize - (maxRoomSize * 2);

        // Get bounding box (want to fit fdg inside of given bounds)
        float
            xMax = float.MinValue,
            xMin = float.MaxValue,
            yMax = float.MinValue,
            yMin = float.MaxValue;
        
        foreach(var point in m_fdgPhysics.m_nodePoints.Values)
        {
            xMax = Math.Max(xMax, point.position.x);
            xMin = Math.Min(xMin, point.position.x);
            yMax = Math.Max(yMax, point.position.y);
            yMin = Math.Min(yMin, point.position.y);
        }

        // Add some padding to width/height
        float fdgWidth = xMax - xMin;
        float fdgHeight = yMax - yMin;

        float graphScale = Mathf.Min(safeSize.x / fdgWidth, safeSize.y / fdgHeight) * 0.8f; //Scale down a little further to make sure everything fits

        foreach (var fdg_node in m_fdgGraph.nodes){
            var point = m_fdgPhysics.m_nodePoints[fdg_node.ID];
            var node = new MapLayoutNode
            {
                position = GraphPosToVector(point.position * graphScale) + (mapSize / 2), //scale, then offset into positive axis
                size = Vector2Int.one * 7, //TEMP
                label = fdg_node.Data.label,
                roomTag = fdg_node.Data.roomTag
            };
            layout.nodes.Add(node);
            layout.NodeDict[fdg_node] = node;
        }

        foreach (var fdg_edge in m_fdgGraph.edges){

            Tuple<MapLayoutNode, MapLayoutNode> edge = new(
                layout.NodeDict[fdg_edge.Source],
                layout.NodeDict[fdg_edge.Target] 
            );
            layout.connections.Add(edge);
        }
        

        return layout;
    }

    public static Vector2Int GraphPosToVector(AbstractVector pos){
        return new Vector2Int(
            Mathf.FloorToInt(pos.x), 
            Mathf.FloorToInt(pos.y));
    }

    private Graph GetGraph(){
        m_fdgGraph = new Graph();

        //TODO: try using initial position too

        Node CreateNode(string label, float mass = 1.0f) {
            NodeData data = new()
            {
                label = label,
                mass = UnityEngine.Random.Range(1.0f, 5.0f)
            };
            return m_fdgGraph.CreateNode(data);
        }

        Edge ConnectNodes(Node a, Node b) {
            EdgeData data = new()
            {
                label = a.Data.label + "-" + b.Data.label,
                length = UnityEngine.Random.Range(5.0f, 10.0f)
            };
            return m_fdgGraph.CreateEdge(a, b, data);
        }

        
        Node startNode = CreateNode("Start", 5.0f);
        startNode.Data.roomTag = RoomTag.START;
        Node endNode = CreateNode("End", 5.0f);
        endNode.Data.roomTag = RoomTag.END;

        Node L1 = CreateNode("L1");
        Node L2 = CreateNode("L2");
        Node L3 = CreateNode("L3");

        Node C1 = CreateNode("C1");
        Node C2 = CreateNode("C2");

        Node R1 = CreateNode("R1");
        Node R2 = CreateNode("R2");
        Node R3 = CreateNode("R3");
        
        m_fdgGraph.nodes[UnityEngine.Random.Range(0, m_fdgGraph.nodes.Count)].Data.mass = 50.0f;
        m_fdgGraph.nodes[UnityEngine.Random.Range(0, m_fdgGraph.nodes.Count)].Data.mass = 50.0f;
        m_fdgGraph.nodes[UnityEngine.Random.Range(0, m_fdgGraph.nodes.Count)].Data.mass = 50.0f;

        ConnectNodes(startNode, L1);
        ConnectNodes(L1, L2);
        ConnectNodes(L2, L3);
        ConnectNodes(L3, C2);

        ConnectNodes(startNode, R1);
        ConnectNodes(R1, R2);
        ConnectNodes(R2, R3);
        ConnectNodes(R3, C2);
        ConnectNodes(C2, endNode);

        ConnectNodes(L2, C1);
        ConnectNodes(R2, C1);

        return m_fdgGraph;
    }
}