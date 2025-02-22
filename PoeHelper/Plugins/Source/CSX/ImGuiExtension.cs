using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ImGuiNET;

namespace CSX
{
    public static class ImGuiExtension
    {
        private static Dictionary<string, bool> popupOpen = new Dictionary<string, bool>();

        public static Keys HotkeySelector(string buttonLabel, Keys currentKey)
        {
            if (!popupOpen.ContainsKey(buttonLabel))
                popupOpen[buttonLabel] = false;

            if (ImGui.Button(buttonLabel))
            {
                popupOpen[buttonLabel] = true;
                ImGui.OpenPopup(buttonLabel);
            }

            if (popupOpen[buttonLabel])
            {
                bool keepOpen = true;
                if (ImGui.BeginPopupModal(buttonLabel, ref keepOpen, ImGuiWindowFlags.AlwaysAutoResize))
                {
                    ImGui.Text("Press any key to set, or ESC for None.");
                    ImGui.Spacing();

                    foreach (Keys testKey in Enum.GetValues(typeof(Keys)))
                    {
                        if (Keyboard.IsKeyDown((int)testKey))
                        {
                            currentKey = (testKey == Keys.Escape) ? Keys.None : testKey;
                            ImGui.CloseCurrentPopup();
                            popupOpen[buttonLabel] = false;
                            ImGui.EndPopup();
                            return currentKey;
                        }
                    }

                    if (ImGui.Button("Cancel"))
                    {
                        ImGui.CloseCurrentPopup();
                        popupOpen[buttonLabel] = false;
                    }

                    ImGui.EndPopup();
                }
                if (!keepOpen) popupOpen[buttonLabel] = false;
            }
            return currentKey;
        }
    }
}