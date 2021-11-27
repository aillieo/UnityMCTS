using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AillieoUtils.MonteCarloTreeSearch
{
    public interface IState
    {
        IEnumerable<IState> Expand();

        float Simulate();

        bool IsTerminal();

        IState DeepCopy();
    }
}
