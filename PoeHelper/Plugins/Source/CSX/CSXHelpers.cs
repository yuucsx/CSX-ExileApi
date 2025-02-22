using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;

namespace CSX
{
    public static class CSXHelpers
    {
        public static bool IsChatOpen(GameController gameController)
        {
            var chatPanel = gameController?.IngameState?.IngameUi?.ChatPanel;
            if (chatPanel == null) return false;

            var inputElement = chatPanel.ChatInputElement;
            if (inputElement == null) return false;

            return inputElement.IsVisibleLocal;
        }

        public static int CountEnemiesInRange(GameController gc, List<Entity> enemyEntities, int range)
        {
            var localPlayer = gc.Game?.IngameState?.Data?.LocalPlayer;
            if (localPlayer == null) return 0;

            var myPos2D = new Vector2(localPlayer.PosNum.X, localPlayer.PosNum.Y);

            int count = 0;
            foreach (var monster in enemyEntities)
            {
                var monPos2D = new Vector2(monster.PosNum.X, monster.PosNum.Y);
                float dist = Vector2.Distance(myPos2D, monPos2D);
                if (dist <= range) count++;
            }
            return count;
        }

        public static int CountAlliesInRange(GameController gc, List<Entity> alliedPlayers, int range)
        {
            var localPlayer = gc.Game?.IngameState?.Data?.LocalPlayer;
            if (localPlayer == null) return 0;

            var myPos2D = new Vector2(localPlayer.PosNum.X, localPlayer.PosNum.Y);

            int count = 0;
            foreach (var ally in alliedPlayers)
            {
                var allyPos2D = new Vector2(ally.PosNum.X, ally.PosNum.Y);
                float dist = Vector2.Distance(myPos2D, allyPos2D);
                if (dist <= range) count++;
            }
            return count;
        }
        public static bool IsPlayerBusy(GameController gc)
        {
            var localPlayer = gc?.Game?.IngameState?.Data?.LocalPlayer;
            if (localPlayer == null) return false;

            var actor = localPlayer.GetComponent<Actor>();
            return actor.Action.HasFlag(ActionFlags.UsingAbility);
        }
    }
}
