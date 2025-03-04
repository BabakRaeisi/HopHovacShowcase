# HopHovac

## Overview
This is a grid-based game where players and AI-controlled opponents move around the grid, capturing tiles and collecting pickups to score points. The goal is to outsmart and outperform opponents to control the most tiles.
https://youtu.be/rcxuEdDlwiY

## Features
- Player-controlled movement
- AI-controlled opponents
- Tile-based grid system
- Pathfinding algorithm for AI
- Power-ups and pickups 

## System Architecture
- **GridSystem**: Manages the grid, stores Nodes in a dictionary, and handles tile occupancy.
- **Movement**: Handles player and AI movement on the grid. both AI and user utilize a Movement as a form of composition 
- **AI Logic**: Implements decision-making and movement for AI players, including pathfinding and tile prioritization.
- AI logic consists of several classes that use a mix of startegy pattern and decision trees to form the AI state

## Setup Instructions
1. Clone the repository
2. Open the project in Unity.
3. Run the scene to start the game.

## Future Enhancements
- Add more intelligent AI pathfinding.
- Introduce more game modes and power-ups.
- [not the final gameplay](https://github.com/BabakRaeisi/HopHovac/blob/Master/game.jpg)
 

