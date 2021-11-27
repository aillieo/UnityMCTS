using AillieoUtils.GoBang;
using AillieoUtils.MonteCarloTreeSearch;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sample
{
    public class GoBangStateWrapper : IState
    {
        public GoBangState goBangState;

        public GoBangStateWrapper(GoBangState goBangState)
        {
            this.goBangState = goBangState;
        }

        public IEnumerable<IState> Expand()
        {
            throw new System.NotImplementedException();
        }

        public bool IsTerminal()
        {
            return goBangState.IsTerminal();
        }

        public float Simulate()
        {
            throw new System.NotImplementedException();
        }

        public IState DeepCopy()
        {
            GoBangState newGoBangState = new GoBangState();
            newGoBangState.side = this.goBangState.side;
            Array.Copy(goBangState.boardState, newGoBangState.boardState, goBangState.boardState.Length);
            return new GoBangStateWrapper(newGoBangState);
        }
    }
}
