# TODO List and Development Phases
## Phase 1: Base functionality
- [x]  GridSystem that provides information about Nodes
- [x]  Self managing nodes/tiles
- [x]   basic visuals for presentation    
- [x]  movement on available tiles



## Phase 2: AI logic
- [x] Implement AI movement to available tiles 
- [x] Pathfinding for AI to find the shortest path to tiles 
- [ ] AI decision making based on available strategies and different scenarios

All strategies for the most part just order AI to go to certain point !
strategies :
1. **CollectableStrategy** (implemented but there are bugs that relates to Decision maker class "AICognition")
2. **ExploreStrategy** : (already implemented)
3. **ShootMissileStrategy** : (leads player to a position and triggers the missile if its available)
4. **CompetingStrategy** : (goes after tiles that are owned by specified opponent and takes over half of them)

## Phase 3: Pickups System
- [X] implementing pool of collectables that spawns object in a form of wave
- [X] score pickup
- [ ] missile pickup
- [ ] Speed Pickup 


## Phase 4: Scoring System
- [ ] Implement scoring based on tile ownership
- [ ] 
