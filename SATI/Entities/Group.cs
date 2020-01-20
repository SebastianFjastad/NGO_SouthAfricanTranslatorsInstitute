using System;
using System.Collections.Generic;
using SATI.Models;

namespace SATI.Entities
{
    public class Group
    {
        public Group()
        {
            Users = new HashSet<User>();
        }

        public int GroupId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<User> Users { get; set; }
    }
}