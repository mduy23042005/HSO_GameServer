# 2D MMORPG HSO

> Inspired by **Knight Age** by Teamobi.  
> This repository contains the **Unity Client project**.

# Team Member

| Full Name | Student ID | Role |
|-----------|------------|------|
| Lai Minh Duy | 2311553066 | Project Leader, Unity Developer, Backend Developer, UI/UX Designer |

---

# Project Overview

HSO is a 2D MMORPG project developed to study and implement a client–server architecture for online games.

This repository contains the Unity Client responsible for rendering the game world, handling player input, managing user interfaces, communicating with the backend Server, and synchronizing real-time game data.

The Unity Client communicates with the Server through WebSocket.

The Server is responsible for real-time game processing and communicates with the Web API for persistent database operations.

---

# System Architecture

The complete system uses the following communication flow:

```text
Unity Client
     │
     │ WebSocket
     ▼
Server
     │
     │ REST API
     ▼
Web API
     │
     │ Entity Framework Core
     ▼
SQL Server
     │
     │ Entity Framework Core
     ▼
Web API
     │
     │ REST API
     ▼
Server
     │
     │ WebSocket
     ▼
Unity Client
```

The Unity Client is responsible for the presentation and client-side gameplay layer.

- Unity Client handles rendering, input, UI, local gameplay presentation, and client-side state management.
- WebSocket is used between the Unity Client and Server for real-time gameplay communication.
- The Server manages real-time multiplayer game processing.
- REST API is used between the Server and Web API for persistent data operations.
- Entity Framework Core is used by the Web API to communicate with SQL Server.

---

# Project Objectives

This project was developed to:

- Study the client–server architecture used in online games.
- Practice game development using Unity.
- Practice real-time communication using WebSocket.
- Implement multiplayer game synchronization.
- Implement player movement and state synchronization.
- Implement character appearance synchronization.
- Implement mob and NPC presentation.
- Implement inventory and equipment systems.
- Implement 2D map and minimap systems.
- Practice asynchronous network programming.
- Practice client-side game architecture and state management.
- Serve as the Unity Client for the HSO MMORPG.
- Serve as a graduation thesis project.

---

# Technologies Used

## Game Engine

- Unity
- Unity 2D
- Unity 2D Animation
- Unity Tilemap
- Unity Input System

## Programming Languages

- C#

## Networking

- WebSocket
- ClientWebSocket
- Binary packet communication

## Graphics and Animation

- SpriteRenderer
- Animator
- SpriteResolver
- SpriteLibrary
- TextMeshPro
- 2D Animation

##Development Tools

- Unity
- Visual Studio 2022
- Git
- GitHub

---

# Client Components

Game Client

The Unity Client is responsible for handling the client-side game experience and communicating with the Server.

Main responsibilities include:

- Managing WebSocket connections.
- Sending and receiving network packets.
- Managing player input.
- Player movement.
- Player state management.
- Player animation.
- Character appearance management.
- Other player synchronization.
- Mob synchronization.
- Mob state presentation.
- NPC presentation.
- Map loading and rendering.
- Tilemap management.
- Minimap management.
- Inventory management.
- Equipment management.
- Character information UI.
- Login and registration UI.
- Game UI management.
- Client-side game state management.
- Processing real-time game data received from the Server.

---

# Client Architecture

The Unity Client is organized into several major layers:

```text
Unity UI
   │
   ▼
Controllers
   │
   ▼
Managers
   │
   ▼
Network Layer
   │
   │ WebSocket
   ▼
Server
```

## UI Layer

The UI layer is responsible for displaying game information and receiving user interactions.

Main components include:

- Login UI.
- Registration UI.
- Character creation UI.
- Character information UI.
- Inventory UI.
- Equipment UI.
- Game HUD.
- HP/MP UI.
- Minimap UI.
- Character selection UI.

## Controller Layer

Controllers handle gameplay behavior and user interaction.

Main responsibilities include:

