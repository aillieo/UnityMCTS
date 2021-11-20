using AillieoUtils.MonteCarloTreeSearch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCase : MonoBehaviour
{
    void Start()
    {
        Node node = new Node();

        MonteCarloTree tree = new MonteCarloTree();

        tree.Selection(node);

        tree.Expansion(node);

        tree.Simulation(node);

        tree.BackPropagation(node);
    }
}
