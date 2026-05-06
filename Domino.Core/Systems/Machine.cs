using System;
using System.Linq;
using Domino.Core.Objects;
using Microsoft.Xna.Framework;

namespace Domino.Core.Systems
{
    public class Machine
    {
        private static readonly Random Rng = new();
        public Table Table { get; }
        public enum MachineState
        {
            Waiting,
            Thinking,
            Executing
        }

        public MachineState State { get; set; } = MachineState.Waiting;
        private float _timer;
        private float _delay;

        public Machine(Table table)
        {
            Table = table;
        }
        
        public void Update(GameTime gameTime)
        {
            if (Table.Turn != Turn.Machine) 
            {
                State = MachineState.Waiting;
                _timer = 0;
                return;
            }

            switch (State)
            {
                case MachineState.Waiting:
                    _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (!ArePiecesActive() || _timer > 3.0f) 
                    {
                        State = MachineState.Thinking;
                        _timer = 0;
                        _delay = 0.5f + (float)Rng.NextDouble() * 1.0f;
                    }
                    break;

                case MachineState.Thinking:
                    _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_timer >= _delay)
                    {
                        State = MachineState.Executing;
                    }
                    break;

                case MachineState.Executing:
                    MakeMove();
                    _timer = 0; 
                    State = MachineState.Waiting;
                    break;
            }
        }
        
        private bool ArePiecesActive()
        {
            var boardTiles = Table.Tiles.Where(
                t => t.Owner == Tile.TileOwner.Board);
            
            return boardTiles.Any(t => Vector2.Distance(
                t.Position, t.LastPosition) > 2.0f);
        }
        
        private void MakeMove()
        {
            var myTiles = Table.Tiles.Where(
                t => t.Owner == Tile.TileOwner.Machine).ToList();
    
            foreach (var tile in myTiles)
            {
                if (Table.ActiveTiles.Count > 0)
                {
                    Vector2 headPos = Table.ActiveTiles.First!.Value.Position;
                    Vector2 tailPos = Table.ActiveTiles.Last!.Value.Position;

                    Table.TryPlaceTile(tile, headPos);
                    if (tile.Owner == Tile.TileOwner.Board) return;

                    Table.TryPlaceTile(tile, tailPos);
                    if (tile.Owner == Tile.TileOwner.Board) return;
                }
                else
                {
                    Table.TryPlaceTile(tile, Vector2.Zero);
                    return;
                }
            }

            if (Table.Tiles.Any(t => t.Owner == Tile.TileOwner.Boneyard))
            {
                Table.Rob(Tile.TileOwner.Machine);
            }
            else
            {
                Table.Turn = Table.SwitchTurn();
            }
        }
    }
}