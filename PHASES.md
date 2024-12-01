# Development Phases

## Phase 1: AI Logic and Movement

### 2.1 - Tile Awareness and Movement
- Goal: Ensure the AI can identify available (unoccupied) tiles and move toward one.
- Details: 
  - AI scans the grid for unoccupied tiles using the dictionary in GridSystem.
  - AI randomly selects one of the available tiles as its target.
  - AI moves to the target tile using movement logic.
  - AI checks availability of target tile.
  - AI should be able to recognize if there is a beneficial powerup spawning near it
  

### 2.2 - Prioritizing Opponent-Owned Tiles
- Goal: Make the AI prioritize opponent-owned tiles.
- Details:
  - AI will check for opponent-owned tiles and choose those as its target, competing for tile control.
  - If no opponent-owned tiles are available, the AI will fall back on selecting unoccupied tiles.

## Future Phases

### Phase 2: Pickups and Power-ups
- Goal: Implement a system where AI recognizes and collects pickups.
- Details:
  - AI will prioritize nearby pickups, adjusting its strategy to collect beneficial power-ups before continuing to capture tiles.
  - prioritize opponents to target their score or hit them with 
  
  
[...and so on for future phases...]
