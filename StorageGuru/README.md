**Changes in Version 2.1**

Saving and loading of storage manifests now happens automatically with the game saving and loading. This is vastly better than before, but will have little impact on gameplay:

- The manifests are stored within the game's save file (Documents\Planetbase\Saves)
- This should eliminate the issue where the wrong manifest is occasionally loaded for a save file
- Individual manifest files within StorageGuruData are no longer needed
- I've kept code to handle legacy manifest files, but once save file has been loaded and saved again, these can be deleted

**Changes in Version 2**

I've finally got around to fixing this mod! I've made a lot of changes and improvements so hopefully it will be a lot more stable than in the past:

- Major rewrite for storage manifest management data structure
- Improve storage targeting code to remove hacky + slow character redirection
- Fix manifest serialization, saving and loading - manifests are now stored per savegame
- Improve error and exception handling
