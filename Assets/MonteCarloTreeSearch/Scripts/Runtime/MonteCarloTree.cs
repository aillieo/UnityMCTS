using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public class MonteCarloTree
    {
        // Selection
        // Expansion
        // Simulation
        // Back propagation

        public Node Selection(Node node)
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

        public void Expansion(Node node)
        {
            throw new NotImplementedException();
        }

        public void Simulation(Node node)
        {
            throw new NotImplementedException();
        }

        public void BackPropagation(Node node)
        {
            throw new NotImplementedException();
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
