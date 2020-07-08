using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Models
{
    [Table("arena_result")]
    class ArenaResult
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("from_group")]
        public long FromGroup { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("content")]
        public string Content { get; set; }
    }
}
