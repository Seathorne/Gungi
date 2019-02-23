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

        int turn_player, turn_number;
        bool[] done_initial_phase;

        bool[] options;

        public static void Main(String[] args)
        {
            Board board = new Board();

            board.Init();

            board.InitPieces(P.BLACK);
            board.InitPieces(P.WHITE);

            board.ResetOptions();

            board.UpdateBoard();
            while (true) board.Turn();
        }

        private void Init()
        {
            constants = new Constants();
            array = new Array();

            pieces = new HashSet<Piece>();
            selected_piece = null;
            ClearBoard();

            turn_player = P.BLACK;
            turn_number = 0;
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
            turn_number++;

            Console.WriteLine("Turn: " + turn_number + ", " + turn_player + "'s move");

            ResetOptions();
            if (!done_initial_phase[P.BLACK] || !done_initial_phase[P.WHITE])
            {
                InitialTurn();
            }
            else
            {
                NextTurn();
            }
            Choose();

            UpdateBoard();

            SwapTurn();
        }

        private void SwapTurn()
        {
            turn_player = (turn_player == P.BLACK) ? P.WHITE : P.BLACK;
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

            array.Update(turn_player, pieces, board_pieces);
        }

        private void InitialTurn()
        {
            if (done_initial_phase[turn_player]) return;

            if (num_pieces_on_board[turn_player] < P.MIN_P)
            {
                options[P.DROP] = true;

            }
            else if (num_pieces_on_board[turn_player] >= P.MAX_P)
            {
                done_initial_phase[turn_player] = true;
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
            if (num_pieces_in_hand[turn_player] > 0 && num_pieces_on_board[turn_player] < P.MAX_P)
            {
                options[P.DROP] = true;
            }
            if (num_pieces_on_board[turn_player] > 0)
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
            done_initial_phase[turn_player] = true;
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
                if (piece.Type() == _type && piece.Color() == turn_player)
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

        private void InitPieces(int _player_color)
        {
            pieces.Add(new Piece(_player_color, P.MAR));
            pieces.Add(new Piece(_player_color, P.SPY));
            pieces.Add(new Piece(_player_color, P.SPY));
            pieces.Add(new Piece(_player_color, P.LIE));
            pieces.Add(new Piece(_player_color, P.LIE));
            pieces.Add(new Piece(_player_color, P.MAJ));
            pieces.Add(new Piece(_player_color, P.MAJ));
            pieces.Add(new Piece(_player_color, P.MAJ));
            pieces.Add(new Piece(_player_color, P.MAJ));
            pieces.Add(new Piece(_player_color, P.GEN));
            pieces.Add(new Piece(_player_color, P.GEN));
            pieces.Add(new Piece(_player_color, P.GEN));
            pieces.Add(new Piece(_player_color, P.GEN));
            pieces.Add(new Piece(_player_color, P.GEN));
            pieces.Add(new Piece(_player_color, P.GEN));
            pieces.Add(new Piece(_player_color, P.ARC));
            pieces.Add(new Piece(_player_color, P.ARC));
            pieces.Add(new Piece(_player_color, P.KNI));
            pieces.Add(new Piece(_player_color, P.KNI));
            pieces.Add(new Piece(_player_color, P.SAM));
            pieces.Add(new Piece(_player_color, P.SAM));
            pieces.Add(new Piece(_player_color, P.CAN));
            pieces.Add(new Piece(_player_color, P.CAN));
            pieces.Add(new Piece(_player_color, P.COU));
            pieces.Add(new Piece(_player_color, P.COU));
            pieces.Add(new Piece(_player_color, P.FOR));
            pieces.Add(new Piece(_player_color, P.FOR));
            pieces.Add(new Piece(_player_color, P.MUS));
            pieces.Add(new Piece(_player_color, P.MUS));
            pieces.Add(new Piece(_player_color, P.PAW));
            pieces.Add(new Piece(_player_color, P.PAW));
            pieces.Add(new Piece(_player_color, P.PAW));
            pieces.Add(new Piece(_player_color, P.PAW));
            pieces.Add(new Piece(_player_color, P.PAW));
            pieces.Add(new Piece(_player_color, P.PAW));
            pieces.Add(new Piece(_player_color, P.PAW));
            pieces.Add(new Piece(_player_color, P.PAW));
            pieces.Add(new Piece(_player_color, P.PAW));
        }
    }
}
