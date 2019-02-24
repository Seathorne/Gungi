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

        HashSet<Piece> all_pieces, all_board_pieces, all_hand_pieces;
        HashSet<Piece>[] board_pieces, hand_pieces, captured_pieces;
        bool[] any_drops, any_moves, any_attacks;

        int turn_player_color, turn_number;
        bool[] done_with_initial_phase;
        bool[] just_passed;

        HashSet<int> options;

        public static void Main(String[] args)
        {
            Board board = new Board();

            board.constants = new Constants();
            board.array = new Array();

            board.Init();

            board.InitPieces(P.BLACK);
            board.InitPieces(P.WHITE);

            board.ClearBoard();

            while (true) board.Turn();
        }

        private void Init()
        {
            all_pieces = new HashSet<Piece>();

            turn_player_color = P.BLACK;
            turn_number = 1;

            done_with_initial_phase = new bool[2];
            just_passed = new bool[2];
        }

        private void ClearBoard()
        {
            all_board_pieces = new HashSet<Piece>();
            all_hand_pieces = new HashSet<Piece>();

            board_pieces = new HashSet<Piece>[] { new HashSet<Piece>(), new HashSet<Piece>() };
            hand_pieces = new HashSet<Piece>[] { new HashSet<Piece>(), new HashSet<Piece>() };
            captured_pieces = new HashSet<Piece>[] { new HashSet<Piece>(), new HashSet<Piece>() };
        }

        private void TurnReset()
        {
            any_drops = new bool[2];
            any_moves = new bool[2];
            any_attacks = new bool[2];

            options = new HashSet<int>();
        }

        private void Turn()
        {
            Console.WriteLine("Turn #" + turn_number + ": " + P.ConvertColor(turn_player_color) + "'s turn.");

            TurnReset();

            UpdateBoard();

            GetOptions();
            Choose();

            SwapTurn();
        }

        private void SwapTurn()
        {
            turn_player_color = (turn_player_color == P.BLACK) ? P.WHITE : P.BLACK;
            turn_number++;
        }

        private void UpdateBoard()
        {
            ClearBoard();

            foreach (Piece p in all_pieces)
            {
                if (p.OnBoard())
                {
                    all_board_pieces.Add(p);
                    board_pieces[p.PlayerColor()].Add(p);
                }
                else if(p.InHand())
                {
                    all_hand_pieces.Add(p);
                    hand_pieces[p.PlayerColor()].Add(p);
                }
                else
                {
                    captured_pieces[p.PlayerColor()].Add(p);
                }
            }

            UpdateArray();

            int[,,] blank = new int[P.TM, P.RM, P.FM];

            foreach (Piece p in all_hand_pieces)
            {
                if (hand_pieces[p.PlayerColor()].Count > 0 && p.Drops() != blank) any_drops[p.PlayerColor()] = true;
            }
            foreach (Piece p in all_board_pieces)
            {
                if (board_pieces[p.PlayerColor()].Count > 0 && p.Moves() != blank) any_moves[p.PlayerColor()] = true;
                if (board_pieces[p.PlayerColor()].Count > 0 && board_pieces[1- p.PlayerColor()].Count > 0 && p.Attacks() != blank) any_attacks[p.PlayerColor()] = true;
            }
        }

        private void UpdateArray()
        {
            array.Update(all_board_pieces, all_hand_pieces);
        }

        private void PrintArray()
        {
            array.PrintBoard();
        }

        private void PrintSelection(Piece _selected_piece)
        {
            array.Select(_selected_piece);
            PrintArray();
        }

        private void Deselect()
        {
            array.Deselect();
        }

        private void GetOptions()
        {
            options.Add(P.SELECT);

            if (!done_with_initial_phase[P.BLACK] || !done_with_initial_phase[P.WHITE])
            {
                if (board_pieces[turn_player_color].Count >= P.MAX_P)
                {
                    options.Add(P.DONE);
                }
                else if (board_pieces[turn_player_color].Count < P.MIN_P)
                {
                    options.Add(P.DROP);
                }
                else {
                    options.Add(P.PASS);
                    options.Add(P.DONE);
                }
            }
            else
            {
                if (any_drops[turn_player_color])
                {
                    options.Add(P.DROP);
                }
                if (any_moves[turn_player_color])
                {
                    options.Add(P.MOVE);
                }
                if (any_attacks[turn_player_color])
                {
                    options.Add(P.ATTACK);
                }
            }
        }

        private void Choose()
        {
        WriteOptions:
            Console.WriteLine("Options:");
            for (int i = 0; i <= options.Max(); i++)
            {
                if (options.Contains(i))
                {
                    Console.WriteLine(" [" + i + "] " + P.ConvertOption(i));
                }
            }

        ReceiveOption:
            int _choice = -1;

            Console.Write("> ");
            try
            {
                _choice = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Invalid option.");
                goto ReceiveOption;
            }

            if (!options.Contains(_choice))
            {
                Console.WriteLine("Invalid option.");
                goto ReceiveOption;
            }

            switch (_choice)
            {
                case P.SELECT:
                    Select();
                    goto WriteOptions;
                case P.DROP:
                    Drop();
                    break;
                case P.MOVE:
                    Move();
                    break;
                case P.ATTACK:
                    Attack();
                    break;
                case P.PASS:
                    Pass();
                    break;
                case P.DONE:
                    Done();
                    break;
                case P.CONCEDE:
                    Concede();
                    break;
                default:
                    break;
            }
        }

        private void Select()
        {
            Piece selected_piece = null;
            Console.WriteLine("Type the location of a piece [t-r-f] or type [-] to select a piece in hand.");
            PrintArray();

        ReceiveOption:
            String _location_choice = " ";
            int _t = -1, _r = -1, _f = -1;

            Console.Write("> ");
            try
            {
                _location_choice = Console.ReadLine();

                if (_location_choice == "-")
                {
                    goto WriteHandOptions;
                }

                _t = Convert.ToInt32(_location_choice.Substring(0, 1));
                _r = Convert.ToInt32(_location_choice.Substring(2, 1));
                _f = Convert.ToInt32(_location_choice.Substring(4, 1));
            }
            catch
            {
                Console.WriteLine("Invalid location.");
                goto ReceiveOption;
            }

            foreach (Piece p in board_pieces[turn_player_color])
            {
                if (p.T() == _t && p.R() == _r && p.F() == _f)
                {
                    selected_piece = p;
                }
            }
            if (selected_piece == null)
            {
                Console.WriteLine("Invalid location.");
                goto ReceiveOption;
            }
            else
            {
                goto Complete;
            }

        WriteHandOptions:
            char _hand_choice = ' ';
            Console.WriteLine("Select a piece in your hand.");
            foreach (Piece p in hand_pieces[turn_player_color])
            {
                Console.Write(P.ConvertPiece(p.Sym()) + " ");
            }
            Console.WriteLine();

        ReceiveHandOption:
            Console.Write("> ");
            try
            {
                _hand_choice = (char)Console.Read();
            }
            catch
            {
                Console.WriteLine("Invalid piece.");
                goto ReceiveOption;
            }

            foreach (Piece p in hand_pieces[turn_player_color])
            {
                if (P.ConvertPiece(p.Sym()) == _hand_choice)
                {
                    selected_piece = p;
                }
            }
            if (selected_piece == null)
            {
                Console.WriteLine("Invalid piece.");
                goto ReceiveHandOption;
            }
            else
            {
                goto Complete;
            }

        Complete:
            Console.WriteLine("Valid drop locations for this piece:");
            PrintSelection(selected_piece);
            Deselect();
            Console.WriteLine("Press a key to continue turn.");
            Console.ReadKey();
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

            foreach (Piece piece in all_hand_pieces)
            {
                if (piece.Type() == _type && piece.PlayerColor() == turn_player_color)
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

            foreach (Piece piece in all_board_pieces)
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

        private void Pass()
        {

        }

        private void Done()
        {
            done_with_initial_phase[turn_player_color] = true;
        }

        private void Concede()
        {
            Console.WriteLine("Unimplemented.");
        }

        private void InitPieces(int _player_color)
        {
            all_pieces.Add(new Piece(_player_color, P.MAR));
            all_pieces.Add(new Piece(_player_color, P.SPY));
            all_pieces.Add(new Piece(_player_color, P.SPY));
            all_pieces.Add(new Piece(_player_color, P.LIE));
            all_pieces.Add(new Piece(_player_color, P.LIE));
            all_pieces.Add(new Piece(_player_color, P.MAJ));
            all_pieces.Add(new Piece(_player_color, P.MAJ));
            all_pieces.Add(new Piece(_player_color, P.MAJ));
            all_pieces.Add(new Piece(_player_color, P.MAJ));
            all_pieces.Add(new Piece(_player_color, P.GEN));
            all_pieces.Add(new Piece(_player_color, P.GEN));
            all_pieces.Add(new Piece(_player_color, P.GEN));
            all_pieces.Add(new Piece(_player_color, P.GEN));
            all_pieces.Add(new Piece(_player_color, P.GEN));
            all_pieces.Add(new Piece(_player_color, P.GEN));
            all_pieces.Add(new Piece(_player_color, P.ARC));
            all_pieces.Add(new Piece(_player_color, P.ARC));
            all_pieces.Add(new Piece(_player_color, P.KNI));
            all_pieces.Add(new Piece(_player_color, P.KNI));
            all_pieces.Add(new Piece(_player_color, P.SAM));
            all_pieces.Add(new Piece(_player_color, P.SAM));
            all_pieces.Add(new Piece(_player_color, P.CAN));
            all_pieces.Add(new Piece(_player_color, P.CAN));
            all_pieces.Add(new Piece(_player_color, P.COU));
            all_pieces.Add(new Piece(_player_color, P.COU));
            all_pieces.Add(new Piece(_player_color, P.FOR));
            all_pieces.Add(new Piece(_player_color, P.FOR));
            all_pieces.Add(new Piece(_player_color, P.MUS));
            all_pieces.Add(new Piece(_player_color, P.MUS));
            all_pieces.Add(new Piece(_player_color, P.PAW));
            all_pieces.Add(new Piece(_player_color, P.PAW));
            all_pieces.Add(new Piece(_player_color, P.PAW));
            all_pieces.Add(new Piece(_player_color, P.PAW));
            all_pieces.Add(new Piece(_player_color, P.PAW));
            all_pieces.Add(new Piece(_player_color, P.PAW));
            all_pieces.Add(new Piece(_player_color, P.PAW));
            all_pieces.Add(new Piece(_player_color, P.PAW));
            all_pieces.Add(new Piece(_player_color, P.PAW));
        }
    }
}
