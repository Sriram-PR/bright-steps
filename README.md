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

2. Open the project in Unity Hub.

3. Let Unity import assets and packages.

4 .Press Play in the editor to run the game.

### 🎤 Pronunciation Module Setup

Ensure microphone permissions are granted on your device.

Configure your API endpoint:

Open PronunciationManager.cs

Add your Speech API URL and API key.

Build the project for Android/PC.

Test by recording and comparing speech.

⚠️ Note: API calls require internet connectivity. For offline usage, consider on-device speech recognition alternatives.

### 📊 Evaluation Plan

* Pilot testing with 5–10 children with ASD.

* Metrics logged:

     * Reaction times

     * Accuracy scores

     * Usage duration per module

* Feedback sources:

     * Parent/therapist questionnaires

    * System Usability Scale (SUS)

### 🔒 Privacy & Ethics

* All recordings and data must be handled with parental consent.

* No personally identifiable information (PII) should be stored unencrypted.

* Recommended: anonymize user IDs, use secure storage, and comply with local data protection regulations.

### 📌 Roadmap

 * Add adaptive difficulty scaling

 * Improve offline speech recognition (on-device fallback)

 * Therapist/parent dashboard with progress reports

 * More relaxation modules (guided meditation, calming visuals)

 * Cloud-enabled secure leaderboard



### 👨‍💻 Author

#### Saravanan K 
#### Sriram PR
#### Aanand
Project for supporting children with Autism Spectrum Disorder through interactive digital therapy.

### 📜 License

This project is licensed under the MIT License – feel free to use, modify, and distribute with proper attribution.

### 🌟 Acknowledgments

Inspired by therapeutic practices for children with ASD.

Thanks to educators, therapists, and parents who provided feedback.

Unity and open-source tools that made development possible.

---
MADE with LOVE :heart:
