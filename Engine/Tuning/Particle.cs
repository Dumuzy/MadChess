﻿// +------------------------------------------------------------------------------+
// |                                                                              |
// |     MadChess is developed by Erik Madsen.  Copyright 2019.                   |
// |     MadChess is free software.  It is distributed under the GNU General      |
// |     Public License Version 3 (GPLv3).  See LICENSE file for details.         |
// |     See https://www.madchess.net/ for user and developer guides.             |
// |                                                                              |
// +------------------------------------------------------------------------------+


using System;


namespace ErikTheCoder.MadChess.Engine.Tuning
{
    public sealed class Particle
    {
        public readonly PgnGames PgnGames;
        public readonly Parameters Parameters;
        public readonly Parameters BestParameters;
        public double EvaluationError;
        public double BestEvaluationError;
        private const double _maxInitialVelocityPercent = 0.10;
        private const double _inertia = 0.75d;
        private const double _influence = 1.50d;
        private readonly double[] _velocities;
        
        
        public Particle(PgnGames PgnGames, Parameters Parameters)
        {
            this.PgnGames = PgnGames;
            this.Parameters = Parameters;
            BestParameters = Parameters.DuplicateWithSameValues();
            EvaluationError = double.MaxValue;
            BestEvaluationError = double.MaxValue;
            // Initialize random velocities.
            _velocities = new double[Parameters.Count];
            InitializeRandomVelocities();
        }


        private void InitializeRandomVelocities()
        {
            for (int index = 0; index < Parameters.Count; index++)
            {
                Parameter parameter = Parameters[index];
                double maxVelocity = _maxInitialVelocityPercent * (parameter.MaxValue - parameter.MinValue);
                // Allow positive or negative velocity.
                _velocities[index] = (SafeRandom.NextDouble() * maxVelocity * 2) - maxVelocity;
            }
        }


        public void Move()
        {
            // Move particle in parameter space.
            for (int index = 0; index < Parameters.Count; index++)
            {
                Parameter parameter = Parameters[index];
                parameter.Value += (int) _velocities[index];
                if (parameter.Value < parameter.MinValue)
                {
                    parameter.Value = parameter.MinValue;
                    _velocities[index] = 0;
                }
                if (parameter.Value > parameter.MaxValue)
                {
                    parameter.Value = parameter.MaxValue;
                    _velocities[index] = 0;
                }
            }
        }


