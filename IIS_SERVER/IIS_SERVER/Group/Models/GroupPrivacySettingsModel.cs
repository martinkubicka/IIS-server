/**
* @file GroupPrivacySettingsModel.cs
* @author { Martin Kubicka (xkubic45)  Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Defintion of group privacy settings model
*/

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