- Player movement.
- Player input.
- Character animation.
- Character appearance.
- Inventory interaction.
- Equipment interaction.
- Map interaction.
- UI interaction.

## Manager Layer

Managers control shared game systems and maintain client-side game state.

Main systems include:

- Socket Manager.
- Map Manager.
- Player Manager.
- Mob Manager.
- Inventory Manager.
- Equipment Manager.
- UI Manager.
- Cache Manager.

## Network Layer

The network layer manages communication between the Unity Client and Server.

Main responsibilities include:

- Establishing WebSocket connections.
- Sending packets.
- Receiving packets.
- Processing received packets.
- Managing send and receive queues.
- Processing synchronization data.
- Handling network connection state.
- Managing asynchronous network operations.

---

# Project Structure

```text
HSO_Client/
│
├── Assets/
│   ├── Animations/
│   ├── Plugins/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   ├── Settings/
│   ├── Sprites/
│   ├── TextMesh Pro/
│   └── StreamingAssets/
├── Packages/
├── ProjectSettings/
├── .gitignore
├── README.md
└── packages.config
```

---

# System Requirements

- Windows 10/11
- Unity 6
- Visual Studio 2022
- Git
- GitHub

The Unity Client requires a running HSO_Server to access multiplayer functionality.

The Server also requires the HSO_WebAPI and Microsoft SQL Server for persistent data operations.

---

# Installation Guide

## 1. Clone the repository

Clone the Unity Client repository:

```bash
git clone https://github.com/mduy23042005/HSO_GameServer.git
```

The Server and Web API are maintained in separate repositories.

Clone the backend repositories if they are not already available:

```bash
git clone https://github.com/mduy23042005/HSO_Server.git
git clone https://github.com/mduy23042005/HSO_WebAPI.git
```

## 2. Open the Unity Project

- Open Unity Hub.
- Select Add project from disk.
- Select the cloned HSO_Client folder.
- Open the project using the required Unity version.
- Wait for Unity to import all project assets and packages.

## 3. Configure the Server Connection

The Unity Client communicates with the Server through WebSocket.
Configure the Server IP address and port in StreamingAssets/ServerConfig.json.

## 4. Start the Web API

- Open the HSO_WebAPI project in Visual Studio 2022.
- Configure SQL Server and start the Web API.
- Start the WebAPI project.
- The default Web API endpoint is: http://localhost:55555

## 5. Start the Server

- Open the HSO_Server project in Visual Studio 2022.
- Make sure the Server is configured to communicate with the Web API.
- Start the Server project.
- The default WebSocket endpoint is: ws://localhost:55556

## 6. Run the Unity Client

After the Web API and Server are running:

- Open the HSO Client project in Unity.
- Open the main game scene.
- Press Play in the Unity Editor.

The Unity Client will establish a WebSocket connection to the Server.

---

# Client-Server Communication

## Unity Client <-> Server

The Unity Client communicates with the Server using WebSocket. The Server is responsible for validating and processing the received requests before sending the appropriate response or synchronization data back to the client.

This connection is used for real-time gameplay data such as:

- Login requests.
- Registration requests.
- Character data.
- Player movement.
- Player state.
- Player appearance.
- Other player synchronization.
- Mob synchronization.
- NPC data.
- Inventory data.
- Equipment data.
- Map data.
- Game state updates.
- Real-time gameplay events.

## Network Synchronization

The Unity Client uses real-time synchronization to maintain a consistent game state between connected players.

### Player Synchronization

The client receives synchronization data for other connected players, including:

- Player position.
- Player direction.
- Player state.
- Character appearance.
- Sprite information.

The client uses this information to update the corresponding remote player objects.

### Mob Synchronization

The client receives mob state information from the Server.

Synchronization includes:

- Mob position.
- Mob direction.
- Mob state.
- Mob movement.
- Mob attack state.
- Mob death state.
- Mob respawn state.

The Server remains authoritative over mob AI and game state.

The Unity Client is responsible for displaying the synchronized state.

## Game Systems

### Character System

The character system manages the player's visual representation and state.

