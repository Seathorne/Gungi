using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    public interface IPiece
    {

        void CalcMoves(int[,,] _board);

        void MoveTo(int _rank, int _file, int _tier);


        int GetPlayer();

        int[,,] GetMoves();

        int[,,] GetAttacks();

        int[] GetLocation();

    }
}
