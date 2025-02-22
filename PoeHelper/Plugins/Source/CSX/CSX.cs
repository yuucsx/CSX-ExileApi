using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Numerics;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ImGuiNET;

namespace CSX
{
    public class CSX : BaseSettingsPlugin<CSXSettings>
    {
        private Queue<string> _pendingCasts = new Queue<string>();
        private Dictionary<string, Func<CustomSkillSetting>> _skillMapper;

        // Cache das entities
        private List<Entity> _enemyEntities = new List<Entity>();
        private List<Entity> _alliedPlayers = new List<Entity>();

        // Dicionário do último uso de cada skill
        private Dictionary<string, DateTime> _lastUsedTime = new Dictionary<string, DateTime>();

        public override bool Initialise()
        {
            _skillMapper = new Dictionary<string, Func<CustomSkillSetting>>
            {
                { "Skill1", () => Settings.CustomSkill1 },
                { "Skill2", () => Settings.CustomSkill2 },
                { "Skill3", () => Settings.CustomSkill3 },
                { "Skill4", () => Settings.CustomSkill4 },
                { "Skill5", () => Settings.CustomSkill5 },
                { "Skill6", () => Settings.CustomSkill6 },
            };
            return true;
        }

        public override void Render()
        {
            if (!Settings.Enable.Value || !GameController.InGame || GameController.IsLoading || MenuWindow.IsOpened || !GameController.IsForeGroundCache || CSXHelpers.IsChatOpen(GameController))
                return;

            if (GameController.Area.CurrentArea.IsTown|| GameController.Area.CurrentArea.IsHideout)
                return;

            var localPlayer = GameController.Game.IngameState.Data.LocalPlayer;
            if (localPlayer == null)
                return;

            if (!localPlayer.IsAlive)
                return;

            _enemyEntities = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Monster]
                .Where(x =>
                    x != null
                    && x.IsAlive
                    && x.IsHostile
                    && x.GetComponent<Life>()?.CurHP > 0
                    && x.GetComponent<Targetable>()?.isTargetable == true
                )
                .ToList();

            var myAddr = localPlayer.Address;
            _alliedPlayers = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Player]
                .Where(x => x != null && x.Address != myAddr && !x.IsHostile)
                .ToList();

            if (_pendingCasts.Count > 0)
            {
                if (CSXHelpers.IsPlayerBusy(GameController))
                    return;

                var skillId = _pendingCasts.Peek();
                bool castOk = TryCastSkill(skillId);
                if (castOk)
                {
                    _pendingCasts.Dequeue();
                    return;
                }

                _pendingCasts.Dequeue();
            }

