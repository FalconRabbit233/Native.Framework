using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Models
{
    [Table("default_dice")]
    class DefaultDice
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("from_group")]
        public long FromGroup { get; set; }

        [Column("from_qq")]
        public long FromQQ { get; set; }

        [Column("default_dice")]
        public int DefaultDiceValue { get; set; }
    }
}
