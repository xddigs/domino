# 🀄 C# Dominoes - Game Rules

This project features a C# implementation of the classic **Double-Six Dominoes**. Below are the standard rules and gameplay mechanics used in this version.

## 📋 General Description
The game uses a set of **28 tiles** (ranging from double-blank to double-six). It supports 2 to 4 players, either in free-for-all or partnership mode.

## 🎮 Gameplay Flow

### 1. The Deal
- All tiles are shuffled face down.
- Each player draws **7 tiles**.
- Remaining tiles are placed in the **"Boneyard"** to be drawn later if a player lacks a valid move.

### 2. The Opening Move
- Traditionally, the player holding the **Double-Six (6-6)** goes first.
- If no one holds the 6-6, the player with the highest double (5-5, 4-4, etc.) starts the game.

### 3. Playing the Game
- Players take turns placing tiles, matching the number on one end of their tile with an open end of the domino chain on the table.
- **Doubles:** These are placed crosswise (perpendicular) to the line for visual clarity, though they follow standard matching rules.
- **Passing and Drawing:** If a player cannot make a move:
    - They must **draw from the boneyard** until they find a playable tile.
    - If the boneyard is empty, the player must **pass** their turn.

### 4. Ending the Round
The round ends when:
1. **Domino!:** A player successfully plays their last tile.
2. **Blocked Game:** No player can make a move, but tiles still remain in hand. In this case, players count the pips (dots) in their hands; the player with the **lowest score** wins the round.

## 🏆 Scoring
- The winner of the round earns points equal to the total sum of pips remaining in the opponents' hands.
- The overall game typically ends when a player or team reaches a target score (e.g., **100 or 200 points**).

---
*Developed with C# and a lot of patience.*