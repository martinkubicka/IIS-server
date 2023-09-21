namespace IIS_SERVER.User.Models;

public class UserPrivacySettingsModel
{
    public bool VisibilityRegistered { get; set; }
    
    public bool VisibilityGuest { get; set; }
    
    public bool VisibilityGroup { get; set; }
}