Features include:

- Character creation.
- Character appearance.
- Character movement.
- Character state.
- Character direction.
- Character animation.
- Character appearance synchronization.

The client uses Unity's animation and 2D rendering systems to display the character.

### Movement System

The movement system handles player movement and movement-related input.

Main features include:

- Keyboard input.
- Player movement.
- Direction handling.
- Map movement.
- Movement synchronization.
- Click-to-move interaction.
- Pathfinding support.

### Animation System

The animation system uses Unity's animation framework to display character and mob states.

Main technologies include:

- Animator.
- SpriteRenderer.
- SpriteResolver.
- SpriteLibrary.
- Animation Clips.

Animation state information can be synchronized from the Server to ensure that remote players display consistent animations.

### Map System

The map system manages the game world and its visual representation.

Main features include:

- 2D map rendering.
- Tilemap rendering.
- Map loading.
- Map collision.
- Map navigation.
- Minimap rendering.
- Full minimap display.

Map data can be loaded from the backend and processed by the Unity Client.

### Minimap System

The client provides a minimap for navigation and world visualization.

Main features include:

- Real-time player position.
- Other player markers.
- Map visualization.
- Full minimap.
- Minimap interaction.

### Inventory & Equipment

The client provides UI and interaction systems for inventory and equipment management.

Features include:

- Inventory display.
- Item information.
- Item attributes.
- Equipment slots.
- Equipment display.
- Character appearance updates.

Persistent inventory and equipment data are managed by the backend.

### Mob System

The client manages the visual representation and synchronization of mobs.

Main features include:

- Mob spawning.
- Mob movement.
- Mob animation.
- Mob state synchronization.
- Mob attack presentation.
- Mob death presentation.
- Mob respawn presentation.

Mob AI and authoritative mob state are handled by the Server.

### UI System

The UI system manages all user interface elements.

Main UI components include:

- Login.
- Registration.
- Character creation.
- Character information.
- Inventory.
- Equipment.
- Game HUD.
- Minimap.
- Notifications.
- Game menus.

Text rendering is implemented using TextMeshPro.

### Performance Optimizations

The Unity Client uses several techniques to reduce unnecessary processing and network overhead.

These include:

- Asynchronous WebSocket communication.
- Send and receive queues.
- Binary packet processing.
- Network synchronization at controlled tick rates.
- Client-side caching.
- Object reuse where appropriate.
- Separation of network processing from presentation logic.
- Reduced string usage in frequently transmitted synchronization data.

The client is designed to support multiple synchronized game entities while maintaining stable frame performance.

---

# Current Features

## Account

- User registration.
- User authentication.
- Login UI.
- Registration UI.

## Character

- Character creation.
- Character information.
- Character movement.
- Character appearance.
- Character animation.
- Character state synchronization.

## Inventory & Equipment

- Inventory UI.
- Inventory management.
- Equipment UI.
- Equipment management.
- Item information.
- Item attribute display.

## World

- 2D map rendering.
- Tilemap.
- Map loading.
- Map navigation.
- Minimap.
- Full minimap.
- Mob rendering.
- Mob synchronization.
- NPC rendering.

## Multiplayer

- WebSocket communication.
- Multiple connected clients.
- Player synchronization.
- Character appearance synchronization.
- Real-time game state synchronization.

## Gameplay

- Player movement.
- Mob AI presentation.
- Mob movement synchronization.
- Mob attack presentation.
- Mob death synchronization.
- Mob respawn synchronization.

---

# Future Development
- Quest system.
- Skill system.
- Combat system improvements.
- HP/MP system.
- More advanced character progression.
- NPC interaction system.
- Shop system.
- Party system.
- Guild system.
- Friend system.
- Chat system.
- Improved pathfinding.
- Client performance optimization.
- Network optimization.
- Asset optimization.
- UI/UX improvements.
- Additional gameplay systems.

---

# License

This project was developed for educational and research purposes only, it is inspired by Knight Age by Teamobi and is not intended for commercial use.