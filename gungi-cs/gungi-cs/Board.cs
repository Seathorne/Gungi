using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    class Board
    {
        public static void Main(String[] args)
        {
            Constants constants = new Constants();
            Array array = new Array();
            array.Update(P.BL, Constants.GetMoves(P.EMP));

            /*General test_general = new General(1, 1, 3, 5);

            Array.Print("Board", Constants.GetMoves(P.EMP));

            test_general.CalcMoves(Constants.GetMoves(P.EMP));
            Array.Print("Moves", test_general.GetMoves());
            Array.Print("Attacks", test_general.GetAttacks());*/
        }

        private void InitPieces(int _player)
        {

        }
    }
}
