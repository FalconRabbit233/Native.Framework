using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Models
{
    [Table("arena_player")]
    class ArenaPlayer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("from_group")]
        public long FromGroup { get; set; }

        [Column("from_qq")]
        public long FromQQ { get; set; }
    }
}
