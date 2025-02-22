using System;
using System.Collections;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;

namespace ExileCore
{
    public class AreaController
    {
        private const string areaChangeCoroutineName = "Area change";

        public AreaController(TheGame theGameState)
        {
            TheGameState = theGameState;
        }

        public TheGame TheGameState { get; }
        public AreaInstance CurrentArea { get; private set; }
        public event Action<AreaInstance> OnAreaChange;

        public void ForceRefreshArea(bool areaChangeMultiThread)
        {
            var ingameData = TheGameState.IngameState.Data;
            var clientsArea = ingameData.CurrentArea;
            var curAreaHash = TheGameState.CurrentAreaHash;
            CurrentArea = new AreaInstance(clientsArea, curAreaHash, ingameData.CurrentAreaLevel);
            if (CurrentArea.Name.Length == 0) return;
            ActionAreaChange();
        }

        public bool RefreshState()
        {
            var ingameData = TheGameState.IngameState.Data;
            var clientsArea = ingameData.CurrentArea;
            var curAreaHash = TheGameState.CurrentAreaHash;

            if (CurrentArea != null && curAreaHash == CurrentArea.Hash)
                return false;

            CurrentArea = new AreaInstance(clientsArea, curAreaHash, ingameData.CurrentAreaLevel);
            if (CurrentArea.Name.Length == 0) return false;
            ActionAreaChange();
            return true;
        }

        //Before call areachange for plugins need wait some time because sometimes gam,e memory not ready because still loading.
        private IEnumerator CoroutineAreaChange(bool areaChangeMultiThread)
        {
            yield return new WaitFunction(() => TheGameState.IsLoading /*&& !TheGameState.InGame*/);

            //   yield return new WaitTime((int)(TheGameState.IngameState.CurLatency));
        }

        private void ActionAreaChange()
        {
            OnAreaChange?.Invoke(CurrentArea);
        }
    }
}
