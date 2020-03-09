using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Server;
using Vintagestory.API.Common;
using Vintagestory.GameContent;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using ProtoBuf;

namespace playerskins
{
    public class EntityCustomShapeRenderer : EntityShapeRenderer
    {

        protected int skinTextureSubId;

        public override TextureAtlasPosition this[string textureCode]
        {
            get
            {
                CompositeTexture cpt = null;
                if (extraTexturesByTextureName?.TryGetValue(textureCode, out cpt) == true)
                {
                    return capi.EntityTextureAtlas.Positions[cpt.Baked.TextureSubId];
                }

                return skinTexPos;
            }
        }

        public EntityCustomShapeRenderer(Entity entity, ICoreClientAPI api) : base(entity, api)
        {
            api.Event.ReloadTextures += reloadSkin;

            string[] skinParts = PlayerSkins.skinTypes.Keys.ToArray();
            foreach (string part in skinParts)
            {
                entity.WatchedAttributes.RegisterModifiedListener(part, () =>
                {
                    this.reloadSkin();
                });
            }


        }

        bool textureSpaceAllocated = false;
        protected override ITexPositionSource GetTextureSource()
        {
            if (!textureSpaceAllocated)
            {
                TextureAtlasPosition origTexPos = capi.EntityTextureAtlas.Positions[entity.Properties.Client.FirstTexture.Baked.TextureSubId];
                int width = (int)((origTexPos.x2 - origTexPos.x1) * AtlasSize.Width);
                int height = (int)((origTexPos.y2 - origTexPos.y1) * AtlasSize.Height);

                capi.EntityTextureAtlas.AllocateTextureSpace(width, height, out skinTextureSubId, out skinTexPos);

                textureSpaceAllocated = true;
            }

            return base.GetTextureSource();
        }

        public override void TesselateShape()
        {
            base.TesselateShape();

            if (eagent.GearInventory != null)
            {
                reloadSkin();
            }
        }

