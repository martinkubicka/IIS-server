using IIS_SERVER.Group.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<bool> AddGroup(GroupListModel user);

    Task<GroupListModel?> GetGroup(string handle);

    Task<List<GroupListModel>> GetGroups();

    //returns only groups that the user either joined or not joined depending on joined param
    Task<List<GroupListModel>> GetGroups(string userEmail, bool joined);

    Task<bool> DeleteGroup(string handle);

    Task<bool> UpdateGroup(GroupListModel listModel);

    Task<bool> UpdateGroupPolicy(GroupPrivacySettingsModel privacySettingsModel);
}