        public void ConfigureEvaluation(Evaluation Evaluation)
        {
            // Pawns
            Evaluation.Config.MgPawnAdvancement = Parameters[nameof(EvaluationConfig.MgPawnAdvancement)].Value;
            Evaluation.Config.EgPawnAdvancement = Parameters[nameof(EvaluationConfig.EgPawnAdvancement)].Value;
            Evaluation.Config.MgPawnCentrality = Parameters[nameof(EvaluationConfig.MgPawnCentrality)].Value;
            Evaluation.Config.EgPawnCentrality = Parameters[nameof(EvaluationConfig.EgPawnCentrality)].Value;
            Evaluation.Config.EgPawnConstant = Parameters[nameof(EvaluationConfig.EgPawnConstant)].Value;
            // Knights
            Evaluation.Config.MgKnightAdvancement = Parameters[nameof(EvaluationConfig.MgKnightAdvancement)].Value;
            Evaluation.Config.EgKnightAdvancement = Parameters[nameof(EvaluationConfig.EgKnightAdvancement)].Value;
            Evaluation.Config.MgKnightCentrality = Parameters[nameof(EvaluationConfig.MgKnightCentrality)].Value;
            Evaluation.Config.EgKnightCentrality = Parameters[nameof(EvaluationConfig.EgKnightCentrality)].Value;
            Evaluation.Config.MgKnightCorner = Parameters[nameof(EvaluationConfig.MgKnightCorner)].Value;
            Evaluation.Config.EgKnightCorner = Parameters[nameof(EvaluationConfig.EgKnightCorner)].Value;
            Evaluation.Config.EgKnightConstant = Parameters[nameof(EvaluationConfig.EgKnightConstant)].Value;
            // Bishops
            Evaluation.Config.MgBishopAdvancement = Parameters[nameof(EvaluationConfig.MgBishopAdvancement)].Value;
            Evaluation.Config.EgBishopAdvancement = Parameters[nameof(EvaluationConfig.EgBishopAdvancement)].Value;
            Evaluation.Config.MgBishopCentrality = Parameters[nameof(EvaluationConfig.MgBishopCentrality)].Value;
            Evaluation.Config.EgBishopCentrality = Parameters[nameof(EvaluationConfig.EgBishopCentrality)].Value;
            Evaluation.Config.MgBishopCorner = Parameters[nameof(EvaluationConfig.MgBishopCorner)].Value;
            Evaluation.Config.EgBishopCorner = Parameters[nameof(EvaluationConfig.EgBishopCorner)].Value;
            Evaluation.Config.EgBishopConstant = Parameters[nameof(EvaluationConfig.EgBishopConstant)].Value;
            // Rooks
            Evaluation.Config.MgRookAdvancement = Parameters[nameof(EvaluationConfig.MgRookAdvancement)].Value;
            Evaluation.Config.EgRookAdvancement = Parameters[nameof(EvaluationConfig.EgRookAdvancement)].Value;
            Evaluation.Config.MgRookCentrality = Parameters[nameof(EvaluationConfig.MgRookCentrality)].Value;
            Evaluation.Config.EgRookCentrality = Parameters[nameof(EvaluationConfig.EgRookCentrality)].Value;
            Evaluation.Config.MgRookCorner = Parameters[nameof(EvaluationConfig.MgRookCorner)].Value;
            Evaluation.Config.EgRookCorner = Parameters[nameof(EvaluationConfig.EgRookCorner)].Value;
            Evaluation.Config.EgRookConstant = Parameters[nameof(EvaluationConfig.EgRookConstant)].Value;
            // Queens
            Evaluation.Config.MgQueenAdvancement = Parameters[nameof(EvaluationConfig.MgQueenAdvancement)].Value;
            Evaluation.Config.EgQueenAdvancement = Parameters[nameof(EvaluationConfig.EgQueenAdvancement)].Value;
            Evaluation.Config.MgQueenCentrality = Parameters[nameof(EvaluationConfig.MgQueenCentrality)].Value;
            Evaluation.Config.EgQueenCentrality = Parameters[nameof(EvaluationConfig.EgQueenCentrality)].Value;
            Evaluation.Config.MgQueenCorner = Parameters[nameof(EvaluationConfig.MgQueenCorner)].Value;
            Evaluation.Config.EgQueenCorner = Parameters[nameof(EvaluationConfig.EgQueenCorner)].Value;
            Evaluation.Config.EgQueenConstant = Parameters[nameof(EvaluationConfig.EgQueenConstant)].Value;
            // King
            Evaluation.Config.MgKingAdvancement = Parameters[nameof(EvaluationConfig.MgKingAdvancement)].Value;
            Evaluation.Config.EgKingAdvancement = Parameters[nameof(EvaluationConfig.EgKingAdvancement)].Value;
            Evaluation.Config.MgKingCentrality = Parameters[nameof(EvaluationConfig.MgKingCentrality)].Value;
            Evaluation.Config.EgKingCentrality = Parameters[nameof(EvaluationConfig.EgKingCentrality)].Value;
            Evaluation.Config.MgKingCorner = Parameters[nameof(EvaluationConfig.MgKingCorner)].Value;
            Evaluation.Config.EgKingCorner = Parameters[nameof(EvaluationConfig.EgKingCorner)].Value;
            // Passed Pawns
            Evaluation.Config.MgPassedPawnScalePercent = Parameters[nameof(EvaluationConfig.MgPassedPawnScalePercent)].Value;
            Evaluation.Config.EgPassedPawnScalePercent = Parameters[nameof(EvaluationConfig.EgPassedPawnScalePercent)].Value;
            Evaluation.Config.EgFreePassedPawnScalePercent = Parameters[nameof(EvaluationConfig.EgFreePassedPawnScalePercent)].Value;
            Evaluation.Config.EgKingEscortedPassedPawn = Parameters[nameof(EvaluationConfig.EgKingEscortedPassedPawn)].Value;
            Evaluation.Configure();
            // Piece Mobility
            Evaluation.Config.MgKnightMobilityScale = Parameters[nameof(EvaluationConfig.MgKnightMobilityScale)].Value;
            Evaluation.Config.EgKnightMobilityScale = Parameters[nameof(EvaluationConfig.EgKnightMobilityScale)].Value;
            Evaluation.Config.MgBishopMobilityScale = Parameters[nameof(EvaluationConfig.MgBishopMobilityScale)].Value;
            Evaluation.Config.EgBishopMobilityScale = Parameters[nameof(EvaluationConfig.EgBishopMobilityScale)].Value;
            Evaluation.Config.MgRookMobilityScale = Parameters[nameof(EvaluationConfig.MgRookMobilityScale)].Value;
            Evaluation.Config.EgRookMobilityScale = Parameters[nameof(EvaluationConfig.EgRookMobilityScale)].Value;
            Evaluation.Config.MgQueenMobilityScale = Parameters[nameof(EvaluationConfig.MgQueenMobilityScale)].Value;
            Evaluation.Config.EgQueenMobilityScale = Parameters[nameof(EvaluationConfig.EgQueenMobilityScale)].Value;
        }
        

