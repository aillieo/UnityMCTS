using AillieoUtils.Gomoku;
using AillieoUtils.MonteCarloTreeSearch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample
{
    public class GomokuStateWrapper : IState<GomokuStateWrapper>
    {
        public GomokuState gomokuState;
        private Random rand = new Random();

        private int instanceId;
        private static int sid;

        public GomokuStateWrapper(GomokuState gomokuState)
        {
            this.gomokuState = gomokuState;
            this.instanceId = sid++;
        }

        public GomokuStateWrapper()
            : this(new GomokuState())
        {
        }

        public IEnumerable<GomokuStateWrapper> Expand()
        {
            //UnityEngine.Debug.Log($"Expand from:\n{this}");

            HashSet<int> cachedActions = StatePool.GetActionSet();
            for (int x = 0; x < GomokuGame.dimension; ++x)
            {
                for (int y = 0; y < GomokuGame.dimension; ++y)
                {
                    if (gomokuState.Get(x, y) != BoardValue.Empty)
                    {
                        for (int i = x - 1; i <= x + 1; ++i)
                        {
                            for (int j = y - 1; j <= y + 1; ++j)
                            {
                                if (i == x && j == y)
                                {
                                    continue;
                                }

                                if (GomokuGame.ValidPos(i, j) && gomokuState.Get(i, j) == BoardValue.Empty)
                                {
                                    int action = GomokuGame.PosToIndex(i, j);
                                    if (cachedActions.Add(action))
                                    {
                                        // 相邻位置 优先考虑落子
                                        yield return ApplyAction(this, action);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 其它位置 随机考虑落子
                        if (rand.NextDouble() > 0.99 /*&& gomokuState.Get(x, y) == BoardValue.Empty*/)
                        {
                            int action = GomokuGame.PosToIndex(x, y);
                            if (cachedActions.Add(action))
                            {
                                yield return ApplyAction(this, action);
                            }
                        }
                    }
                }
            }

            StatePool.RecycleActionSet(cachedActions);
        }

        public static GomokuStateWrapper ApplyAction(GomokuStateWrapper initial, int action)
        {
            GomokuStateWrapper newState = StatePool.GetState();
            newState.CopyFrom(initial);
            PlayerSide curSide = GomokuGame.Flip(newState.gomokuState.lastPlacedSide);
            UnityEngine.Vector2Int pos = GomokuGame.IndexToPos(action);
            newState.gomokuState.Set(pos.x, pos.y, GomokuGame.PlayerSideToBoardValue(curSide));
            newState.gomokuState.lastPlaced = action;
            newState.gomokuState.lastPlacedSide = curSide;
            newState.gomokuState.step++;
            return newState;
        }

        public bool IsTerminal()
        {
            return gomokuState.IsTerminal();
        }

        public float Simulate(IAgent agent)
        {
            GomokuStateWrapper cachedInitial = StatePool.GetState();
            cachedInitial.CopyFrom(this);

            int simuTime = 1000;
            PlayerSide selfSide = (agent as PlayerAI).side;
            //selfSide = PlayerSide.White; // todo

            while (!cachedInitial.IsTerminal())
            {
                if (simuTime-- <= 0)
                {
                    // timeout (draw)
                    return 0.5f;
                }

                List<GomokuStateWrapper> expansion = StatePool.GetStateList();
                expansion.AddRange(cachedInitial.Expand());
                int count = expansion.Count;
                if (count == 0)
                {
                    UnityEngine.Debug.LogError("s" + cachedInitial);
                    throw new InvalidOperationException();
                }

                GomokuStateWrapper randState = expansion[this.rand.Next(0, count)];
                cachedInitial.CopyFrom(randState);

                foreach (var s in expansion)
                {
                    StatePool.RecycleState(s);
                }

                StatePool.RecycleStateList(expansion);
            }

            if (!GomokuGame.ValidIndex(cachedInitial.gomokuState.lastPlaced))
            {
                // draw
                return 0.5f;
            }

            if (cachedInitial.gomokuState.lastPlacedSide == selfSide)
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
            stringBuilder.Append($"({instanceId}) lp={GomokuGame.IndexToPos(gomokuState.lastPlaced)} ls={gomokuState.lastPlacedSide}");
            stringBuilder.AppendLine();
            for (int i = GomokuGame.dimension - 1; i >= 0; --i)
            {
                for (int j = 0; j < GomokuGame.dimension; ++j)
                {
                    switch (gomokuState.Get(j, i))
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

        public void CopyFrom(GomokuStateWrapper src)
        {
            Array.Copy(src.gomokuState.boardState, gomokuState.boardState, gomokuState.boardState.Length);
            gomokuState.lastPlacedSide = src.gomokuState.lastPlacedSide;
            gomokuState.lastPlaced = src.gomokuState.lastPlaced;
            gomokuState.step = src.gomokuState.step;
        }

        public void Reset()
        {
        }
    }
}
