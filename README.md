# Staff Only: Maintenance Shift

**Staff Only: Maintenance Shift** is a short multiplayer simulation game prototype developed as part of a case study. Players take on the role of a night shift worker tasked with completing essential maintenance duties before closing up a café.

You begin your shift in the staff room after the café has closed. With limited time, you must complete all assigned tasks to successfully end your shift and exit the building.

---

## Features

- First-person multiplayer gameplay with basic task-based objectives
- Interactable objects including:
  - Light switches
  - Physics-based items
  - A mop tool for cleaning spills
- Objective system tracking progress across all players
- Time-limited gameplay session to simulate pressure and urgency

---

## Technology Stack

- **Engine**: Unity 6
- **Networking**: FishNet 4.6.7
- **Architecture**: Server-authoritative design

---

## Running the Project

1. Clone or download the project to your local machine.
2. Open the project using **Unity 6.1.0** or newer.
3. Use Multiplayer Play Mode.
4. Press **Play** in the editor or build the project.
   - The **first player to run the game becomes the Host** automatically (FishNet's Host mode).
   - Additional players can connect as clients by launching a second instance of the game.
5. Begin completing tasks cooperatively to finish the maintenance shift.

> Note: Disconnecting and rejoining during a session is currently unsupported due to time constraints during development.

---

## Development Notes

This prototype was developed solo over the course of four days (from Wednesday to Tuesday, excluding the weekend). The goal was to demonstrate core multiplayer functionality, task interaction systems, and synchronized state management using FishNet.

---

## License

This project was created for demonstration purposes and is not intended for commercial release.
