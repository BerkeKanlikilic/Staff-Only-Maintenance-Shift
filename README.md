# Staff Only: Maintenance Shift

**Staff Only: Maintenance Shift** is a short, multiplayer co-op game where players take on the role of night shift staff in a closed café. The goal is to complete the final maintenance tasks before leaving for the night, all within a limited amount of time.

## Game Summary

You begin in the staff room after closing hours. The café is empty, but your shift isn't over yet. To finish the maintenance shift and exit the building, players must work together to complete a series of simple but varied tasks. Time is limited, so teamwork and efficiency are key.

## Core Features

- **Multiplayer Co-op**: Developed using FishNet for smooth LAN-based multiplayer support.
- **Interactive Environment**: Engage with various objects such as:
  - Light switches to power down rooms.
  - Physics-based items to pick up and move.
  - A mop tool to clean up spills or puddles.
- **Task-based Objectives**: Players must complete all assigned tasks to win the game, including turning off lights, cleaning, organizing items, and locking the exit door.

## Technology Stack

- **Engine**: Unity 6
- **Networking**: FishNet v4.6.7 (Host mode setup)

## How to Run

1. Open the project using Unity 6.
2. Press **Play** to run the game. The first instance will automatically become the host/server.
3. Launch additional instances to connect as clients. They will automatically join the host if it's already running.

> No manual server setup is required. FishNet is configured to automatically host on the first launch.

## Known Limitations

- **Reconnection Not Supported**: Disconnecting and rejoining mid-session is currently not functional due to time constraints during development.
- **Polish & Edge Case Bugs**: As this is a short demo built under a tight deadline, some minor issues may exist.

## Development Notes

This project was solo developed over the course of four days (Wednesday to Tuesday, excluding the weekend) for a case study/demo. The focus was on creating a clean, modular architecture with working multiplayer interactions under limited time.
