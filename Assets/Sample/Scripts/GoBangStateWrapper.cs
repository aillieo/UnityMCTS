using AillieoUtils.GoBang;
using AillieoUtils.MonteCarloTreeSearch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample
{
    public class GoBangStateWrapper : IState<GoBangStateWrapper>
    {
        public GoBangState goBangState;
        private Random rand = new Random();

        private List<GoBangStateWrapper> cachedExpansion = new List<GoBangStateWrapper>();
        private GoBangStateWrapper cachedInitial;
        private HashSet<int> cachedActions = new HashSet<int>();

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

        public IEnumerable<GoBangStateWrapper> Expand()
        {
            //UnityEngine.Debug.Log($"Expand from:\n{this}");

            cachedActions.Clear();
            for (int x = 0; x < GoBangGame.dimension; ++x)
            {
                for (int y = 0; y < GoBangGame.dimension; ++y)
                {
                    if (goBangState.Get(x, y) != BoardValue.Empty)
                    {
                        for (int i = x - 1; i <= x + 1; ++i)
                        {
                            for (int j = y - 1; j <= y + 1; ++j)
                            {
                                if (GoBangGame.ValidPos(i, j) && goBangState.Get(i, j) == BoardValue.Empty)
                                {
                                    int action = GoBangGame.PosToIndex(i, j);
                                    if (cachedActions.Add(action))
                                    {
                                        // 相邻位置 优先考虑落子
                                        GoBangStateWrapper newState = new GoBangStateWrapper();
                                        newState.CopyFrom(this);
                                        PlayerSide curSide = GoBangGame.Flip(newState.goBangState.lastPlacedSide);
                                        newState.goBangState.Set(i, j, GoBangGame.PlayerSideToBoardValue(curSide));
                                        newState.goBangState.lastPlaced = action;
                                        newState.goBangState.lastPlacedSide = curSide;
                                        newState.goBangState.step++;
                                        //UnityEngine.Debug.Log($"Expand to:\n{newState}");
                                        yield return newState;
                                    }
                                }
                            }
                        }
                    }

                    // 其它位置 随机考虑落子
                    if (rand.NextDouble() > 0.99 && goBangState.Get(x, y) == BoardValue.Empty)
                    {
                        int action = GoBangGame.PosToIndex(x, y);
                        if (cachedActions.Add(action))
                        {
                            GoBangStateWrapper newState = new GoBangStateWrapper();
                            newState.CopyFrom(this);
                            PlayerSide curSide = GoBangGame.Flip(newState.goBangState.lastPlacedSide);
                            newState.goBangState.Set(x, y, GoBangGame.PlayerSideToBoardValue(curSide));
                            newState.goBangState.lastPlaced = action;
                            newState.goBangState.lastPlacedSide = curSide;
                            newState.goBangState.step++;
                            //UnityEngine.Debug.Log($"Expand to:\n{newState}");
                            yield return newState;
                        }
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
                cachedInitial = new GoBangStateWrapper();
            }

            cachedInitial.CopyFrom(this);

            int simuTime = 1000;
            PlayerSide selfSide = GoBangGame.Flip(cachedInitial.goBangState.lastPlacedSide);
            selfSide = PlayerSide.White;

            while (!cachedInitial.IsTerminal())
            {
                if (simuTime-- <= 0)
                {
                    return 0;
                }

                cachedExpansion.Clear();
                cachedExpansion.AddRange(cachedInitial.Expand());
                int count = cachedExpansion.Count();
                if (count == 0)
                {
                    UnityEngine.Debug.LogError("s" + cachedInitial);
                    throw new InvalidOperationException();
                }

                GoBangStateWrapper rand = cachedExpansion[this.rand.Next(0, count)];
                cachedInitial.CopyFrom(rand);
            }

            int steps = cachedInitial.goBangState.step - goBangState.step;

            if (cachedInitial.goBangState.lastPlacedSide == selfSide)
            {
                // win
                return 10f / steps;
            }
            else
            {
                // lose
                return -10f / steps;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"({instanceId}) lp={GoBangGame.IndexToPos(goBangState.lastPlaced)} ls={goBangState.lastPlacedSide}");
            stringBuilder.AppendLine();
            for (int i = GoBangGame.dimension - 1; i >= 0; --i)
            {
                for (int j = 0; j < GoBangGame.dimension; ++j)
                {
                    switch (goBangState.Get(j, i))
                    {
                        case BoardValue.Empty:
                            stringBuilder.Append('_');
                            break;
                        case BoardValue.Black:
                            stringBuilder.Append('x');
                            break;
                        case BoardValue.White:
                            stringBuilder.Append('o');
                            break;
                    }

                    stringBuilder.Append(' ');
                    stringBuilder.Append(' ');
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public void CopyFrom(GoBangStateWrapper src)
        {
            Array.Copy(src.goBangState.boardState, goBangState.boardState, goBangState.boardState.Length);
            goBangState.lastPlacedSide = src.goBangState.lastPlacedSide;
            goBangState.lastPlaced = src.goBangState.lastPlaced;
            goBangState.step = src.goBangState.step;
        }

        public void Reset()
        {
        }
    }
}
