using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AillieoUtils.Gomoku
{
    public class GomokuState
    {
        public int step;
        public int lastPlaced = -1;
        public readonly BoardValue[] boardState = new BoardValue[GomokuGame.dimension * GomokuGame.dimension];
        public PlayerSide lastPlacedSide = PlayerSide.Black;

        public bool IsTerminal()
        {
            if (!GomokuGame.ValidIndex(lastPlaced))
            {
                return false;
            }

            Vector2Int lastPlacedPos = GomokuGame.IndexToPos(lastPlaced);
            // horizontal
            int left = GetChainedCountByDir(lastPlacedPos, Vector2Int.left);
            if (left >= 4)
            {
                return true;
            }

            int right = GetChainedCountByDir(lastPlacedPos, Vector2Int.right);
            if (left + right >= 4)
            {
                return true;
            }

            // vertical
            int up = GetChainedCountByDir(lastPlacedPos, Vector2Int.up);
            if (up >= 4)
            {
                return true;
            }

            int down = GetChainedCountByDir(lastPlacedPos, Vector2Int.down);
            if (up + down >= 4)
            {
                return true;
            }

            // diagonal 1
            int lu = GetChainedCountByDir(lastPlacedPos, new Vector2Int(-1, 1));
            if (lu >= 4)
            {
                return true;
            }

            int rd = GetChainedCountByDir(lastPlacedPos, new Vector2Int(1, -1));
            if (lu + rd >= 4)
            {
                return true;
            }

            // diagonal 2
            int ld = GetChainedCountByDir(lastPlacedPos, new Vector2Int(-1, -1));
            if (ld >= 4)
            {
                return true;
            }

            int ru = GetChainedCountByDir(lastPlacedPos, new Vector2Int(1, 1));
            if (ld + ru >= 4)
            {
                return true;
            }

            //if (!boardState.Any(v => v == BoardValue.Empty))
            //{
            //    lastPlaced = -1;
            //    return true;
            //}
            // avoid GC
            bool anyEmpty = false;
            foreach (var value in boardState)
            {
                if (value == BoardValue.Empty)
                {
                    anyEmpty = true;
                    break;
                }
            }

            if (!anyEmpty)
            {
                lastPlaced = -1;
                return true;
            }

            return false;
        }

        private int GetChainedCountByDir(Vector2Int center, Vector2Int dir)
        {
            BoardValue centerValue = Get(center.x, center.y);
            int max = 0;
            Vector2Int pos = center;
            while (true)
            {
                pos = pos + dir;
                if (!GomokuGame.ValidPos(pos.x, pos.y))
                {
                    break;
                }

                if (Get(pos.x, pos.y) != centerValue)
                {
                    break;
                }

                ++max;
            }

            return max;
        }

        public BoardValue Get(int x, int y)
        {
            int index = GomokuGame.PosToIndex(x, y);
            return boardState[index];
        }

        public bool Set(int x, int y, BoardValue newValue)
        {
            int index = GomokuGame.PosToIndex(x, y);
            if (boardState[index] != BoardValue.Empty)
            {
                return false;
            }

            boardState[index] = newValue;
            return true;
        }
    }
}
