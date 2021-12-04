using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public interface IState<T> where T : class, IState<T>, new()
    {
        IEnumerable<T> Expand();

        float Simulate();

        bool IsTerminal();

        void CopyFrom(T src);

        void Reset();
    }
}
