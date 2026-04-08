# Whack-a-Mole VR

**Auteur :** Léandre Kanmegne  
**Cours :** Environnements Immersifs — Cégep de Victoriaville  
**Session :** Hiver 2026

---

## Description

Application de réalité virtuelle pour Meta Quest 2 développée avec Unity 6.3. Le joueur attrape un marteau et frappe des cibles qui apparaissent et disparaissent dans l'environnement. Chaque cible frappée rapporte des points. La partie se déroule sur 60 secondes, après quoi le score final est affiché et le joueur peut recommencer.

---

## Versions et packages

| Outil                     | Version                         |
| ------------------------- | ------------------------------- |
| Unity                     | 6000.3.5f1                      |
| XR Interaction Toolkit    | 3.3.1                           |
| XR Plugin Management      | 4.5.4                           |
| XR Core Utilities         | 2.5.3                           |
| Input System              | 1.17.0                          |
| Universal Render Pipeline | 17.3.0                          |
| Plateforme cible          | Meta Quest 2 (Android, API 29+) |
| Architecture              | ARM64, IL2CPP                   |

**Asset Store**

- Free Pop Sound Effects Pack 1.0 — sons d'interaction (apparition des cibles, impact)

---

## Fonctionnalités implémentées

- Grab cinématique du marteau via XR Interaction Toolkit
- Détection des coups par contact physique (OnTriggerEnter)
- Spawn dynamique des cibles à positions aléatoires
- Disparition automatique des cibles non frappées
- Boucle de jeu complète : menu -> partie -> fin -> rejouer
- Score et minuterie en temps réel
- UI en World Space (aucun Canvas en Screen Space)
- Retour haptique distinct au grab et à l'impact
- Son spatial lié à l'apparition des cibles et à l'impact du marteau

---

## Structure des scripts

| Script                 | Responsabilité                                                |
| ---------------------- | ------------------------------------------------------------- |
| `GestionnaireJeu.cs`   | États de jeu, score, minuterie, gestion des panneaux UI       |
| `GestionnaireSpawn.cs` | Création et destruction dynamique des cibles                  |
| `Cible.cs`             | Détection du coup, déclenchement de l'event, son d'apparition |
| `FeedbackMarteau.cs`   | Retour haptique au grab et à l'impact, son d'impact           |

La communication entre `Cible` et `GestionnaireJeu` passe par un event C# statique (`Action<int> OnCibleTouchee`) pour découpler les deux scripts.

---

## Défis rencontrés et solutions

**UI en World Space** — La configuration du Canvas en mode World Space avec le Tracked Device Graphic Raycaster et le XR UI Input Module sur l'EventSystem n'était pas évidente. La solution a été de remplacer le Graphic Raycaster standard par le Tracked Device Graphic Raycaster et de substituer le Standalone Input Module par le XR UI Input Module pour permettre l'interaction via les contrôleurs.

**Interaction XR Toolkit** — Comprendre la distinction entre le Near-Far Interactor (géré automatiquement par le toolkit) et les raycast manuels via Physics.Raycast a demandé de l'exploration. Dans ce projet, le grab du marteau passe entièrement par le toolkit tandis que la détection des coups repose sur la collision physique directe.

**Déploiement sur casque** — Le premier build IL2CPP pour Android ARM64 prend considérablement de temps. L'utilisation du cache entre les builds suivants et la validation systématique sur le casque avant la remise finale ont permis d'éviter les surprises de dernière minute.

---

## Utilisation de l'IA

L'IA (Claude, Anthropic) a été utilisée comme outil d'apprentissage et d'assistance au développement, conformément au niveau d'utilisation autorisé dans le cadre du cours. Concrètement :

- Aide à la compréhension des concepts théoriques (raycasting, quaternions, XR Toolkit, events C#)
- Suggestions de structure de code et révision des scripts
- Débogage et identification d'erreurs de compilation
- Guidance sur la configuration Unity (Canvas World Space, XR UI Input Module)

Toutes les décisions d'implémentation, la configuration de la scène Unity et la compréhension du code sont de ma responsabilité.

---

## Sources

- Cégep de Victoriaville. _Travail pratique — Whack-a-Mole VR_. Environnements Immersifs, 2026.
- Cégep de Victoriaville. _Exercice 4 — Tri spatial VR : Grab & Socket_. Environnements Immersifs, 2026.
- Cégep de Victoriaville. _Exercice 4.1 — Feedback VR : haptiques et audio spatial_. Environnements Immersifs, 2026.
- Cégep de Victoriaville. _Exercice 4.2 — UI VR et GameManager_. Environnements Immersifs, 2026.
- Cégep de Victoriaville. _Configuration VR dans Unity_. Environnements Immersifs, 2026.
- Unity Technologies. _XR Interaction Toolkit Documentation_. docs.unity3d.com, 2024.
