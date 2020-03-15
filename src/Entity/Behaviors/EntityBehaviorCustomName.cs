using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace dominions.characters
{
    internal class EntityBehaviorCustomName : EntityBehaviorNameTag
    {
        public EntityBehaviorCustomName(Entity entity) : base(entity)
        {
        }

        public new string DisplayName
        {
            get
            {
                return this.entity.WatchedAttributes.GetString("charactername", base.DisplayName);
            }
        }

        public void SetCharacterName(string name)
        {
            if (name == null) return;
            this.entity.WatchedAttributes.SetString("charactername", name);
        }
    }
}
