﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Empedo.Models.TempusHub
{
    public class ServerInfoShort
    {
        public string Alias { get; set; }
        public string Name { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string CurrentMap { get; set; }
        public string IpAddress { get; set; }
        public int Id { get; set; }
    }
}
