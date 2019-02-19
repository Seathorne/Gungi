using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    public interface IPiece
    {

        void SetMovements();

        void SetProperties();


        void CalcMoves(int[][][] _open_tiles);

        void CalcAttacks(int[][][] _open_tiles);

        void CalcDrops(int[][][] _open_tiles);


        void MoveTo(int _rank, int _file, int _tier);


        int GetPlayer();

        int[][][] GetMoves();

        int[][][] GetAttacks();

        int[] GetLocation();

    }
}
