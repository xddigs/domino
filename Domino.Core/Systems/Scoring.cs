using Domino.Core.Objects;

namespace Domino.Core.Systems
{
    public class Scoring
    {
        public int[] Scores { get; }
        public Table Table { get; }
        
        public Scoring(Table table)
        {
            Table = table;
            Scores = new int[2];
        }
        
        public void Add(Turn turn, int amount)
        {
            int index = (turn == Turn.Player) ? 0 : 1;
            Scores[index] += amount;
        }
    }
}