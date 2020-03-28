using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.View.Missions;

namespace CommunityAdminTools
{
    public class AdminToolsBase : MissionView
    {
        private Queue<(List<InquiryElement> inquiryElements, Action<List<InquiryElement>> onSelected,
            Action<List<InquiryElement>> onAborted)> menuQueue = new Queue<(List<InquiryElement> inquiryElements, Action<List<InquiryElement>> onSelected, Action<List<InquiryElement>> onAborted)>();

        public bool isInquiryDisplayed { get; set; }

        public  void InquiryData(List<InquiryElement> inquiryElements, Action<List<InquiryElement>> onSelected,
            Action<List<InquiryElement>> onAborted)
        {
            //Wrap Select/Abort 
            this.menuQueue.Enqueue((inquiryElements, x=>
            {
                this.isInquiryDisplayed = false;
                onSelected(x);
                
            }, x=>
            {
                
                this.isInquiryDisplayed = false;
                
                onAborted(x);
            }));
            
        }

        protected void InquiryDequeue()
        {
            if (menuQueue.Count <= 0 || isInquiryDisplayed) return;
            var (inquiryElements, onSelected, onAborted) = menuQueue.Dequeue();
            InformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(
                "Admin Panel", "What Actions do you want to take my Lord", inquiryElements, true, false,
                "Open", "no",
                onSelected, onAborted), false);
            isInquiryDisplayed = true;
        }

        public override void OnPreMissionTick(float dt)
        {
            
            InquiryDequeue();
            base.OnPreMissionTick(dt);
        }
    }
}