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

        private List<IState> cachedExpansion = new List<IState>();
        private GoBangStateWrapper cachedInitial;

        private int instanceId;
        private static int sid;

        public GoBangStateWrapper(GoBangState goBangState)
        {
            this.goBangState = goBangState;
            this.instanceId = sid++;
        }

        public GoBangStateWrapper()
            : this(new GoBangState())
        {
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
                                if (GoBangGame.ValidPos(i, j))
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
            if (cachedInitial == null)
            {
                cachedInitial = this.DeepCopy() as GoBangStateWrapper;
            }
            else
            {
                Copy(this, cachedInitial);
            }

            int simuTime = 1000;
            PlayerSide selfSide = cachedInitial.goBangState.side;
            while (!cachedInitial.IsTerminal())
            {
                if (simuTime-- <= 0)
                {
                    return 0.5f;
                }

                cachedExpansion.Clear();
                cachedExpansion.AddRange(Expand());
                int count = cachedExpansion.Count();
                IState rand = cachedExpansion[this.rand.Next(0, count)];
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
            GoBangStateWrapper newIns = new GoBangStateWrapper();
            Copy(this, newIns);
            return newIns;
        }

        public static void Copy(GoBangStateWrapper src, GoBangStateWrapper dst)
        {
            dst.goBangState.side = src.goBangState.side;
            Array.Copy(src.goBangState.boardState, dst.goBangState.boardState, src.goBangState.boardState.Length);
            dst.goBangState.lastPlaced = src.goBangState.lastPlaced;
        }

        public override string ToString()
        {
            return $"({instanceId}){base.ToString()}";
        }
    }
}
