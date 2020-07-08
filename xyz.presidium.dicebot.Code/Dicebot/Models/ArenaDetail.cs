using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Models
{
    [Table("arena_detail")]
    class ArenaDetail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("arena_result_id")]
        public long ArenaResultId { get; set; }

        [Column("fight_order")]
        public int FightOrder { get; set; }

        [Column("content")]
        public string Content { get; set; }
    }
}
