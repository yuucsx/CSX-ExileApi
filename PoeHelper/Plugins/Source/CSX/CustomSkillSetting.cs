using ExileCore.Shared.Nodes;
using System.Windows.Forms;

namespace CSX
{
    public class CustomSkillSetting
    {
        public CustomSkillSetting()
        {
            Enabled = new ToggleNode(false);
            Key = new HotkeyNode(Keys.None);
            TriggerRange = new RangeNode<int>(300, 0, 3000);
            MinEnemy = new RangeNode<int>(0, 0, 50);
            MinAllies = new RangeNode<int>(0, 0, 50);
            CustomCooldownMs = new RangeNode<int>(1000, 0, 5000);
            InternalName = new TextNode("");
        }

        public ToggleNode Enabled { get; set; }
        public HotkeyNode Key { get; set; }
        public RangeNode<int> TriggerRange { get; set; }
        public RangeNode<int> MinEnemy { get; set; }
        public RangeNode<int> MinAllies { get; set; }
        public RangeNode<int> CustomCooldownMs { get; set; }
        public TextNode InternalName { get; set; }
    }
}