        // See http://talkchess.com/forum/viewtopic.php?t=50823&postdays=0&postorder=asc&highlight=texel+tuning&topic_view=flat&start=20.
        public void CalculateEvaluationError(Board Board, Search Search, int WinPercentScale)
        {
            // Sum the square of evaluation error over all games.
            double evaluationError = 0;
            for (int gameIndex = 0; gameIndex < PgnGames.Count; gameIndex++)
            {
                PgnGame game = PgnGames[gameIndex];
                if (game.Result == GameResult.Unknown) continue; // Skip games with unknown results.
                Board.SetPosition(Board.StartPositionFen, true);
                for (int moveIndex = 0; moveIndex < game.Moves.Count; moveIndex++)
                {
                    ulong move = game.Moves[moveIndex];
                    // Play move.
                    Board.PlayMove(move);
                    // Get quiet score.
                    Board.NodesExamineTime = long.MaxValue;
                    Search.PvInfoUpdate = false;
                    Search.Continue = true;
                    int quietScore = Search.GetQuietScore(Board, 1, 1, Board.AllSquaresMask, -StaticScore.Max, StaticScore.Max);
                    // Convert quiet score to win percent.
                    double winPercent = GetWinPercent(quietScore, WinPercentScale);
                    // Compare win percent to game result.
                    double result;
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (game.Result)
                    {
                        case GameResult.WhiteWon:
                            result = Board.CurrentPosition.WhiteMove ? 1d : 0;
                            break;
                        case GameResult.Draw:
                            result = 0.5d;
                            break;
                        case GameResult.BlackWon:
                            result = Board.CurrentPosition.WhiteMove ? 0 : 1d;
                            break;
                        default:
                            throw new InvalidOperationException($"{game.Result} game result not supported.");
                    }
                    evaluationError += Math.Pow(winPercent - result, 2);
                }
            }
            EvaluationError = evaluationError;
            if (EvaluationError < BestEvaluationError)
            {
                BestEvaluationError = EvaluationError;
                Parameters.CopyValuesTo(BestParameters);
            }
        }


        public void UpdateVelocity(Particle BestSwarmParticle, Particle GloballyBestParticle)
        {
            for (int index = 0; index < Parameters.Count; index++)
            {
                Parameter parameter = Parameters[index];
                Parameter bestParameter = BestParameters[index];
                Parameter bestSwarmParameter = BestSwarmParticle.BestParameters[index];
                Parameter globallyBestParameter = GloballyBestParticle.BestParameters[index];
                double velocity = _inertia * _velocities[index];
                double particleMagnitude = SafeRandom.NextDouble() * _influence;
                velocity += particleMagnitude * (bestParameter.Value - parameter.Value);
                double swarmMagnitude = SafeRandom.NextDouble() * ParticleSwarm.Influence;
                velocity += swarmMagnitude * (bestSwarmParameter.Value - parameter.Value);
                double allSwarmsMagnitude = SafeRandom.NextDouble() * ParticleSwarms.Influence;
                velocity += allSwarmsMagnitude * (globallyBestParameter.Value - parameter.Value);
                _velocities[index] = velocity;
            }
        }
        

        private static double GetWinPercent(int Score, int WinPercentScale) => 1d / (1d + Math.Pow(10d, -1d * Score / WinPercentScale)); // Use a sigmoid function to map score to winning percent.  See WinPercent.xlsx.
    }
}
