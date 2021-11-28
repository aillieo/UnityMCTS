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
                    depth = 0,
                },
            };
        }

        public Node Run(int time)
        {
            if (root == null)
            {
                throw new Exception();
            }

            if (root.state.IsTerminal())
            {
                return root;
            }

            int rest = time;

            while (rest-- > 0)
            {
                Node child = Selection(root);

                if (child.simulateTimes != 0)
                {
                    child = Expansion(child);
                }

                UnityEngine.Debug.Log($"Simu in Run ({rest}/{time})");

                Simulation(child);
            }

            return Selection(root);
        }

        public static Node Selection(Node node)
        {
            UnityEngine.Debug.Log($"Selection: {node}");
            while (node.children != null && node.children.Count > 0)
            {
                node = SelectChild(node);

                if (node.depth > 225)
                {
                    throw new Exception("too deep");
                }
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
            UnityEngine.Debug.Log($"Expansion: {node}");

            if (node.state.IsTerminal())
            {
                return node;
            }

            foreach (var s in node.state.Expand())
            {
                node.children.Add(new Node()
                {
                    state = s,
                    parent = node,
                    depth = node.depth + 1,
                });
            }

            return node.children.FirstOrDefault();
        }

        public static void Simulation(Node node)
        {
            UnityEngine.Debug.Log($"Simulation: {node}");

            node.value += node.state.Simulate();
            node.simulateTimes++;
        }

        public static void BackPropagation(Node node)
        {
            UnityEngine.Debug.Log($"BackPropagation: {node}");

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
