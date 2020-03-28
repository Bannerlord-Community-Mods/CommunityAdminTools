using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;

namespace CommunityAdminTools
{
    internal class UserManagement
    {
        private readonly AdminTools _adminTools;

        public UserManagement(AdminTools adminTools)
        {
            _adminTools = adminTools;
        }

        public void Open()
        {
            var players = GameNetwork.NetworkPeers;

            var inquiry = new List<InquiryElement> {new InquiryElement(players, "Everyone", new ImageIdentifier())};
            inquiry.AddRange(players.Select(player => new InquiryElement(player, player.UserName.ToString(), new ImageIdentifier(player.VirtualPlayer.Id))));
            _adminTools.InquiryData(inquiry,OnSelectPlayers,OnSelectPlayersAborted);
            
        }
      
        private void OnSelectPlayersAborted(List<InquiryElement> obj)
        {
            InformationManager.HideInquiry();
        }

        private void OnSelectPlayers(List<InquiryElement> inquiryAnswers)
        {
            InformationManager.HideInquiry();
            var players = inquiryAnswers.Where(i => i.IsEnabled).SelectMany(x =>
            {
                switch (x.Identifier)
                {
                    case NetworkCommunicator peer:
                        return new List<NetworkCommunicator> {peer};
                    case List<NetworkCommunicator> peers:
                        return peers;
                    default:
                        return new List<NetworkCommunicator>();
                }
            }).ToList();
            var inquiry = new List<InquiryElement>
            {
                new InquiryElement(players, "Kick", new ImageIdentifier()),
                new InquiryElement(players, "Ban", new ImageIdentifier()),
                new InquiryElement(players, "Goto", new ImageIdentifier()),
                new InquiryElement(players, "Heal", new ImageIdentifier()),
                new InquiryElement(players, "To me", new ImageIdentifier())
            };    
            _adminTools.InquiryData(inquiry,OnActionsSelected,OnSelectPlayersAborted);
        }

        private void OnActionsSelected(List<InquiryElement> obj)
        {
            InformationManager.HideInquiry();
            var actionstoTake = obj.Where(j=>j.IsEnabled);
            foreach (var action in actionstoTake)
            {
                switch (action.Title)
                {
                    case "Kick":
                        foreach (var player in (List<NetworkCommunicator>)action.Identifier)
                        {
                            DisconnectInfo disconnectInfo = player.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
                            disconnectInfo.Type = DisconnectType.KickedByHost;
                            player.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
                            GameNetwork.AddNetworkPeerToDisconnectAsServer(player);
                        }
                        break;
                    case "Ban":
                        foreach (var player in (List<NetworkCommunicator>)action.Identifier)
                        {
                            //Ban For 1H
                            BannedPlayerManagerCustomGameClient.AddBannedPlayer(player.VirtualPlayer.Id,Environment.TickCount+1000*60*60);
                            DisconnectInfo disconnectInfo = player.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
                            disconnectInfo.Type = DisconnectType.BannedByPoll;
                            player.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
                            GameNetwork.AddNetworkPeerToDisconnectAsServer(player);
                            
                            
                        }
                        break;
                        
                    case "Goto":
                        var firstPlayer = ((List<NetworkCommunicator>) action.Identifier).First();
                        if (firstPlayer.ControlledAgent != null)
                        {
                            Mission.Current.MainAgent.TeleportToPosition(firstPlayer.ControlledAgent.Position);
                        }

                        break;
                    case "Heal":
                        foreach (var player in ((List<NetworkCommunicator>)action.Identifier).Where(player => player.ControlledAgent != null))
                        {
                            player.ControlledAgent.Health = player.ControlledAgent.HealthLimit;
                        }
                        break;
                    case "To me":
                        foreach (var player in (List<NetworkCommunicator>)action.Identifier)
                        {
                            player.ControlledAgent.TeleportToPosition(Mission.Current.MainAgent.Position);
                            
                            
                        }
                        break;
                }
            }
        }
    }
}