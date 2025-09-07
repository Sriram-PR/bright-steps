# 🎮 BrightSteps – Autism Support Game (Unity + C#)

BrightSteps is a **multi-feature educational and therapeutic game** developed in Unity for children with Autism Spectrum Disorder (ASD).  
It combines fun, structured gameplay with learning tools to help children practice **pronunciation, colors, music, breathing exercises, and cognitive skills** in a supportive environment.

---

## ✨ Features

- 🎨 **Color Match**
  - Level 1: Match color word to color.
  - Level 2: Stroop test variant (word vs ink-color inhibition training).

- 🎹 **Piano**
  - Simple 13-key digital piano.
  - Helps with auditory and motor coordination.

- ⚡ **Reaction Test**
  - Visual stimulus → user taps quickly.
  - Tracks reaction speed and attention.

- 🌬️ **Breathing Exercises**
  - Box Breathing (4-4-4).
  - 4-7-8 Breathing.
  - Visual + audio guides for relaxation and self-regulation.

- 🗣️ **Word Pronunciation**
  - Plays a target word (TTS).
  - Records user voice via microphone.
  - Sends audio to external Speech Analysis API (Google/Azure style).
  - Provides feedback score and improvement tracking.

- 🔤 **Alphabet Learning**
  - Interactive A–Z letters with sound playback.
  - Supports early literacy and recognition.

- 🏆 **Leaderboard**
  - Encourages engagement with simple gamification.
  - Can be adapted for local or cloud storage.

---

## 🛠️ Tech Stack

- **Engine:** [Unity](https://unity.com/) (2022.3 LTS)
- **Language:** C#
- **Frameworks & APIs:**
  - Unity Input System
  - Unity Microphone API
  - External Speech Recognition / Analysis API (cloud)
- **Assets:** Custom-designed in Photoshop + Unity assets
- **Version Control:** Git + GitHub

---

## 📂 Project Structure

```
bright-steps/
│
├── Assets/ # Game assets (scripts, prefabs, scenes, UI, sounds)
├── Packages/ # Unity packages
├── ProjectSettings/ # Unity project configuration
│ ├── AudioManager.asset
│ ├── InputManager.asset
│ ├── GraphicsSettings.asset
│ └── ... etc
├── .gitignore
└── README.md # You are here

```


---

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 (or later, LTS recommended)
- .NET / C# 8.0+
- Git

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/SaRaVaNaN0504/bright-steps.git
