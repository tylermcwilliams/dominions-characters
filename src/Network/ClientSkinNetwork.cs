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
    public class ClientSkinNetwork
    {
        ICoreClientAPI capi;
        IClientNetworkChannel clientNetworkChannel;

        public ClientSkinNetwork(ICoreClientAPI api)
        {
            this.capi = api;

            this.clientNetworkChannel = api.Network.RegisterChannel("playerskins");
            this.clientNetworkChannel.RegisterMessageType(typeof(SkinChange));
        }

        public void SendSkinPacket(string newpart, string newvariant)
        {
            clientNetworkChannel.SendPacket(new SkinChange()
            {
                part = newpart,
                variant = newvariant
            });
        }
    }
}
