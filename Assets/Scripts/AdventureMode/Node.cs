using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    string name;
    public Vector2Int pos;
    List<float> angles;
    public List<Node> neighbors;

    public Node (Vector2Int n_pos) {
        name = "Node: " + n_pos;
        pos = n_pos;
        angles = new List<float>();
        neighbors = new List<Node>();
    }

    public void addNeighbor(Node n) {
        float ab = angleBetween(pos, n.pos);
        float dist = Vector2Int.Distance(pos, n.pos);
        for (int i = 0; i < angles.Count; i++) {
            if (ab == angleBetween(pos, neighbors[i].pos)) {
                if (dist < Vector2Int.Distance(pos, neighbors[i].pos)) {
                    angles.RemoveAt(i);
                    neighbors.RemoveAt(i);
                } else {
                    return;
                }
            }
        }
        angles.Add(ab);
        neighbors.Add(n);
    }

    public void print() {
        Debug.Log(name + " neighbors: ");
        for (int i = 0; i < neighbors.Count; i++) {
            Debug.Log("   " + neighbors[i].pos + " "  + angles[i]);
        }
    }

    float angleBetween(Vector2Int a, Vector2Int b) {
        Vector2Int dir = a - b;
        return Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
