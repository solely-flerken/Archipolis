# 🧾 Devlog – Archipolis

## 📅 Date: 2025-05-15
I’m starting this devlog a bit later into the project, since I initially didn’t expect it to grow this much.
As the scope has expanded, I realized that maintaining a devlog could help me organize my thoughts, 
track progress, and plan future features more effectively. So here we are.

At this point, the core systems are already in place: building and preview mechanics are functional,
basic map generation is implemented, and the main game menu is up and running. 
There's also a working saving system, a dynamic UI, and proper resource management in place.

### ✅ Completed
- Created `StartMenu`, `LoadingScene`, and `MainScene`.
- Implemented basic scene switching.
- Implemented some progress tracking for game initialization.

### 🔧 In Progress
- Asynchronous loading with progress bar, especially for the map generation.

### 📌 Planned
- Improve map generation to include trees, grass, and ore deposits.
- Get some assets to finally give the game a soul.

---

## 🧠 Notes & Thoughts
- Do we care about cheating? Save files are easily editable right now.
- What about implementing a local "inventory" for buildings?

---

## 🔖 Milestones
- [x] Core game loop functionalities
- [x] Saving/Loading game progress
- [x] Procedural map generation
- [ ] Scene loading with progress tracking
- [ ] Assets (Models for Buildings)