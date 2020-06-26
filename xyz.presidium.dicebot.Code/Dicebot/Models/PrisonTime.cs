using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Models
{
    [Table("prison_time")]
    class PrisonTime
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("from_group")]
        public long FromGroup { get; set; }

        [Column("from_QQ")]
        public long FromQQ { get; set; }

        [Column("prison_time_value")]
        public int PrisonTimeValue { get; set; }
    }
}
