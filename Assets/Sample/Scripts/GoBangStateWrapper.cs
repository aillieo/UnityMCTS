using AillieoUtils.GoBang;
using AillieoUtils.MonteCarloTreeSearch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sample
{
    public class GoBangStateWrapper : IState
    {
        public GoBangState goBangState;
        private Random rand = new Random();

        private List<IState> cachedSimu = new List<IState>();
        private GoBangStateWrapper cachedInitial = new GoBangStateWrapper(null);

        public GoBangStateWrapper(GoBangState goBangState)
        {
            this.goBangState = goBangState;
        }

        public IEnumerable<IState> Expand()
        {
            for (int x = 0; x < GoBangGame.dimension; ++x)
            {
                for (int y = 0; y < GoBangGame.dimension; ++y)
                {
                    if (goBangState.Get(x, y) == GoBangGame.PlayerSideToBoardValue(goBangState.side))
                    {
                        for (int i = x - 1; i <= x + 1; ++i)
                        {
                            for (int j = y - 1; j <= y + 1; ++j)
                            {
                                if (GoBangState.ValidPos(i, j))
                                {
                                    if (goBangState.Get(i, j) == BoardValue.Empty)
                                    {
                                        // 相邻位置 优先考虑落子
                                        GoBangStateWrapper newState = this.DeepCopy() as GoBangStateWrapper;
                                        newState.goBangState.Set(i, j, GoBangGame.PlayerSideToBoardValue(goBangState.side));
                                        newState.goBangState.lastPlaced = GoBangGame.PosToIndex(i, j);
                                        newState.goBangState.side = GoBangGame.Flip(newState.goBangState.side);
                                        yield return newState;
                                    }
                                }
                            }
                        }
                    }

                    // 其它位置 随机考虑落子
                    if (rand.NextDouble() > 0.6 && goBangState.Get(x, y) == BoardValue.Empty)
                    {
                        GoBangStateWrapper newState = this.DeepCopy() as GoBangStateWrapper;
                        newState.goBangState.Set(x, y, GoBangGame.PlayerSideToBoardValue(goBangState.side));
                        newState.goBangState.lastPlaced = GoBangGame.PosToIndex(x, y);
                        newState.goBangState.side = GoBangGame.Flip(newState.goBangState.side);
                        yield return newState;
                    }
                }
            }
        }

        public bool IsTerminal()
        {
            return goBangState.IsTerminal();
        }

        public float Simulate()
        {
            cachedInitial = this.DeepCopy() as GoBangStateWrapper;
            int simuTime = 5;
            PlayerSide selfSide = cachedInitial.goBangState.side;
            while (!cachedInitial.IsTerminal())
            {
                if (simuTime-- <= 0)
                {
                    return 0.5f;
                }

                cachedSimu.Clear();
                cachedSimu.AddRange(Expand());
                int count = cachedSimu.Count();
                IState rand = cachedSimu[this.rand.Next(0, count)];
                cachedInitial.goBangState = (rand as GoBangStateWrapper).goBangState;
            }

            if (cachedInitial.goBangState.side == GoBangGame.Flip(selfSide))
            {
                // win
                return 1f;
            }
            else
            {
                // lose
                return 0f;
            }
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