        public override void reloadSkin()
        {
            TextureAtlasPosition origTexPos = capi.EntityTextureAtlas.Positions[entity.Properties.Client.FirstTexture.Baked.TextureSubId];

            LoadedTexture entityAtlas = new LoadedTexture(null)
            {
                TextureId = origTexPos.atlasTextureId,
                Width = capi.EntityTextureAtlas.Size.Width,
                Height = capi.EntityTextureAtlas.Size.Height
            };

            /* capi.Render.GlToggleBlend(false);
             capi.EntityTextureAtlas.RenderTextureIntoAtlas(
                 entityAtlas,
                 (int)(origTexPos.x1 * AtlasSize.Width),
                 (int)(origTexPos.y1 * AtlasSize.Height),
                 (int)((origTexPos.x2 - origTexPos.x1) * AtlasSize.Width),
                 (int)((origTexPos.y2 - origTexPos.y1) * AtlasSize.Height),
                 skinTexPos.x1 * capi.EntityTextureAtlas.Size.Width,
                 skinTexPos.y1 * capi.EntityTextureAtlas.Size.Height,
                 -1
             ); */

            capi.Render.GlToggleBlend(true, EnumBlendMode.Overlay);

            int[] skinRenderOrder = new int[]
            {
                (int)EnumSkinPart.skincolor,
                (int)EnumSkinPart.eyecolor,
                (int)EnumSkinPart.facialhair,
                (int)EnumSkinPart.hairtype,
            };

            for (int x = 0; x < skinRenderOrder.Length; x++)
            {
                string skinProperty = Enum.GetName(typeof(EnumSkinPart), x);
                string componentPath = "playerskins:textures/entity/skin/";
                float alphaTest = 0.005f;
                if (skinProperty == "haircolor")
                {
                    continue;
                }
                if (skinProperty == "hairtype" || skinProperty == "facialhair")
                {
                    if (entity.WatchedAttributes.GetString(skinProperty, "none") == "none")
                    {
                        continue;
                    }
                }
                switch (skinProperty)
                {
                    case "skincolor":
                        componentPath += "skins/" + entity.WatchedAttributes.GetString("sex", "male") + "/" + entity.WatchedAttributes.GetString("skincolor", "tan") + ".png";
                        alphaTest = -1f;
                        break;
                    case "eyecolor":
                        componentPath += "eyes/" + entity.WatchedAttributes.GetString("eyecolor", "brown") + ".png";
                        break;
                    case "facialhair":
                        componentPath += "facialhairs/" + entity.WatchedAttributes.GetString("facialhair", "full") + "-" + entity.WatchedAttributes.GetString("haircolor", "brown") + ".png";
                        break;
                    case "hairtype":
                        componentPath += "hairs/" + entity.WatchedAttributes.GetString("hairtype", "m") + "-" + entity.WatchedAttributes.GetString("haircolor", "brown") + ".png";
                        break;
                }


                int textureSubID;
                LoadedTexture componentTexture = new LoadedTexture(capi);
                TextureAtlasPosition texPos = null;
                AssetLocation componentLoc = new AssetLocation(componentPath);
                BitmapRef bitMap = capi.Assets.Get(componentLoc).ToBitmap(capi);
                capi.Render.GetOrLoadTexture(componentLoc, bitMap, ref componentTexture);

                capi.EntityTextureAtlas.AllocateTextureSpace(bitMap.Width, bitMap.Height, out textureSubID, out texPos);

                capi.Render.GlToggleBlend(false);

                capi.EntityTextureAtlas.RenderTextureIntoAtlas(
                    componentTexture,
                    0,
                    0,
                    128,
                    128,
                    skinTexPos.x1 * capi.EntityTextureAtlas.Size.Width,
                    skinTexPos.y1 * capi.EntityTextureAtlas.Size.Height, alphaTest
                );

                capi.Render.GlToggleBlend(true, EnumBlendMode.Overlay);
            }

            // Standard

            int[] renderOrder = new int[]
            {
                (int)EnumCharacterDressType.LowerBody,
                (int)EnumCharacterDressType.Foot,
                (int)EnumCharacterDressType.UpperBody,
                (int)EnumCharacterDressType.UpperBodyOver,
                (int)EnumCharacterDressType.Waist,
                (int)EnumCharacterDressType.Shoulder,
                (int)EnumCharacterDressType.Emblem,
                (int)EnumCharacterDressType.Neck,
                (int)EnumCharacterDressType.Head,
                (int)EnumCharacterDressType.Hand,
                (int)EnumCharacterDressType.Arm,
                (int)EnumCharacterDressType.Face
            };

            if (gearInv == null && eagent?.GearInventory != null)
            {
                eagent.GearInventory.SlotModified += gearSlotModified;
                gearInv = eagent.GearInventory;
            }

            // Ugly fix
            if (gearInv == null)
            {
                return;
            }

            for (int i = 0; i < renderOrder.Length; i++)
            {
                int slotid = renderOrder[i];

                ItemStack stack = gearInv[slotid]?.Itemstack;
                if (stack == null) continue;

                int itemTextureSubId = stack.Item.FirstTexture.Baked.TextureSubId;

                TextureAtlasPosition itemTexPos = capi.ItemTextureAtlas.Positions[itemTextureSubId];

                LoadedTexture itemAtlas = new LoadedTexture(null)
                {
                    TextureId = itemTexPos.atlasTextureId,
                    Width = capi.ItemTextureAtlas.Size.Width,
                    Height = capi.ItemTextureAtlas.Size.Height
                };

                capi.EntityTextureAtlas.RenderTextureIntoAtlas(
                    itemAtlas,
                    itemTexPos.x1 * capi.ItemTextureAtlas.Size.Width,
                    itemTexPos.y1 * capi.ItemTextureAtlas.Size.Height,
                    (itemTexPos.x2 - itemTexPos.x1) * capi.ItemTextureAtlas.Size.Width,
                    (itemTexPos.y2 - itemTexPos.y1) * capi.ItemTextureAtlas.Size.Height,
                    skinTexPos.x1 * capi.EntityTextureAtlas.Size.Width,
                    skinTexPos.y1 * capi.EntityTextureAtlas.Size.Height
                );
            }

            capi.Render.GlToggleBlend(true);
            capi.Render.BindTexture2d(skinTexPos.atlasTextureId);
            capi.Render.GlGenerateTex2DMipmaps();

        }


        public override void Dispose()
        {
            base.Dispose();

            capi.Event.ReloadTextures -= reloadSkin;
            if (eagent?.GearInventory != null)
            {
                eagent.GearInventory.SlotModified -= gearSlotModified;
            }

            capi.EntityTextureAtlas.FreeTextureSpace(skinTextureSubId);
        }
    }
}