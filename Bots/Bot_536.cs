namespace auto_Bot_536;
using ChessChallenge.API;
using System.Linq;

public class Bot_536 : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        int evaluate(out Move bestMove, int depth, int alpha, int beta)
        {
            bestMove = new();

            if (board.IsDraw())
                return 0;

            int eval = board.GetLegalMoves().Length - board.FiftyMoveCounter, iMax = -1000000 * depth;
            if (depth == 0)
            {
                foreach (PieceList pieceList in board.GetAllPieceLists())
                    eval += (int)pieceList.TypeOfPieceInList * pieceList.Count * (pieceList.IsWhitePieceList == board.IsWhiteToMove ? 20 : -20);
                return eval;
            }

            foreach (Move move in board.GetLegalMoves().OrderByDescending(move => move.CapturePieceType))
            {
                board.MakeMove(move);
                eval = -evaluate(out Move dump, depth - (board.IsInCheck() ? 0 : 1), -beta, -alpha);
                board.UndoMove(move);

                if (timer.MillisecondsElapsedThisTurn > timer.MillisecondsRemaining / 30)
                    return 0;

                if (eval > iMax)
                {
                    iMax = eval;
                    bestMove = move;
                }
                if (eval > alpha)
                    alpha = eval;
                if (alpha >= beta)
                    break;
            }

            return iMax;
        }

        Move chosenMove = board.GetLegalMoves()[0], newChosenMove = new();
        int depth = 1;

        while (timer.MillisecondsElapsedThisTurn < timer.MillisecondsRemaining / 30)
        {
            chosenMove = newChosenMove;
            evaluate(out newChosenMove, depth++, -1000000000, 1000000000);
        }

        return chosenMove;
    }

}