using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public class MonteCarloTree
    {
        // Selection
        // Expansion
        // Simulation
        // Back propagation

        private Node root;

        private MonteCarloTree()
        {
        }

        public static MonteCarloTree CreateTree(IState state)
        {
            return new MonteCarloTree()
            {
                root = new Node()
                {
                    state = state,
                },
            };
        }

        public Node Run(int time)
        {
            while (time -- > 0)
            {

            }

            return default;
        }

        public static Node Selection(Node node)
        {
            while (node.children != null && node.children.Count > 0)
            {
                node = SelectChild(node);
            }

            return node;
        }

        private static Node SelectChild(Node node)
        {
            Node best = default;
            float max = float.MinValue;
            foreach (var n in node.children)
            {
                float uct = UCT(n);
                if (uct > max)
                {
                    max = uct;
                    best = n;
                }
            }

            return best;
        }

        public static Node Expansion(Node node)
        {
            if (node.state.IsTerminal())
            {
                return node;
            }

            foreach (var s in node.state.Expand())
            {
                node.children.Add(new Node() { state = s });
            }

            return node.children.FirstOrDefault();
        }

        public static void Simulation(Node node)
        {
            node.value += node.state.Simulate();
        }

        public static void BackPropagation(Node node)
        {
            while (node.parent != null)
            {
                node = node.parent;
            }
        }

        private static float UCT(Node node)
        {
            float exploit = node.value / node.simulateTimes;
            float explore = Mathf.Sqrt(Mathf.Log(node.parent.simulateTimes) / node.simulateTimes);
            float c = 1.41421356f;
            return exploit + c * explore;
        }
    }
}