            HandleCustomSkill("Skill1");
            HandleCustomSkill("Skill2");
            HandleCustomSkill("Skill3");
            HandleCustomSkill("Skill4");
            HandleCustomSkill("Skill5");
            HandleCustomSkill("Skill6");
        }

        private void HandleCustomSkill(string skillId)
        {
            var skillCfg = _skillMapper[skillId]();
            if (!skillCfg.Enabled.Value) return;
            if (skillCfg.Key.Value == Keys.None) return;

            if (!IsSkillReady(skillCfg, skillId))
                return;

            if (CSXHelpers.IsPlayerBusy(GameController))
            {
                if (!_pendingCasts.Contains(skillId))
                    _pendingCasts.Enqueue(skillId);
                return;
            }

            CastNow(skillCfg, skillId);
        }

        private bool IsSkillReady(CustomSkillSetting skillCfg, string skillId)
        {
            int enemiesCount = CSXHelpers.CountEnemiesInRange(GameController, _enemyEntities, skillCfg.TriggerRange.Value);
            int alliesCount = CSXHelpers.CountAlliesInRange(GameController, _alliedPlayers, skillCfg.TriggerRange.Value);

            bool enemyOk = (skillCfg.MinEnemy.Value == 0) || (enemiesCount >= skillCfg.MinEnemy.Value);
            bool allyOk = (skillCfg.MinAllies.Value == 0) || (alliesCount >= skillCfg.MinAllies.Value);
            if (!enemyOk || !allyOk)
                return false;

            if (!string.IsNullOrEmpty(skillCfg.InternalName.Value))
            {
                var actorSkills = GameController.Game.IngameState.Data.LocalPlayer
                    ?.GetComponent<Actor>()?.ActorSkills;
                if (actorSkills != null)
                {
                    var foundSkill = actorSkills.FirstOrDefault(s =>
                        s?.InternalName?.Equals(skillCfg.InternalName.Value, StringComparison.OrdinalIgnoreCase) == true
                    );
                    if (foundSkill != null)
                    {
                        if (!foundSkill.AllowedToCast)
                            return false;

                        if (foundSkill.IsVaalSkill && foundSkill.TotalVaalUses <= 0)
                            return false;
                    }
                }
            }

            var dictKey = string.IsNullOrEmpty(skillCfg.InternalName.Value) ? skillId : skillCfg.InternalName.Value;
            if (!_lastUsedTime.ContainsKey(dictKey))
                _lastUsedTime[dictKey] = DateTime.MinValue;

            double elapsedMs = (DateTime.Now - _lastUsedTime[dictKey]).TotalMilliseconds;
            if (elapsedMs < skillCfg.CustomCooldownMs.Value)
                return false;

            return true;
        }

        private void CastNow(CustomSkillSetting skillCfg, string skillId)
        {
            var dictKey = string.IsNullOrEmpty(skillCfg.InternalName.Value) ? skillId : skillCfg.InternalName.Value;
            _lastUsedTime[dictKey] = DateTime.Now;

            Keyboard.KeyPress(skillCfg.Key.Value);
        }

        private bool TryCastSkill(string skillId)
        {
            var skillCfg = _skillMapper[skillId]();
            if (!skillCfg.Enabled.Value) return false;
            if (!IsSkillReady(skillCfg, skillId)) return false;

            CastNow(skillCfg, skillId);
            return true;
        }

        public override void DrawSettings()
        {
            if (ImGui.TreeNode("Custom Skill Usage"))
            {
                DrawCustomSkillUi(Settings.CustomSkill1, "Skill #1");
                DrawCustomSkillUi(Settings.CustomSkill2, "Skill #2");
                DrawCustomSkillUi(Settings.CustomSkill3, "Skill #3");
                DrawCustomSkillUi(Settings.CustomSkill4, "Skill #4");
                DrawCustomSkillUi(Settings.CustomSkill5, "Skill #5");
                DrawCustomSkillUi(Settings.CustomSkill6, "Skill #6");

                ImGui.TreePop();
            }
        }

        private void DrawCustomSkillUi(CustomSkillSetting skill, string label)
        {
            if (ImGui.TreeNode(label))
            {
                bool tmp = skill.Enabled.Value;
                if (ImGui.Checkbox($"Enabled {label}", ref tmp))
                    skill.Enabled.Value = tmp;

                var newKey = ImGuiExtension.HotkeySelector($"Hotkey {label}: {skill.Key.Value}", skill.Key.Value);
                if (newKey != skill.Key.Value)
                    skill.Key.Value = newKey;

                int tmpRange = skill.TriggerRange.Value;
                if (ImGui.SliderInt($"Range {label}", ref tmpRange, 0, 1000))
                    skill.TriggerRange.Value = tmpRange;

                int tmpMinE = skill.MinEnemy.Value;
                if (ImGui.SliderInt($"Min Enemy {label}", ref tmpMinE, 0, 10))
                    skill.MinEnemy.Value = tmpMinE;

                int tmpMinA = skill.MinAllies.Value;
                if (ImGui.SliderInt($"Min Allies {label}", ref tmpMinA, 0, 6))
                    skill.MinAllies.Value = tmpMinA;

                int tmpCd = skill.CustomCooldownMs.Value;
                if (ImGui.SliderInt($"Manual CD (ms) {label}", ref tmpCd, 0, 5000))
                    skill.CustomCooldownMs.Value = tmpCd;

                ImGui.Text("(Optional) InternalName:");
                ImGui.SameLine();
                ImGui.TextDisabled("(?)");
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
                {
                    ImGui.SetTooltip("If you set this skill's exact name (e.g. 'enduring_cry'),\nthe plugin will check in-game IsOnCooldown. If also using manual cooldown,\nboth checks are combined: the skill only fires when it's ready in-game\nAND the manual delay has passed.");
                }

                string strVal = skill.InternalName.Value;
                if (ImGui.InputText($"##InternalName_{label}", ref strVal, 100))
                    skill.InternalName.Value = strVal;

                ImGui.TreePop();
            }
        }
    }
}