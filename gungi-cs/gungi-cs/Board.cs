using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gungi_cs
{
    class Board
    {
        Constants constants;
        Array array;

        Piece[,] pieces;
        int[] pieces_in_hand, pieces_on_board;
        int[,,] board;

        int turn;
        bool[] done_initial_phase;

        bool[] options;

        public static void Main(String[] args)
        {
            Board board = new Board();
            board.Init();

            board.InitPieces(P.BL);
            board.InitPieces(P.WH);

            board.UpdatePieceCounters();

            while(true) board.Turn();
        }

        private void Init()
        {
            constants = new Constants();
            array = new Array();

            pieces = new Piece[2, 38];
            pieces_on_board = new int[2];
            pieces_in_hand = new int[] { P.MAX_P, P.MAX_P };
            board = new int[P.TM, P.RM, P.FM];

            turn = P.BL;
            done_initial_phase = new bool[2];

            ResetOptions();
        }

        private void ResetOptions()
        {
            options = new bool[5];
        }

        private void UpdatePieceCounters()
        {
            pieces_on_board[P.BL] = 0;
            pieces_on_board[P.WH] = 0;
            pieces_in_hand[P.WH] = 0;
            pieces_in_hand[P.WH] = 0;

            foreach (Piece piece in pieces)
            {
                if (piece.InHand())
                {
                    pieces_in_hand[piece.Color()]++;
                }
                else if (piece.OnBoard())
                {
                    pieces_on_board[piece.Color()]++;
                }
            }
        }

        private void Turn()
        {
            Console.WriteLine("Turn: " + turn);
            UpdateBoard();
            array.Update(turn, board);

            ResetOptions();
            if (!done_initial_phase[P.BL] || !done_initial_phase[P.WH])
            {
                InitialTurn();
            }
            else
            {
                NextTurn();
            }
            Choose();

            turn = (turn == P.BL) ? P.WH : P.BL;
        }

        private void UpdateBoard()
        {
            foreach (Piece piece in pieces)
            {
                if (piece.OnBoard())
                {
                    board[piece.Location()[P.T], piece.Location()[P.R], piece.Location()[P.F]] = piece.Type() * ((piece.Color() == P.BL) ? -1 : 1);
                }
            }
        }

        private void InitialTurn()
        {
            if (done_initial_phase[turn]) return;

            if (pieces_on_board[turn] < P.MIN_P)
            {
                options[P.DROP] = true;

            }
            else if (pieces_on_board[turn] >= P.MAX_P)
            {
                done_initial_phase[turn] = true;
            }
            else
            {
                options[P.DROP] = true;
                options[P.PASS] = true;
                options[P.DONE] = true;
            }
        }

        private void NextTurn()
        {
            if (pieces_in_hand[turn] > 0 && pieces_on_board[turn] < P.MAX_P)
            {
                options[P.DROP] = true;
            }
            if (pieces_on_board[turn] > 0)
            {
                options[P.MOVE] = true;
                options[P.MOVE] = true;
            }
        }

        private void Choose()
        {
            Console.Write("Options: ");
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i]) Console.Write(i + " | ");
            }
            Console.WriteLine();

            Console.Write("Select a choice: ");
            int _choice = Convert.ToInt32(Console.ReadLine());
            while (!options[_choice])
            {
                _choice = Convert.ToInt32(Console.ReadLine());
            }

            if (_choice == 0)
            {
                Console.WriteLine("Passed turn.");
            }
            else if (_choice == 1)
            {
                Console.WriteLine("Done setting up.");
                done_initial_phase[turn] = true;
            }
            else if (_choice == 2)
            {
                Console.WriteLine("Drop selected.");
                Drop();
            }
            else if (_choice == 3)
            {
                Console.WriteLine("Move selected.");
                Move();
            }
            else if (_choice == 4)
            {
                Console.WriteLine("Attack selected.");
                Attack();
            }
        }

        private void Drop()
        {
            Console.Write("Select a piece to drop: ");
            int _type = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Select a coordinate: ");
            Console.Write("Tier: ");
            int _tier = Convert.ToInt32(Console.ReadLine());
            Console.Write("Rank: ");
            int _rank = Convert.ToInt32(Console.ReadLine());
            Console.Write("File: ");
            int _file = Convert.ToInt32(Console.ReadLine());

            foreach (Piece piece in pieces)
            {
                if (piece.Type() == _type && piece.Color() == turn && piece.InHand())
                {
                    piece.Drop(_tier, _rank, _file);
                    AddToBoard(piece);
                    return;
                }
            }
        }

        private void Move()
        {
            Console.Write("Select a piece to move: ");
            Console.Write("Tier: ");
            int _c_tier = Convert.ToInt32(Console.ReadLine());
            Console.Write("Rank: ");
            int _c_rank = Convert.ToInt32(Console.ReadLine());
            Console.Write("File: ");
            int _c_file = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Select a coordinate: ");
            Console.Write("Tier: ");
            int _tier = Convert.ToInt32(Console.ReadLine());
            Console.Write("Rank: ");
            int _rank = Convert.ToInt32(Console.ReadLine());
            Console.Write("File: ");
            int _file = Convert.ToInt32(Console.ReadLine());

            int[] c_location = new int[] { _c_tier, _c_rank, _c_file };

            foreach (Piece piece in pieces)
            {
                if (piece.Location() == c_location)
                {
                    piece.MoveTo(_tier, _rank, _file);
                    AddToBoard(piece);
                    return;
                }
            }
        }

        private void Attack()
        {
            Console.WriteLine("Unimplemented.");
        }

        private void AddToBoard(Piece _piece)
        {
            array.AddPiece(turn, _piece);
        }

        private void InitPieces(int _player)
        {
            pieces[_player, 0] = new Piece(_player, P.MAR);
            pieces[_player, 1] = new Piece(_player, P.SPY);
            pieces[_player, 2] = new Piece(_player, P.SPY);
            pieces[_player, 3] = new Piece(_player, P.LIE);
            pieces[_player, 4] = new Piece(_player, P.LIE);
            pieces[_player, 5] = new Piece(_player, P.MAJ);
            pieces[_player, 6] = new Piece(_player, P.MAJ);
            pieces[_player, 7] = new Piece(_player, P.MAJ);
            pieces[_player, 8] = new Piece(_player, P.MAJ);
            pieces[_player, 9] = new Piece(_player, P.GEN);
            pieces[_player, 10] = new Piece(_player, P.GEN);
            pieces[_player, 11] = new Piece(_player, P.GEN);
            pieces[_player, 12] = new Piece(_player, P.GEN);
            pieces[_player, 13] = new Piece(_player, P.GEN);
            pieces[_player, 14] = new Piece(_player, P.GEN);
            pieces[_player, 15] = new Piece(_player, P.ARC);
            pieces[_player, 16] = new Piece(_player, P.ARC);
            pieces[_player, 17] = new Piece(_player, P.KNI);
            pieces[_player, 18] = new Piece(_player, P.KNI);
            pieces[_player, 19] = new Piece(_player, P.SAM);
            pieces[_player, 20] = new Piece(_player, P.SAM);
            pieces[_player, 21] = new Piece(_player, P.CAN);
            pieces[_player, 22] = new Piece(_player, P.CAN);
            pieces[_player, 23] = new Piece(_player, P.COU);
            pieces[_player, 24] = new Piece(_player, P.COU);
            pieces[_player, 25] = new Piece(_player, P.FOR);
            pieces[_player, 26] = new Piece(_player, P.FOR);
            pieces[_player, 27] = new Piece(_player, P.MUS);
            pieces[_player, 28] = new Piece(_player, P.MUS);
            pieces[_player, 29] = new Piece(_player, P.PAW);
            pieces[_player, 30] = new Piece(_player, P.PAW);
            pieces[_player, 31] = new Piece(_player, P.PAW);
            pieces[_player, 32] = new Piece(_player, P.PAW);
            pieces[_player, 33] = new Piece(_player, P.PAW);
            pieces[_player, 34] = new Piece(_player, P.PAW);
            pieces[_player, 35] = new Piece(_player, P.PAW);
            pieces[_player, 36] = new Piece(_player, P.PAW);
            pieces[_player, 37] = new Piece(_player, P.PAW);
        }
    }
}
