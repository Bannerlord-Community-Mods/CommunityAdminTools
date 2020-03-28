using System.Collections.Generic;
using TaleWorlds.Core;

namespace CommunityAdminTools
{
    internal class ServerManagement
    {
        private readonly AdminTools _adminTools;

        public ServerManagement(AdminTools adminTools)
        {
            _adminTools = adminTools;
        }

        public void Open()
        {
            var inquiry = new List<InquiryElement>();
            _adminTools.InquiryData(inquiry,onSelected,onAborted);
        }

        private void onAborted(List<InquiryElement> obj)
        {
            InformationManager.HideInquiry();
        }

        private void onSelected(List<InquiryElement> obj)
        {
            InformationManager.HideInquiry();
        }
    }
}