using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public class Node
    {
        public IState state;

        public float value;

        public Node parent;
        public readonly List<Node> children = new List<Node>();

        public int simulateTimes;
        public int depth;
    }
}
