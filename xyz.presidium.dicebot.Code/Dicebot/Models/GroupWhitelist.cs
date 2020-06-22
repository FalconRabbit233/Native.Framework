using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Models
{
    [Table("group_whitelist")]
    class GroupWhitelist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("enabled_group")]
        public long enabledGroup { get; set; }
    }
}
