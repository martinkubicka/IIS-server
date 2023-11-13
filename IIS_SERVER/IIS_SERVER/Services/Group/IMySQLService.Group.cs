using IIS_SERVER.Group.Models;

namespace IIS_SERVER.Services;

public partial interface IMySQLService
{
    Task<bool> AddGroup(GroupListModel group);

    Task<GroupListModel?> GetGroup(string handle);

    Task<List<GroupListModel?>> GetGroups(int limit = 0);

    //returns only groups that the user either joined or not joined depending on joined param
    Task<List<GroupListModel>> GetGroups(string userEmail, bool joined);

    Task<bool> DeleteGroup(string handle);

    Task<bool> UpdateGroup(GroupListModel listModel);

    Task<bool> UpdateGroupPolicy(GroupPrivacySettingsModel privacySettingsModel, string handle);

    Task<GroupPrivacySettingsModel?> GetGroupPolicy(string handle);

    Task<List<GroupListModel?>> SearchGroups(string searchTerm, int limit);
}
