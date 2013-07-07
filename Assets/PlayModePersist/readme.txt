PlayModePersist Readme - AlmostLogical Software - support@almostlogical.com

Overview
------------
To open the PlayModePersist window, simply navigate to Window->PlayModePersist. This will dock a window with the Inspector. 
In Unity 3.5 or greater this will occur automatically on opening a project where PlayModePersist is not already open.
Now while in Play Mode, when you have a GameObject(s) selected, simply click over to the PM-Persist tab. Now selected the desired checkboxes for each component of each GameObject you wish to persist and then click stop. Your changes will then automatically persist.
The same works if you have multiple GameObjects selected, allowing you to select multiple components to persist at once.

If you have any questions/issues or feature requests please feel free to contact us at support@almostlogical.com.

New to Version 2.0
------------------
-PlayModePersist is now window based (no more time consuming dropdown). PlayModePersist window should automatically dock beside the inspector (feature requires Unity 3.5 or greater). If not simply navigate to Window->PlayModePersist to open.
-Multi-Object Editing is now supported (requires Unity 3.5 or greater). Simply select multiple GameObjects and click the component checkboxes to persist that component across all those GameObjects)
-Auto Persist Window upgrades (can now be used reliability during PlayMode).
-Improved shortcut - Persist Selected GameObject(s) (Alt+Shift+P) - now applies to all components of all selected GameObjects. (requires Unity 3.5 for multiple GameObjects with one click)
-No more 3rd party plugins/editor scripts conflicts in sharing the Transform custom inspector (PlayModePersist is now window based making using PlayModePersist even easier to use)
-Other misc bug fixes

Bug Fix in Version 1.5.1
------------------------
- Changed shortcut keys for open/closing PlayModePersist dropdown and persisting all components within selected GameObject to Shift+Alt+O and Shift+Alt+P respectively.

New to Version 1.5
-------------------------
- Shortcut key : Open/closing the PlayModePersist dropdown : Shift+Alt+O
- Shortcut key : Persisting all components within selected GameObject : Shift+Alt+P
- Auto Persist Settings - Ability to search for and add components you would like to always auto persist in all your projects (Window -> PlayModePerist Auto Persist Settings)
- Fix : Issue persisting multiple GameObjects at the same time
- Bug Fixes : Cleaning up warning and error messages that can occur since Unity 3.2

How to Use Auto Persist
-------------------------------
Go to Window -> PlayModePerist Auto Persist Settings. This window will allow you to select which components you wish to always auto persist within your project. You can scroll through the list and click the add button for each component you wish to Auto Persist. You can also filter the search by typing the start of the components name into the search field above. If you wish to see which components you are currently persisting, you can click the checkbox at the bottom. This will allow you to easily remove any components you no longer want to auto persist. 
All checked auto persist components will persist when stop is clicked.
An important note, as there is no difference between components changed in the editor vs by code, if you change an auto persisting option programatically this value will also persist.


Troubleshooting
--------------------
Q: Where is the PlayModePersist dropdown? It's doesn't show in the Transform like before.
A: Starting with PlayModePersist Version 2.0 there is no longer a dropdown in the Transform. 
   PlayModePersist is now window based and should automatically appear docked with the Inspector in Unity 3.5. If not click Window->PlayModePersist to open.
   This change will improve your workflow and allows for new features like the new Multi-Object edit.
Q: When check a checkbox(s) and clicking stop my settings to not persist
A: First try removing all plugins/editor scripts from your project to see if that resolves the problem. Next make sure you are not switching between scenes in code. For example if you switch to another scene in code you are not longer modifying the same GameObjects as were in the editor. After clicking play if your code load another scene, then reloads the current scene these will also be new copies GameObject compared to what was in the editor so you will be unable to persist changes.
Q: Shortcut Key is not working.
A: Try opening the edit menu and see if the menu option appear in that list. Sometimes just opening the menu for the first time is required to activate the shortcut keys.
Q: Selecting multiple GameObjects to persist multiple components is not working.
A: This feature (Multi-Object Edit) requires Unity 3.5 or greater.
Q: PlayModePersist window is showing 'Failed to load', what is wrong?
A: There is a bug within the Unity Editor that causes custom editor windows fail to load when switching projects. In Unity 3.4 simply close the window and reopen PlayModePersist. In Unity 3.5 it should automatically close all failed windows and reopen and dock PlayModePersist to the inspector.
Q: I don't want PlayModePersist to auto load. Can this be disabled?
A: Yes it can. The auto load feature is for convenience, particularly when moving between projects but in certain cases this might not be desired functionality.
   Simply delete PPAutoOpen.cs (located in PlayModePersist/Editor folder) from your project will remove this feature.
   
If you are still having problems please contact us at: support@almostlogical.com.

Advanced Users
----------------------
If you wish for an individual property to never persist (be it within a built-in Unity component or a custom component), you can manually add the to the code at simply add that property and class as an exception at the bottom of PPLocalStorageManager.cs within PlayModePersist/Editor. IMPORTANT NOTE: Updates may cause this to be overwritten so keep a backup available. Please let me know if you need to do this often as we will add creating a settings window for this task to our features list.

Thanks for purchasing PlayModePersist!

If you have any questions/issues or feature requests please feel free to contact us at support@almostlogical.com.