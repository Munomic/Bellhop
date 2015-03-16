using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jubble
{
    public struct GameState
    {
        public const int MaxRoundCount = 3;
        public const int MaxSpawnCount = 7;
        public const int MaxTargetCount = 6;

        public int PlayerNumber;
        public int NumberOfRounds;
        public int NumberOfSpawns;
        public int NumberOfTargets;
        public int BadTargetsHit;

        public float[] RoundTime;
        public float ScoreTime;
        public int[] RoundScore;
        public float TimeToNextSpawn;
        public float BullseyeBonus;
        public int TotalBullseyes;
        public int BullseyeStreakCount;

        public int CurrentRound
        {
            get { return MaxRoundCount - NumberOfRounds - 1; }
        }

        public float CurrentRoundTime
        {
            get { return RoundTime[ CurrentRound ]; }
            set { RoundTime[ CurrentRound ] = value; }
        }

        public int CurrentRoundScore
        {
            get { return RoundScore[ CurrentRound ]; }
            set { RoundScore[ CurrentRound ] = value; }
        }

        public int TotalScore
        {
            get
            {
                int totalScore = 0;
                for( int i = 0; i < RoundScore.Length; i++ )
                {
                    totalScore += RoundScore[ i ];
                }
                return totalScore;
            }
        }

        public GameState(
            int playerNumber,
            int numberOfRounds,
            int numberOfSpawns,
            int numberOfTargets,
            float scoreTime,
            float timeToNextSpawn
            )
        {
            PlayerNumber = playerNumber;
            NumberOfRounds = numberOfRounds;
            NumberOfSpawns = numberOfSpawns;
            NumberOfTargets = numberOfTargets;
            BadTargetsHit = 0;
            RoundTime = new float[ MaxRoundCount ];
            ScoreTime = scoreTime;
            RoundScore = new int[ MaxRoundCount ];
            TimeToNextSpawn = timeToNextSpawn;
            BullseyeBonus = 0.0f;
            TotalBullseyes = 0;
            BullseyeStreakCount = 0;
        }
    }
}
