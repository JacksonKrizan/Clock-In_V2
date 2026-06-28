**********Clock-In    Made in Unity 2022.3.22**********
===================================================================================

Clock-In is a mutlyplayer game that is supose to simulate different jobs with mini games and mini worlds.

This has been a project worked on by three creators. 
(blayne67)(sigmaJakob)(jacksonkrizan)



*****How to open/run it*****
======================================
Clone or download, then unzip and open the folder in Unity Hub.

The game is also at https://bluedingoes.com/ to run the web version



*****Controls vary on each map, but here are the basics*****
======================================

WASD to move
Space to Jump
Shift to Run
T - Toggle scoreboard
Q - Exit tutorial
Right and Left click to interact
Tab - For in game options
Esc: Quit the game



*****Important if coding*****
======================================

All the game folders are labeled in the "ProjectFiles.txt" file inside the assest folder (Clock-In_V2\Assets\Clock-In_V2\Assets). Use this as reference.

*****Don't rename the Resources folder*****
======================================
The game grabs prefabs from here and can't find the folder if renamed or moved.


Keep Unreliable On Change for player position and rotation syncing — that’s the right setting.


Use Reliable or Reliable Delta Compressed only for data that must always arrive.


Esc quits the game/calls "ExitGame". In launcher.cs, the function "ExitGame" if called closes the game.

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // Quit() no-ops in Editor
        #else
                Application.Quit();
        #endif
            }    public void ExitGame()
            {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; // Quit() no-ops in Editor
        #else
                Application.Quit();
        #endif`
    }



*****Packeges*****
======================================
TextMeshPro     3.0.6    all the text (HUD, scoreboard, menus)
ProBuilder      5.2.4    building level geometry in-editor
Visual Scripting 1.9.4   node-based scripting (Bolt)
Timeline        1.7.6    animation / cutscene sequencing
-uGUI (Unity UI) 1.0.0    canvas, buttons, UI
Collab Proxy    2.7.1    version control
Development feature set  1.0.1  dev/test tools bundle


*****Current game issue*****
======================================
There are no game breaking bugs, as this readme is updated 6/28/2026
Many worlds need work upon.

*****Game Screenshots*****
======================================
![alt text](<Screenshot 2026-06-28 073754.png>)
Menu




![alt text](<Screenshot 2026-06-28 073918.png>)
Network Engineer




![alt text](<Screenshot 2026-06-28 074109.png>)
FireFighter




![alt text](<Screenshot 2026-06-28 074141.png>)
Car Mechanic
