# Marketeer
This README will give information what would've been presented.

1. Overview of Game
In Marketeer, you play as a employee of a Supermarket.
In the game you are trying to keep shelves stocked as much
as possible. Customers take from the shelves, and the Manager
will randomly check shelves. If the manager finds an empty shelf,
the player gets a strike. 3 strikes and the player is fired.
The player's ultimate goal is to make as much money as possible.
This is done in 2 ways. First, the player gets paid minimum wage
every minute ($7.25). Second, the player can get tips from helping customers.

## IMPORTANT: Once you get at least $3, press space on the coke vending machine

2. Each member's contributions

Joey
- Programmed most of the game
- Wrote AI and State Machine
- Created a few base models for shoppers, shelving, and the green truck
- Game Design
- General bug fixes + other help
- Dimension Shifting

Jermie
- Created vast majority of the art (Every model excluding the few made by Joey)
- Created inital state machine for Manager
- Doors + Dumpster functionality
- Back Shelves
- Most 2D Textures
- Menu System
- Game Design

Zach
- Intial Game idea
- Map design
- Created UI icons for inventory
- All Music and Sound Effects (save 1)
- Final end screen score
- Timer Counter
- Menu additions
- Tutorial text
- Game Design

3. AI Component
The primary AI component in our game is a modified A* algorithm.
In our game manager we have a public Dictionary<Vector3, Dictionary<int, GridObject>> GameGrid.
This GameGrid contains everything in the scene that is on the Grid. The key of the parent
Dictionary is the location of the object, and the given value is a dictionary of all of the
different objects that plan to be on a space at a given time. 

This allows us to have our GridObjects navigate around each other, as every GridObject
stores its path (including the time it will take to get to that step in the path) in the GameGrid.

If you want to look at the AI code directly, The GameGrid is in GridManager.cs.
The modified A* algorithm is in MovingGridObject.cs