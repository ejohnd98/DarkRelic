using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Utils;

public class PathResult {
    public bool validPath = false;
    public List<Vector2Int> steps; //should not include starting spot
    int index = 0;

    public bool HasNextStep(){
        return index < steps.Count;
    }

    public Vector2Int AdvanceStep(){
        Vector2Int next = steps[index];
        index++;
        return next;
    }
}

public class Pathfinding {
    public static PathResult FindPath(DR_Map map, Vector2Int a, Vector2Int b){
        PathResult result = new PathResult();

        //TODO: pathfinding code (A*)
        // Setup data structures
        Vector2Int size = map.MapSize;
        int w = size.x;
        int h = size.y;

        bool [,] visited = new bool[size.y, size.x];
        int [,] dist = new int[size.y, size.x];
        Vector2Int [,] cameFrom = new Vector2Int[size.y, size.x];

        PriorityQueue<Vector2Int, int> pq = new PriorityQueue<Vector2Int, int>();

        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                dist[y,x] = 999999;
                visited[y,x] = false;
                cameFrom[y,x] = -Vector2Int.one;
            }
        }

        //keep in mind pq will always remove the item with LOWEST priority value
        pq.Enqueue(a, 0); //add start to queue
        dist[a.y,a.x] = 0;
        visited[a.y,a.x] = true;
        cameFrom[a.y,a.x] = a;

        Vector2Int curr;
        bool pathFound = false;
        //bool actorsBlock = false;
        while (pq.Count > 0) {
            curr = pq.Dequeue(); //get Vector2Int on top of queue

            if (curr == b) {
                pathFound = true;
                break;
            }

            for (int i = 0; i < 4; i++) { //loop through adjacent cells 
                Vector2Int adj = curr;
                switch (i) {
                case 0:
                    adj.x++; break; //only add if within bounds
                case 1:
                    adj.x--; break;
                case 2:
                    adj.y++; break;
                case 3:
                    adj.y--; break;
                }

                if (map.ValidPosition(adj) && (!map.BlocksMovement(adj, true) || adj == b)) { //if space is free (true = wall)
                    if ((!visited[adj.y,adj.x] || dist[adj.y,adj.x] + 1 < dist[adj.y,adj.x])) { //check if new or shorter path
                        cameFrom[adj.y,adj.x] = curr; //set path followed to get here
                        dist[adj.y,adj.x] = dist[curr.y,curr.x] + 1; //dist is one more than parent (as we are working with grids)
                        int priority = (dist[adj.y,adj.x] + GetHeuristic(adj, b)); //set priority to dist + heuristicdd to queue
                        pq.Enqueue(adj, priority);
                        visited[adj.y,adj.x] = true; //set as visited
                        //actorsBlock |= map.BlocksMovement(adj);
                    }
                }
            }

        }
        if (pathFound) { //reconstruct path
            int length = 0;
            result.steps = new List<Vector2Int>();
            curr = b;
            while (curr != a) {
                result.steps.Add(curr);
                length++;
                curr = cameFrom[curr.y,curr.x];
                if (curr == -Vector2Int.one) {
                    Debug.LogError("ERROR in pathfinding reconstruction");
                }
            }
            result.validPath = true;
            result.steps.Reverse();
        }
        else {
            Debug.LogWarning("Path not found from: " + a.x + ", " + a.y + " to " + b.x + ", " + b.y);
        }

        return result;
    }

    static int GetHeuristic(Vector2Int a, Vector2Int b) { //get distance between two positions
        int dx = b.x - a.x;
        int dy = b.y - a.y;
        return Mathf.Abs(dx) + Mathf.Abs(dy);//sqrt((dx * dx) + (dy * dy));
    }
}
