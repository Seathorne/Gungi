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

        HashSet<Piece> pieces, board_pieces, hand_pieces, captured_pieces;
        Piece selected_piece;
        int[] num_pieces_on_board, num_pieces_in_hand;

        int turn;
        bool[] done_initial_phase;

        bool[] options;

        public static void Main(String[] args)
        {
            Board board = new Board();

            board.Init();

            board.InitPieces(P.BL);
            board.InitPieces(P.WH);

            board.ResetOptions();

            while (true) board.Turn();
        }

        private void Init()
        {
            constants = new Constants();
            array = new Array();

            pieces = new HashSet<Piece>();
            selected_piece = null;
            ClearBoard();

            turn = P.BL;
            done_initial_phase = new bool[2];
            options = new bool[5];
        }

        private void ClearBoard()
        {
            board_pieces = new HashSet<Piece>();
            hand_pieces = new HashSet<Piece>();
            captured_pieces = new HashSet<Piece>();

            num_pieces_on_board = new int[2];
            num_pieces_in_hand = new int[] { P.MAX_P, P.MAX_P };
        }

        private void ResetOptions()
        {
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = false;
            }
        }

        private void Turn()
        {
            Console.WriteLine("Turn: " + turn);
            UpdateBoard();

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
            SwapTurn();
        }

        private void SwapTurn()
        {
            turn = (turn == P.BL) ? P.WH : P.BL;
        }

        private void UpdateBoard()
        {
            ClearBoard();

            foreach (Piece p in pieces)
            {
                if (p.OnBoard())
                {
                    board_pieces.Add(p);
                    num_pieces_on_board[p.Color()]++;
                }
                else if(p.InHand())
                {
                    hand_pieces.Add(p);
                }
                else
                {
                    captured_pieces.Add(p);
                    num_pieces_in_hand[p.Color()]++;
                }
            }

            array.Update(turn, pieces, board_pieces);
        }

        private void InitialTurn()
        {
            if (done_initial_phase[turn]) return;

            if (num_pieces_on_board[turn] < P.MIN_P)
            {
                options[P.DROP] = true;

            }
            else if (num_pieces_on_board[turn] >= P.MAX_P)
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
            if (num_pieces_in_hand[turn] > 0 && num_pieces_on_board[turn] < P.MAX_P)
            {
                options[P.DROP] = true;
            }
            if (num_pieces_on_board[turn] > 0)
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
            while (_choice < 0 || _choice > options.Length - 1 || !options[_choice])
            {
                _choice = Convert.ToInt32(Console.ReadLine());
            }

            if (_choice == 0)
            {
                Console.WriteLine("Passed turn.");
                Pass();
            }
            else if (_choice == 1)
            {
                Console.WriteLine("Done setting up.");
                Done();
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

        private void Pass()
        {

        }

        private void Done()
        {
            done_initial_phase[turn] = true;
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

            foreach (Piece piece in hand_pieces)
            {
                if (piece.Type() == _type && piece.Color() == turn)
                {
                    piece.Drop(_tier, _rank, _file);
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

            foreach (Piece piece in board_pieces)
            {
                if (piece.Location() == c_location)
                {
                    piece.BoardMove(_tier, _rank, _file);
                    return;
                }
            }
        }

        private void Attack()
        {
            Console.WriteLine("Unimplemented.");
        }

        private void InitPieces(int _player)
        {
            pieces.Add(new Piece(_player, P.MAR));
            pieces.Add(new Piece(_player, P.SPY));
            pieces.Add(new Piece(_player, P.SPY));
            pieces.Add(new Piece(_player, P.LIE));
            pieces.Add(new Piece(_player, P.LIE));
            pieces.Add(new Piece(_player, P.MAJ));
            pieces.Add(new Piece(_player, P.MAJ));
            pieces.Add(new Piece(_player, P.MAJ));
            pieces.Add(new Piece(_player, P.MAJ));
            pieces.Add(new Piece(_player, P.GEN));
            pieces.Add(new Piece(_player, P.GEN));
            pieces.Add(new Piece(_player, P.GEN));
            pieces.Add(new Piece(_player, P.GEN));
            pieces.Add(new Piece(_player, P.GEN));
            pieces.Add(new Piece(_player, P.GEN));
            pieces.Add(new Piece(_player, P.ARC));
            pieces.Add(new Piece(_player, P.ARC));
            pieces.Add(new Piece(_player, P.KNI));
            pieces.Add(new Piece(_player, P.KNI));
            pieces.Add(new Piece(_player, P.SAM));
            pieces.Add(new Piece(_player, P.SAM));
            pieces.Add(new Piece(_player, P.CAN));
            pieces.Add(new Piece(_player, P.CAN));
            pieces.Add(new Piece(_player, P.COU));
            pieces.Add(new Piece(_player, P.COU));
            pieces.Add(new Piece(_player, P.FOR));
            pieces.Add(new Piece(_player, P.FOR));
            pieces.Add(new Piece(_player, P.MUS));
            pieces.Add(new Piece(_player, P.MUS));
            pieces.Add(new Piece(_player, P.PAW));
            pieces.Add(new Piece(_player, P.PAW));
            pieces.Add(new Piece(_player, P.PAW));
            pieces.Add(new Piece(_player, P.PAW));
            pieces.Add(new Piece(_player, P.PAW));
            pieces.Add(new Piece(_player, P.PAW));
            pieces.Add(new Piece(_player, P.PAW));
            pieces.Add(new Piece(_player, P.PAW));
            pieces.Add(new Piece(_player, P.PAW));
        }
    }
}
