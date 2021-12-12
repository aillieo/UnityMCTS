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
                                        yield return ApplyAction(this, action);
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
                            yield return ApplyAction(this, action);
                        }
                    }
                }
            }
        }

        public static GoBangStateWrapper ApplyAction(GoBangStateWrapper initial, int action)
        {
            GoBangStateWrapper newState = StatePool.GetState();
            newState.CopyFrom(initial);
            PlayerSide curSide = GoBangGame.Flip(newState.goBangState.lastPlacedSide);
            UnityEngine.Vector2Int pos = GoBangGame.IndexToPos(action);
            newState.goBangState.Set(pos.x, pos.y, GoBangGame.PlayerSideToBoardValue(curSide));
            newState.goBangState.lastPlaced = action;
            newState.goBangState.lastPlacedSide = curSide;
            newState.goBangState.step++;
            return newState;
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
            selfSide = PlayerSide.White; // todo

            while (!cachedInitial.IsTerminal())
            {
                if (simuTime-- <= 0)
                {
                    // timeout (draw)
                    return 0.5f;
                }

                List<GoBangStateWrapper> expansion = StatePool.GetStateList();
                expansion.AddRange(cachedInitial.Expand());
                int count = expansion.Count();
                if (count == 0)
                {
                    UnityEngine.Debug.LogError("s" + cachedInitial);
                    throw new InvalidOperationException();
                }

                GoBangStateWrapper rand = expansion[this.rand.Next(0, count)];
                cachedInitial.CopyFrom(rand);

                foreach (var s in expansion)
                {
                    StatePool.RecycleState(s);
                }

                StatePool.RecycleStateList(expansion);
            }

            if (!GoBangGame.ValidIndex(cachedInitial.goBangState.lastPlaced))
            {
                // draw
                return 0.5f;
            }

            if (cachedInitial.goBangState.lastPlacedSide == selfSide)
            {
                // win
                return 1f;
            }
            else
            {
                // lose
                return 0;
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
