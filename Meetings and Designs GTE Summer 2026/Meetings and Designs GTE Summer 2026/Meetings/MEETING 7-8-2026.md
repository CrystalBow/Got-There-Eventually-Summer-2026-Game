# Itinerary
- [x] The story so far
	- [x] What we have discussed
	- [x] What is coming up
- [x] Yimer's Vision
- [x] Discussion (Open Table)
- [x] Design Document Discussion
- [ ] Assign Tasks
# Attendance
- Yimer C. Duggan
- Aidan Alich
- Ron Bora
- Jaime Sepulveda
- Maria
# Notes
## The story so far
It's week 3 of the class and professor wants us to have all the basics down, fortunately I've already set up our git hub, project management, and unity 2D project. We just need to agree on rough design principles and handing out tasks and roles.
### What we have discussed
Card RPG where we use cards as actions.
Tolls and Taxes: the story about taxes and wealth gap.
Goblin is mad about taxes.
possible affinities
3-4 heroes
### What is coming up
- Design Document (Today)
- Unity Practicum (Friday)
- Protype Presentation (Next Week)

## Yimer's Vision
Characters: Deck, hp, mp, atk, def, spd
![[Pasted image 20260708121826.png]]
Atk + Cards value = damage dealt
### Exploration/Story
Use Attacks to destroy things or move blocks
Use grappling hook to cross gaps
Use dashes to move faster
Enemies patrol and chase you down to start combat.
### Combat
Turn based
Each character has deck and a hand
Attack, Buffs, Debuffs, and Healing cards
Characters have 3 signature cards they start with.
Shuffling Aspect (auto or action)
Enemies just do damage
### HOW
JSON  (GameData) -> Singleton (DataCenter) -> All other parts of the game
Between Scenes: Scene Data -> (DataCneter) -> next Scene
Scenes: City, Dungeon, Combat Scene, Start Menu, Game Over is also a scene
#### State Machines
Something can be in State.
State Machine is a graph made of states and transitions that something can move along.

Players
- Player Movement
- Combat

Enemies
- Patrolling
- Combat

NPC (or generic Unit)
- Interactable
- Patrolling

### MVP
- 3 Characters
- Collection of 12 cards
- No items
- 1 Area and City
- 3 environment mechanics
- Rest
- Level ups
# Discussion (Open Table)
- Possible removal of city of story doesn't require it.
# Design Document
- Needs to get done tonight
- Lists mostly what we talked about and stretch goals.
# Assign Tasks and Roles
## Combat
Aiden (L)
Ron

## Exploration
Yimer (L)
Jamie
Aiden 

## Menus and UI
Maria (L)
Ron

## Story and Art Direction
Jamie (L)
Maria

## Tasks
### Everyone
Unity Practicum (Friday)

### Menus and UI
Start and Gameover 

### Data Center
Yimer

### Combat
Initiative and it taking turns

### Exploration
Character Movement and Tile Map

### Design Doc
Maria - Art and Compromises
Aidan - Review and fill gaps

