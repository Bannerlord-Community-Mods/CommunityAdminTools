using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;

namespace CommunityAdminTools
{
    public class AdminTools : AdminToolsBase
    {
        private Scene SwitchtoScene;
        private Camera _camera;
        private Scene oldScene;
        private UserManagement _userManagement;

        private ServerManagement _serverManagement;
        public override void OnBehaviourInitialize()
        {

            base.OnBehaviourInitialize();
        }
        
        public override void AfterStart()
        
        {
            
            this._serverManagement = new ServerManagement(this);
            this._userManagement = new UserManagement(this);
            base.AfterStart();
        }

        private void SwitchScene()
        {
            if (this.MissionScreen == null) return;
            if (SwitchtoScene == null)
            {
                this.SwitchtoScene = Scene.CreateNewScene(true);
                this.SwitchtoScene.SetName("MBInitialScreenBase");
                this.SwitchtoScene.SetPlaySoundEventsAfterReadyToRender(true);
                this.SwitchtoScene.Read("main_menu_a");
                for (int i = 0; i < 40; i++)
                {
                    this.SwitchtoScene.Tick(0.1f);
                }

                Vec3 vec = default(Vec3);
                this.SwitchtoScene.FindEntityWithTag("camera_instance")
                    .GetCameraParamsFromCameraScript(this._camera = Camera.CreateCamera(), ref vec);
            }

            if (oldScene == null)
            {
                
                oldScene = this.MissionScreen.SceneLayer.SceneView.GetScene();
            }
            this.MissionScreen.SceneLayer.SetScene(this.MissionScreen.SceneLayer.SceneView.GetScene() == SwitchtoScene? oldScene: SwitchtoScene);
        }

        public override void OnPreMissionTick(float dt)
        {
            if (base.Input.IsKeyReleased(InputKey.F1) && base.Input.IsAltDown())
            {
                DisplayAdminInterface();
            }

            base.OnPreMissionTick(dt);
        }

        private void DisplayAdminInterface()
        {
            
                var inquiryElements = new List<InquiryElement>
                {
                    new InquiryElement(ToolSelection.Users, "Manage Users",
                        new ImageIdentifier(GameNetwork.MyPeer.VirtualPlayer.Id)),
                    /*new InquiryElement(ToolSelection.Server, "Manage Server",
                        new ImageIdentifier(GameNetwork.MyPeer.VirtualPlayer.Id)),*/
                    new InquiryElement(ToolSelection.SwitchToScence, "Switch Scene(Test Thing)",
                        new ImageIdentifier(GameNetwork.MyPeer.VirtualPlayer.Id))
                };
                InquiryData(inquiryElements, OnAdminInterfaceSelected, OnAdminInterfaceAborted);
            
        }

        private void OnAdminInterfaceAborted(List<InquiryElement> obj)
        {
            InformationManager.HideInquiry();
            Console.WriteLine(obj.ToString());
        }

        private void OnAdminInterfaceSelected(List<InquiryElement> obj)
        {
            InformationManager.HideInquiry();
            this.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(
                HotKeyManager.GetCategory("MissionOrderHotkeyCategory"));
            foreach (var inquiryElement in obj.Where(x => x.IsEnabled))
            {
                switch ((ToolSelection) inquiryElement.Identifier)
                {
                    case ToolSelection.Users:
                        _userManagement.Open();
                        break;
                    case ToolSelection.Server:

                        _serverManagement.Open();
                        break;
                    case ToolSelection.SwitchToScence:
                        SwitchScene();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Console.WriteLine(obj.ToString());
        }

      
    }
}