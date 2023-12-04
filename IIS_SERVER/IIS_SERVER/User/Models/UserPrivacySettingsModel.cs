/**
* @file UserPrivacySettingsModel.cs
* author { Matìj Macek (xmacek27)}
* @date 17.12.2023
* @brief Definition of UserPrivacySettingsModel model
*/

namespace IIS_SERVER.User.Models;

public class UserPrivacySettingsModel
{
    public bool VisibilityRegistered { get; set; }
    
    public bool VisibilityGuest { get; set; }
    
    public bool VisibilityGroup { get; set; }
}