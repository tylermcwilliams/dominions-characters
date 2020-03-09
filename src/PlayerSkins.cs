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
    public class PlayerSkins : ModSystem
    {
        // Might move to a JSON
        #region Config
        // list of skin parts
        public static Dictionary<string, string[]> skinTypes = new Dictionary<string, string[]>()
        {
            ["skincolor"] = new[] { "brown", "light-brown", "olive", "yellow", "tan", "pale" },
            ["eyecolor"] = new[] { "dark-brown", "brown", "light-brown", "dark-green", "light-green", "deep-blue", "light-blue", "grey", "milky" },
            ["haircolor"] = new[] { "black", "dark-brown", "brown", "red", "blonde", "white" },
            ["hairtype"] = new[] { "none", "m" },
            ["facialhair"] = new[] { "none", "full" },
        };
        // default skin
        public static Dictionary<string, string> defaultSkin = new Dictionary<string, string>()
        {
            ["skincolor"] = "tan",
            ["eyecolor"] = "brown",
            ["haircolor"] = "brown",
            ["hairtype"] = "m",
            ["facialhair"] = "full",
            ["sex"] = "male"
        };
        #endregion

        ClientSkinNetwork clientSkinNetwork;
        ServerSkinNetwork serverSkinNetwork;

        public override void StartClientSide(ICoreClientAPI api)
        {
            this.clientSkinNetwork = new ClientSkinNetwork(api);

            api.RegisterCommand("changeskin", "Opens skin change gui.", "", (int i, CmdArgs args) =>
            {
                GuiDialogSkin skinGui = new GuiDialogSkin(api, this.clientSkinNetwork);
                skinGui.TryOpen();
            });

            api.RegisterEntityRendererClass("CustomRenderer", typeof(EntityCustomShapeRenderer));

            base.StartClientSide(api);
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            this.serverSkinNetwork = new ServerSkinNetwork(api);

            base.StartServerSide(api);
        }
    }

}
