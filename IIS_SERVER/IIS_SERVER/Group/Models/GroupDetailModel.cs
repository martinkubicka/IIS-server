using IIS_SERVER.Enums;
using System.ComponentModel.DataAnnotations;
using IIS_SERVER.Utils;

namespace IIS_SERVER.Group.Models;

public class GroupDetailModel : GroupListModel
{
    
    public bool VisibilityGuest {get; set;}

    public bool VisibilityMember {get;set;}

    public GroupDetailModel(){
        this.VisibilityGuest = true;
        this.VisibilityMember = true;
    }
}