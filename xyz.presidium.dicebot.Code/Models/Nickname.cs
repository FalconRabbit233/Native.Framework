using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyz.presidium.dicebot.Code.Models
{
    [Table("nickname")]
    class Nickname
    {
        public int Id { get; set; }

        public long FromGroup { get; set; }

        public long FromQQ { get; set; }

        [Column("nickname")]
        public string NicknameValue { get; set; }
    }
}
