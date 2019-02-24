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

        int turn_player_color;
        double turn_number;
        bool[] just_passed, setup_done;
        bool setup_phase;

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
            turn_number = 1.0;

            just_passed = new bool[2];
            setup_done = new bool[2];
            setup_phase = true;
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
            if (setup_phase && turn_number >= P.MAX_P + 1) EndSetup();
            if (!setup_phase && turn_number == 1) turn_player_color = P.WHITE;

            if (setup_phase) Console.WriteLine("Placement Turn #" + turn_number + ": " + P.ConvertColor(turn_player_color) + "'s turn.");
            else Console.WriteLine("Turn #" + turn_number + ": " + P.ConvertColor(turn_player_color) + "'s turn.");

            TurnReset();

            UpdateBoard();

            GetOptions();
            Choose();

            SwapTurn();
        }

        private void SwapTurn()
        {
            turn_player_color = (turn_player_color == P.BLACK) ? P.WHITE : P.BLACK;
            turn_number += 0.5;
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

        private void SelectAndPrint(Piece _selected_piece)
        {
            array.Select(_selected_piece);
            PrintArray();
        }

        private void SelectOnly(Piece _selected_piece)
        {
            array.Select(_selected_piece);
        }

        private void Deselect()
        {
            array.Deselect();
        }

        private void RandomDropLocation(out int _t, out int _r, out int _f)
        {
            int[] location = array.RandomDropLocation();
            _t = location[P.T];
            _r = location[P.R];
            _f = location[P.F];
        }

        private String LocString(int[] _location)
        {
            return (_location[P.R] + 1) + "-" + (_location[P.F] + 1) + "-" + (_location[P.T] + 1);
        }

        private void GetOptions()
        {
            options.Add(P.SELECT);

            if (setup_phase)
            {
                if (board_pieces[turn_player_color].Count < P.MAX_P)
                {
                    options.Add(P.DROP);
                    if (board_pieces[turn_player_color].Count >= P.MIN_P)
                    {
                        options.Add(P.PASS);
                        options.Add(P.DONE);
                    }
                }
                else {
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
            Deselect();
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

            if (_choice != P.PASS) just_passed[turn_player_color] = false;
            switch (_choice)
            {
                case P.SELECT:
                    SelectAny();
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

        private void SelectAny()
        {
            Piece selected_piece = null;
            Console.WriteLine("Type [b] to select a piece on the board, or type [h] to select a piece from your hand.");

        ReceiveOption:
            int _choice = -1;

            Console.Write("> ");
            try
            {
                _choice = Console.ReadLine()[0];
            }
            catch
            {
                Console.WriteLine("Invalid input.");
                goto ReceiveOption;
            }

            if (_choice == 'b')
            {
                if (all_board_pieces.Count > 0) goto FromBoard;

                Console.WriteLine("No pieces on board.");
                goto Complete;
            }
            else if (_choice == 'h')
            {
                if (hand_pieces[turn_player_color].Count > 0) goto FromHand;

                Console.WriteLine("No pieces in hand.");
                goto Complete;
            }
            else
            {
                Console.WriteLine("Invalid input.");
                goto ReceiveOption;
            }

        FromBoard:
            selected_piece = SelectFromBoard(true);
            Console.WriteLine("Valid moves and attacks for this piece:");
            SelectAndPrint(selected_piece);
            goto Complete;

        FromHand:
            selected_piece = SelectFromHand();
            Console.WriteLine("Valid drop locations for this piece:");
            SelectAndPrint(selected_piece);
            goto Complete;

        Complete:
            Console.WriteLine("Press a key to continue.\n");
            Console.ReadKey(true);
        }

        private Piece SelectFromHand()
        {
            Piece selected_piece = null;
            int _hand_choice = -1;

            Console.WriteLine("Select a piece from your hand.");
            foreach (Piece p in hand_pieces[turn_player_color])
            {
                Console.Write(p.Char() + " ");
            }
            Console.WriteLine();

        ReceiveOption:
            Console.Write("> ");
            try
            {
                _hand_choice = Console.ReadLine()[0];
            }
            catch
            {
                Console.WriteLine("Invalid piece.");
                goto ReceiveOption;
            }

            int rand = -1;
            if (_hand_choice == 'x')
            {
                rand = new Random().Next(0, hand_pieces[turn_player_color].Count);
            }

            int i = 0;
            foreach (Piece p in hand_pieces[turn_player_color])
            {
                if (p.Char() == _hand_choice || i == rand)
                {
                    selected_piece = p;
                }
                i++;
            }
            if (selected_piece == null)
            {
                Console.WriteLine("Invalid piece.");
                goto ReceiveOption;
            }

            return selected_piece;
        }

        private Piece SelectFromBoard(bool _all_board)
        {
            Piece selected_piece = null;
            String _location_choice = " ";
            int _t = -1, _r = -1, _f = -1;

            Console.WriteLine("Type the location of a piece [t-r-f].");
            PrintArray();

        ReceiveOption:
            Console.Write("> ");
            try
            {
                _location_choice = Console.ReadLine();

                _r = Convert.ToInt32(_location_choice.Substring(0, 1)) - 1;
                _f = Convert.ToInt32(_location_choice.Substring(2, 1)) - 1;
                _t = Convert.ToInt32(_location_choice.Substring(4, 1)) - 1;
            }
            catch
            {
                Console.WriteLine("Invalid location.");
                goto ReceiveOption;
            }

            if (_all_board)
            {
                foreach (Piece p in all_board_pieces)
                {
                    if (p.T() == _t && p.R() == _r && p.F() == _f)
                    {
                        selected_piece = p;
                    }
                }
            }
            else
            {
                foreach (Piece p in board_pieces[turn_player_color])
                {
                    if (p.T() == _t && p.R() == _r && p.F() == _f)
                    {
                        selected_piece = p;
                    }
                }
            }
            
            if (selected_piece == null)
            {
                Console.WriteLine("Invalid location.");
                goto ReceiveOption;
            }

            return selected_piece;
        }

        private int[] SelectLocation()
        {
            String _choice = " ";
            int _t = -1, _r = -1, _f = -1;

            Console.WriteLine("Type a location [r-f-t].");

        ReceiveOption:
            Console.Write("> ");
            try
            {
                _choice = Console.ReadLine();

                _r = Convert.ToInt32(_choice.Substring(0, 1)) - 1;
                _f = Convert.ToInt32(_choice.Substring(2, 1)) - 1;
                _t = Convert.ToInt32(_choice.Substring(4, 1)) - 1;
            }
            catch
            {
                if (_choice == "x")
                {
                    RandomDropLocation(out _t, out _r, out _f);
                }
                else
                {
                    Console.WriteLine("Invalid location.");
                    goto ReceiveOption;
                }
            }

            if (_t < 0 || _t >= P.TM || _r < 0 || _r >= P.RM || _f < 0 || _f >= P.FM)
            {
                Console.WriteLine("Invalid location.");
                goto ReceiveOption;
            }

            return new int[] { _t, _r, _f };
        }

        private void Drop()
        {
            Piece selected_piece = null;

        Beginning:
            if (board_pieces[turn_player_color].Count == 0)
            {
                foreach (Piece p in hand_pieces[turn_player_color])
                {
                    if (p.Type() == P.MAR)
                    {
                        selected_piece = p;
                    }
                }
                Console.WriteLine("The marshal must be dropped first. Press a key to select the marshal.");
                Console.ReadKey(true);
            }
            else
            {
                selected_piece = SelectFromHand();
            }
            Console.WriteLine("Valid drop locations for [" + selected_piece.Char() + "]:");
            SelectAndPrint(selected_piece);
            SelectOnly(selected_piece);

            int[] location = SelectLocation();
            if (!selected_piece.DropTo(location))
            {
                Console.WriteLine("Invalid location: " + LocString(location) + ".");
                goto Beginning;
            }

            Console.WriteLine("You dropped [" + selected_piece.Char() + "] onto " + LocString(location) + ". Press a key to end your turn.\n");
            Console.ReadKey(true);
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
                    piece.MoveTo(_tier, _rank, _file);
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
            Console.WriteLine(P.ConvertColor(turn_player_color) + "'s turn was passed.");
            just_passed[turn_player_color] = true;

            if (just_passed[1 - turn_player_color]) EndSetup();
        }

        private void Done()
        {
            Console.WriteLine("Your placement is now complete.");
            setup_done[turn_player_color] = true;

            if (setup_done[1 - turn_player_color]) EndSetup();
        }

        private void EndSetup()
        {
            Console.WriteLine("Both placements are complete. The game may now begin.");
            setup_phase = false;
            array.SetupDone();
            turn_number = 1;
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
