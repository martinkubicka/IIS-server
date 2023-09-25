namespace IIS_SERVER.Group.Models;

public class GroupPrivacySettingsModel
{
    public bool VisibilityMember { get; set; }

    public bool VisibilityGuest { get; set; }

    GroupPrivacySettingsModel()
    {
        VisibilityMember = true;
        VisibilityGuest = true;
    }
}
