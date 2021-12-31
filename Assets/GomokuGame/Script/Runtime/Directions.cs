using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AillieoUtils.Gomoku
{
    public static class Directions
    {
        public static readonly Vector2Int Left = Vector2Int.left;
        public static readonly Vector2Int Right = Vector2Int.right;
        public static readonly Vector2Int Up = Vector2Int.up;
        public static readonly Vector2Int Down = Vector2Int.down;
        public static readonly Vector2Int LeftUp = new Vector2Int(-1, 1);
        public static readonly Vector2Int RightDown = new Vector2Int(1, -1);
        public static readonly Vector2Int LeftDown = new Vector2Int(-1, -1);
        public static readonly Vector2Int RightUp = new Vector2Int(1, 1);

        private static readonly Vector2Int[] dirs = new Vector2Int[]
        {
            Left,
            Right,

            Up,
            Down,

            LeftUp,
            RightDown,

            LeftDown,
            RightUp,
        };

        //public static IEnumerable<Vector2Int> Enumerate()
        public static Vector2Int[] Enumerate()
        {
            //yield return Left;
            //yield return Right;

            //yield return Up;
            //yield return Down;

            //yield return LeftUp;
            //yield return RightDown;

            //yield return LeftDown;
            //yield return RightUp;

            return dirs;
        }
    }
}
