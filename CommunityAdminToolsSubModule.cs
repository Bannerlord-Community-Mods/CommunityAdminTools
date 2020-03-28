using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screen;

namespace CommunityAdminTools
{
    public class CommunityAdminToolsSubModule : MBSubModuleBase
    {
        public override void OnMissionBehaviourInitialize(Mission mission)
        {
           
            mission.AddMissionBehaviour(new AdminTools());
            base.OnMissionBehaviourInitialize(mission);
        }
    }
}