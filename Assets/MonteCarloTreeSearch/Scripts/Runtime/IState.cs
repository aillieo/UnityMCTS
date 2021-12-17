using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public interface IState<T> where T : IState<T>
    {
        IEnumerable<T> Expand();

        float Simulate(IAgent agent);

        bool IsTerminal();

        void CopyFrom(T src);

        void Reset();
    }
}
