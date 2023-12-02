/**
* @file GroupMemberCompositeModel.cs
* @author { Martin Kubicka (xkubic45) }
* @date 17.12.2023
* @brief Defintion of group member composite model
*/

using IIS_SERVER.Member.Models;

namespace IIS_SERVER.Group.Models
{
    public class GroupMemberCompositeModel
    {
        public GroupEmailModel Group { get; set; }
        public MemberModel Member { get; set; }
    }
}
