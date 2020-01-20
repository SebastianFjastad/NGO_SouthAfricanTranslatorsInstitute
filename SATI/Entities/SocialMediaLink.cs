using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SATI.Entities
{
    public class SocialMediaLink
    {
        public int SocialMediaLinkId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}