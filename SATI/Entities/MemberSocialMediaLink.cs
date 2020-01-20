using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SATI.Models;

namespace SATI.Entities
{
    public class UserSocialMediaLink
    {
        public int UserSocialMediaLinkId { get; set; }

        public int SocialMediaLinkId { get; set; }
        public SocialMediaLink SocialMediaLink { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public string Link { get; set; }
    }
}