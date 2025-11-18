# Emoji Battle  
A mobile puzzle-battle game built in **Unity 6**.  
Gameplay is inspired by classic tic-tac-toe mechanics but enhanced with emoji customization, AI opponents, progression and unlockable content.

## 🎮 Features

### ✅ Implemented  
- Custom Emoji System: 9 colours × 87 emojis with unlocks and rarity.  
- AI Opponents: 3 difficulty levels (Easy / Normal / Hard) via `IAIStrategy`.  
- Win / Lose / Draw Logic: clear separation of board logic, turn manager, and win checker.  
- Player Progression (basic): unlockable emojis and colours; progress saved via JSON/PlayerPrefs.  
- Mobile-Ready UI: lobby, emoji selection, popups, animations.  
- Clean C# Architecture: SOLID principles, ScriptableObjects for emoji data, event-driven communication, minimal coupling.  
- Optimized for Mobile (initial): lightweight assets, minimal GC spikes.

### 🔧 In Progress  
- Ads integration (rewarded & interstitial)  
- Loot-box reward popup system  
- WebGL port  
- Sound effects & full audio mixer setup  

### 🧭 Planned  
- Google Play release & analytics integration  
- Multiplayer mode (match-vs-match)  
- Leaderboards & social sharing  
- iOS port  

## 🧠 Tech & Architecture  
- Unity 6  
- C#  
- Use of ScriptableObjects for emoji data  
- `IAIStrategy` interface for AI plug-ins  
- Event-driven UI system  
- Async/Coroutines for animations/delays  
- Player progress stored via JSON/PlayerPrefs  
- Editor tools for testing emoji sets (optional)  

## 📂 Project Structure  
/Assets
/Scripts
/Core
/AI
/Board
/UI
/Progress
/SO (ScriptableObjects)
/Sprites
/Prefabs

## 🚀 How to Build / Run  
1. Install Unity 6 (latest).  
2. Clone: `git clone https://github.com/SD7games/Emoji_Battle.git`  
3. Open project via Unity Hub → select Unity 6.  
4. Scenes available: `Lobby`, `Game`.  
5. Platforms: Android (default), WebGL (experimental), iOS (planned).

## 📸 Screenshots

<p align="center">
  <img src="https://github.com/user-attachments/assets/4b3916c1-822c-4724-bc4f-90469bbe0df3" width="260" />
  <img src="https://github.com/user-attachments/assets/3493ee22-de05-47d3-b13f-1c08b4642b29" width="260" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/6b700aa7-0317-4c1d-90a9-dc624bfc6bd8" width="260" />
  <img src="https://github.com/user-attachments/assets/4d8ac580-c872-4093-9b78-f831097f2fe3" width="260" />
</p>

<p align="center">
  <b>Splash → Loading → Lobby → Gameplay</b>
</p>

## 🛠 Roadmap  
- Polish UI & animations  
- Add ads integration (rewarded & interstitial)  
- Add loot-box reward popup  
- Add sound effects & feedback  
- Final optimization & Google Play release  

## 👨‍💻 Developer  
Oleksandr Tokarev — Unity & C# Game Developer based in Finland.  
Open to work and collaboration.  
