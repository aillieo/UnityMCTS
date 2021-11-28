using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AillieoUtils.GoBang
{
    public enum BoardValue : byte
    {
        Empty = 0,
        Black,
        White
    }

    public class GoBangGame
    {
        public const int dimension = 8;

        private GoBangState current;
        private Dictionary<PlayerSide, Player> players = new Dictionary<PlayerSide, Player>();

        public T CreatePlayer<T>() where T : Player
        {
            if (!players.TryGetValue(PlayerSide.Black, out Player p1))
            {
                p1 = Activator.CreateInstance<T>();
                players[PlayerSide.Black] = p1;

                p1.belongingGame = this;
                p1.side = PlayerSide.Black;

                return p1 as T;
            }

            if (!players.TryGetValue(PlayerSide.White, out Player p2))
            {
                p2 = Activator.CreateInstance<T>();
                players[PlayerSide.White] = p2;

                p2.belongingGame = this;
                p2.side = PlayerSide.White;

                return p2 as T;
            }

            return null;
        }

        public void Init()
        {
            current = new GoBangState();
            current.side = PlayerSide.Black;
        }

        public GoBangState GetCurrentState()
        {
            return current;
        }

        public async Task<Player> Run()
        {
            GoBangState state = GetCurrentState();
            while (!state.IsTerminal())
            {
                state = await PlayerMove();
            }

            Player winner = players[Flip(state.side)];
            return winner;
        }

        public async Task<GoBangState> PlayerMove()
        {
            Player curPlayer = players[current.side];

            Task<int> playTask = curPlayer.Play();

            //playTask.Wait();

            await Task.WhenAny(playTask, Task.Delay(10000));

            if (!playTask.IsCompleted)
            {
                throw new TimeoutException();
            }

            int index = await playTask;
            current.lastPlaced = index;

            if (current.boardState[index] != BoardValue.Empty)
            {
                throw new InvalidOperationException();
            }

            BoardValue value = PlayerSideToBoardValue(current.side);
            current.boardState[index] = value;
            current.side = Flip(current.side);

            return current;
        }

        public void DrawGizmos()
        {
            GoBangState state = GetCurrentState();

            if (state == null)
            {
                return;
            }

            Vector3 center = new Vector3(GoBangGame.dimension / 2, 0, GoBangGame.dimension / 2);
            Vector3 size = new Vector3(GoBangGame.dimension, 1, GoBangGame.dimension);
            Gizmos.DrawWireCube(center, size);

            for (int y = 0; y < GoBangGame.dimension; ++y)
            {
                for (int x = 0; x < GoBangGame.dimension; ++x)
                {
                    BoardValue value = state.boardState[x + y * GoBangGame.dimension];
                    if (value != BoardValue.Empty)
                    {
                        DrawPiece(value, x, y);
                    }
                }
            }
        }

        private void DrawPiece(BoardValue value, int posX, int posY)
        {
            Color color = Gizmos.color;

            switch (value)
            {
            case BoardValue.Black:
                Gizmos.color = Color.black;
                break;
            case BoardValue.White:
                Gizmos.color = Color.white;
                break;
            default:
                break;
            }

            Vector3 center = new Vector3(posX, 0, posY);
            Vector3 size = Vector3.one * 0.2f;
            Gizmos.DrawCube(center, size);

            Gizmos.color = color;
        }

        public static int PosToIndex(int x, int y)
        {
            if (!ValidPos(x, y))
            {
                throw new IndexOutOfRangeException($"x={x},y={y}");
            }
            return x + y * dimension;
        }

        public static Vector2Int IndexToPos(int index)
        {
            if (!ValidIndex(index))
            {
                throw new IndexOutOfRangeException($"index={index}");
            }
            int x = index % dimension;
            int y = index / dimension;
            return new Vector2Int(x, y);
        }

        public static BoardValue PlayerSideToBoardValue(PlayerSide playerSide)
        {
            if (playerSide == PlayerSide.Black)
            {
                return BoardValue.Black;
            }
            else
            {
                return BoardValue.White;
            }
        }

        public static PlayerSide BoardValueToPlayerSide(BoardValue boardValue)
        {
            if (boardValue == BoardValue.Black)
            {
                return PlayerSide.Black;
            }
            else if (boardValue == BoardValue.White)
            {
                return PlayerSide.White;
            }

            throw new Exception();
        }

        public static PlayerSide Flip(PlayerSide current)
        {
            if (current == PlayerSide.Black)
            {
                return PlayerSide.White;
            }
            else
            {
                return PlayerSide.Black;
            }
        }

        public static bool ValidPos(int x, int y)
        {
            return x >= 0 && x < dimension && y >= 0 && y < dimension;
        }

        public static bool ValidIndex(int index)
        {
            return index >= 0 && index < dimension * dimension;
        }
    }
}
