using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using System.Runtime;

namespace CSX
{
    public class CSXSettings : ISettings
    {
        public CSXSettings()
        {
            Enable = new ToggleNode(false);
            CustomSkill1 = new CustomSkillSetting();
            CustomSkill2 = new CustomSkillSetting();
            CustomSkill3 = new CustomSkillSetting();
            CustomSkill4 = new CustomSkillSetting();
            CustomSkill5 = new CustomSkillSetting();
            CustomSkill6 = new CustomSkillSetting();
        }

        public ToggleNode Enable { get; set; }
        public CustomSkillSetting CustomSkill1 { get; set; }
        public CustomSkillSetting CustomSkill2 { get; set; }
        public CustomSkillSetting CustomSkill3 { get; set; }
        public CustomSkillSetting CustomSkill4 { get; set; }
        public CustomSkillSetting CustomSkill5 { get; set; }
        public CustomSkillSetting CustomSkill6 { get; set; }
    }
}