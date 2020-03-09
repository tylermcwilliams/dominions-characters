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
    public class ServerSkinNetwork
    {
        ICoreServerAPI sapi;
        IServerNetworkChannel serverNetworkChannel;

        public ServerSkinNetwork(ICoreServerAPI api)
        {
            this.sapi = api;

            this.serverNetworkChannel = api.Network.RegisterChannel("playerskins");
            this.serverNetworkChannel.RegisterMessageType(typeof(SkinChange));
            this.serverNetworkChannel.SetMessageHandler<SkinChange>(SaveSkin);
        }

        public void SaveSkin(IServerPlayer player, SkinChange change)
        {
            player.Entity.WatchedAttributes.SetString(change.part, change.variant);
        }
    }
}
