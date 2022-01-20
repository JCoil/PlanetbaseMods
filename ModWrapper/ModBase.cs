using Planetbase;
using System;
using UnityEngine;

namespace ModWrapper
{
    public abstract class ModBase : IMod
    {
        public GameStateGame Game { get; set; }
        public GuiMenuSystem MenuSystem { get; set; }

        /// <summary>
        /// Called early in the game lifecycle. This is where to register strings, create data structures etc.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Called every game update while in GameStateGame
        /// </summary>
        public void Update()
        {
            Game = GameManager.getInstance().getGameState() as GameStateGame;

            // The menu system has been reinitialised, so it looks like we've just come from the Title Menu
            if (Game.mMenuSystem != MenuSystem)
            {
                MenuSystem = Game.mMenuSystem;
                OnGameStart();
            }

            Update(Time.unscaledDeltaTime);
        }

        /// <summary>
        /// Called when we enter GameStateGame after loading. This is where to set up menus, poll modules etc.
        /// </summary>
        public abstract void OnGameStart();

        /// <summary>
        /// Called every game update while in GameStateGame
        /// </summary>
        /// <param name="timeStep">Time period since last update</param>
        public abstract void Update(float timeStep);
    }
}
