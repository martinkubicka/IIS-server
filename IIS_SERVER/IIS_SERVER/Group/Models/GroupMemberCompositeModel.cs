using IIS_SERVER.Member.Models;

namespace IIS_SERVER.Group.Models
{
    public class GroupMemberCompositeModel
    {
        public GroupEmailModel Group { get; set; }
        public MemberModel Member { get; set; }
    }
}
