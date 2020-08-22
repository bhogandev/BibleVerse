using System;
using System.Collections.Generic;

namespace BibleVerse.DTO
{
    public class OrgProfile
    {
        public string Name { get; set; }

        public int Followers { get; set; }

        public int Following { get; set; }

        public int Members { get; set; }

        public List<Posts> Posts { get; set; }

    }
}
