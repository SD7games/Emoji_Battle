# Emoji Battle  
A mobile puzzle-battle game built in **Unity 6**.  
Inspired by classic tic-tac-toe, but expanded with emoji customization, AI opponents, basic progression and a mobile-friendly UI.

---

## 🎮 Features

### ✅ Implemented  
- **Custom Emoji System:** 9 colours × 87 emojis, basic unlock rules, rarity levels.  
- **AI Opponents:** Easy / Normal / Hard difficulty via the `IAIStrategy` interface.  
- **Game Logic:** turn management, win/draw detection, and clearly separated board logic.  
- **Basic Player Progression:** simple unlock system, saved via JSON / PlayerPrefs.  
- **Mobile-Ready UI:** lobby, emoji selection screen, basic popups and animations.  
- **Architecture:** ScriptableObjects for emoji data, event-driven interactions, low coupling.  
- **Initial Mobile Optimization:** lightweight assets, minimal GC allocations.

### 🔧 In Progress  
- Extended progression system (additional unlock conditions, streak-based rewards).  
- Popup system (settings, victory/defeat, progression updates).  
- Rewarded and interstitial ads integration.  
- Loot-box reward popup.  
- Sound effects and full Audio Mixer setup.  

### 🧭 Planned  
- Google Play release with basic analytics.  
- Optional WebGL build.  
- Multiplayer mode (concept).  
- Leaderboards & simple social sharing.  
- iOS build support.

---

## 🧠 Tech & Architecture  
- Unity 6  
- C#  
- ScriptableObjects for emoji data and configuration  
- `IAIStrategy` interface for modular AI behaviour  
- Event-based UI and communication  
- Coroutines for delays and animations  
- Player progress stored in JSON / PlayerPrefs  
- Optional editor tools for emoji testing

---

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

---

## 🚀 How to Build / Run  
1. Install Unity 6 (latest available version).  
2. Clone the repository:  
   `git clone https://github.com/SD7games/Emoji_Battle.git`  
3. Open the project via Unity Hub → select Unity 6.  
4. Main scenes:  
   - `Lobby`  
   - `Game`  
5. Platforms:  
   - Android (primary)  
   - WebGL (experimental, planned)  
   - iOS (planned)

---

## 📸 Screenshots

<p align="center">
  <img src="https://github.com/user-attachments/assets/4b3916c1-822c-4724-bc4f-90469bbe0df3" width="260" />
  <img src="https://github.com/user-attachments/assets/6b700aa7-0317-4c1d-90a9-dc624bfc6bd8" width="260" />  
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/3493ee22-de05-47d3-b13f-1c08b4642b29" width="260" />  
  <img src="https://github.com/user-attachments/assets/4d8ac580-c872-4093-9b78-f831097f2fe3" width="260" />
</p>

<p align="center">
  <b>Splash → Loading → Lobby → Gameplay</b>
</p>

---

## 🛠 Roadmap  
- Improve UI feedback and animations  
- Implement ads (rewarded / interstitial)  
- Add loot-box reward popup  
- Add sound effects and polish audio  
- Final optimization  
- Prepare for Google Play release  

---

## 👨‍💻 Developer  
**Oleksandr Tokarev** — Unity & C# Game Developer based in Finland.  
Open to work and collaboration.  

