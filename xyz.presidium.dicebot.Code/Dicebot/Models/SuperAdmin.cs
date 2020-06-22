using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;

namespace xyz.presidium.dicebot.Code.Dicebot.Models
{
    [Table("super_admin")]
    class SuperAdmin
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("qq")]
        public long QQNumber { get; set; }
    }
}
