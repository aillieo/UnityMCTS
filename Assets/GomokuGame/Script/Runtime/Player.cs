using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AillieoUtils.Gomoku
{
    public enum PlayerSide
    {
        Black = 0,
        White,
    }

    public abstract class Player
    {
        public PlayerSide side { get; protected internal set; }
        public GomokuGame belongingGame { get; protected internal set; }

        public abstract Task<int> Play();

        public abstract float maxOperationSeconds { get; }
    }
}
