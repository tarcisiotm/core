using System;
using UnityEngine;

namespace TG.Core
{
    public enum GameState 
    {
        None = 0,
        MainMenu,
        Gameplay, 
        Paused, 
        CutScene,
        LoadingScene, 
    }

    public class GameStateManagerBase : ModuleBase
    {
        public GameState CurrentState { get; private set; } = GameState.None;
        public GameState PreviousState { get; private set; } = GameState.None;

        public delegate void StateChanged(GameState previousGameState, GameState newGameState);
        public static StateChanged OnGameStateChanged;

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) { return; }

            PreviousState = CurrentState;
            CurrentState = newState;

            HandlePause();

            OnGameStateChanged?.Invoke(PreviousState, newState);
        }

        #region Pause
        protected virtual void HandlePause()
        {
            if (CurrentState == GameState.Paused)
            {
                Time.timeScale = 0f;
            }
            else if (PreviousState == GameState.Paused)
            {
                Time.timeScale = 1f;
            }
        }

        public void Pause(bool pause) 
        {
            if (pause && CurrentState == GameState.Paused) { return; }
            if (pause && CurrentState != GameState.Paused)
            {
                SetState(GameState.Paused);
            }
            else if (!pause && CurrentState == GameState.Paused) 
            {
                GameState targetState = (PreviousState == GameState.Paused || PreviousState == GameState.None)
                         ? GameState.Gameplay
                         : PreviousState;
                SetState(targetState);
            }
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Paused)
            {
                GameState targetState = (PreviousState == GameState.Paused || PreviousState == GameState.None)
                                         ? GameState.Gameplay
                                         : PreviousState;
                SetState(targetState);
            }
            else
            {
                SetState(GameState.Paused);
            }
        }
        #endregion Pause

    }
}