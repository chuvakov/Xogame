using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOgame.Core.Models
{
    public class SettingsSound : Entity
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public bool IsEnabledWin { get; set; }
        public bool IsEnabledLose { get; set; }
        public bool IsEnabledStep { get; set; }
        public bool IsEnabledDraw { get; set; }
        public bool IsEnabledStartGame { get; set; }
            
    }
}
