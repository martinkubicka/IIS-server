namespace IIS_SERVER.Group.Models;

public class GroupPrivacySettingsModel
{
    public bool VisibilityMember { get; set; }

    public bool VisibilityGuest { get; set; }

    public GroupPrivacySettingsModel()
    {
        VisibilityMember = true;
        VisibilityGuest = true;
    }
}
