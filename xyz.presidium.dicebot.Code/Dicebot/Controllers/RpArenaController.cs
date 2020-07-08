using Native.Sdk.Cqp.Model;
using CqpModel = Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.presidium.dicebot.Code.Dicebot.Sqlite;
using xyz.presidium.dicebot.Code.Dicebot.Models;
using Unity.Interception.Utilities;

namespace xyz.presidium.dicebot.Code.Dicebot.Controllers
{
    class RpArenaController : IResponsable
    {
        private SQLiteConnection context;

        private Rng rng;

        public RpArenaController(SQLiteConnection context, Rng rng)
        {
            this.context = context;
            this.rng = rng;
        }

        public string Response(QQ fromQQ, QQMessage message, CqpModel.Group fromGroup, Discuss fromDiscuss)
        {
            if (fromGroup == null) return null;

            var currentCSTDate = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time")
                ).Date;

            if (context.Table<ArenaResult>().Any(r => r.CreatedAt == currentCSTDate && r.FromGroup == fromGroup.Id))
            {
                var shortResult = context.Table<ArenaResult>().First(r => r.CreatedAt == currentCSTDate && r.FromGroup == fromGroup.Id);
                var longResults = context.Table<ArenaDetail>()
                    .Where(d => d.ArenaResultId == shortResult.Id)
                    .OrderBy(d => d.FightOrder);

                foreach (var item in longResults)
                {
                    fromQQ.SendPrivateMessage(item.Content);
                }
                return $" * {currentCSTDate:yyyy年MM月dd日} rp竞技场结果:\n{shortResult.Content}\n详情已尝试私信发送";
            }

            // 选出选手
            var groupPlayers = context.Table<ArenaPlayer>()
                .Where(p => p.FromGroup == fromGroup.Id)
                .ToArray();

            if (groupPlayers.Count() < 3) return " * 三人以下请使用rppk";

            var players = new List<RPArenaPlayer>();
            foreach (var player in groupPlayers)
            {
                var playerQQ = new QQ(fromQQ.CQApi, player.FromQQ);
                Utils.GetNickname(context, playerQQ, fromGroup, fromDiscuss, out var nickname);
                players.Add(new RPArenaPlayer()
                {
                    QQ = player.FromQQ,
                    Nickname = nickname.NicknameValue,
                    Jrrp = 101 - Utils.GetJrrp(playerQQ, fromGroup, fromDiscuss),
                    Style = (BattleStyle)(rng.Roll(5) - 1)
                });
            }

            // 选手分组
            var playerGroups = new List<List<RPArenaPlayer>>();
            var groupWidth = (int)Math.Round(Math.Sqrt(players.Count));
            var remainCount = players.Count % groupWidth;

            // 钦定特殊组员
            if (remainCount > 0)
            {
                var reducedGroup = new List<RPArenaPlayer>();
                players.Sort((a, b) => -a.Jrrp.CompareTo(b.Jrrp));

                if (remainCount % 2 == 1)
                {
                    reducedGroup.Add(players[0]);
                    players.RemoveAt(0);
                    remainCount -= 1;
                }
                else
                {
                    reducedGroup.Add(players[0]);
                    players.RemoveAt(0);
                    reducedGroup.Add(players[players.Count - 1]);
                    players.RemoveAt(players.Count - 1);
                    remainCount -= 2;
                }

                for (int i = 0; i < groupWidth - reducedGroup.Count; i++)
                {
                    var picked = players[rng.Roll(players.Count) - 1];
                    var image = new RPArenaPlayer()
                    {
                        Jrrp = (int)Math.Round((double)picked.Jrrp / 4 * 3),
                        QQ = fromQQ.CQApi.GetLoginQQ().Id,
                        Nickname = $"|{picked.Nickname}|",
                        Style = (BattleStyle)(rng.Roll(5) - 1),
                    };
                    reducedGroup.Add(image);
                }

                playerGroups.Add(reducedGroup);
            }

            // 正常组员分组
            while (players.Count > 0)
            {
                var normalGroup = new List<RPArenaPlayer>();
                for (int j = 0; j < groupWidth; j++)
                {
                    var pickedPlayer = players[rng.Roll(players.Count) - 1];
                    normalGroup.Add(pickedPlayer);
                    players.Remove(pickedPlayer);
                }
                playerGroups.Add(normalGroup);
            }

            // 组间对战
            var idleGroups = new List<List<RPArenaPlayer>>();
            var detailedText = new List<string>();
            var shortText = new List<string>();
            var fightNumber = 1;
            while (playerGroups.Count > 1)
            {
                var milk = (4 - rng.Roll(7)) * 0.01;  // 奶

                var fightingGroups = new List<RPArenaPlayer>[2];

                for (int i = 0; i < 2; i++)
                {
                    var fightingGroup = playerGroups[rng.Roll(playerGroups.Count) - 1];
                    playerGroups.Remove(fightingGroup);
                    fightingGroups[i] = fightingGroup;
                }

                var combatPowerSums = new int[] { 0, 0 };

                var groupPlayerStrings = new string[2][];
                for (int i = 0; i < 2; i++)
                {
                    var currentGroup = fightingGroups[i];
                    var targetGroup = fightingGroups[1 - i];

                    string[] playerSlots = new string[currentGroup.Count];
                    for (int j = 0; j < currentGroup.Count; j++)
                    {
                        var player = currentGroup[j];
                        var combatLuck = (int)Math.Ceiling(rng.Roll(player.Jrrp) * 0.9);
                        var combatBase = (int)Math.Ceiling(player.Jrrp * 0.07);
                        var supportCount = player.GetSupportCount(currentGroup);
                        var advantageCount = player.GetAdvantageCount(targetGroup);
                        var relationCount = supportCount + advantageCount;
                        var combatPower = (int)Math.Ceiling((combatLuck + combatBase) * (1 + relationCount * (0.1 + milk)));
                        combatPowerSums[i] += combatPower;
                        playerSlots[j] = $"{player.Nickname}@{player.Jrrp}({player.Style}): ({combatLuck}+{combatBase}) * ({supportCount}生{advantageCount}克) = {combatPower}";
                    }
                    groupPlayerStrings[i] = playerSlots;
                }

                var winnersIndex = (combatPowerSums[0] > combatPowerSums[1]) ? 0 : 1;
                var losersIndex = 1 - winnersIndex;
                var resultText = $"第{fightNumber}战:" +
                    fightingGroups[losersIndex].JoinStrings(", ", p => p.Nickname) + " 战败。" +
                    fightingGroups[winnersIndex].JoinStrings(", ", p => p.Nickname);

                shortText.Add(resultText);

                var detail = $" * 第{fightNumber}战: 奶@{(int)(milk * 100)}%\n" +
                    groupPlayerStrings[0].JoinStrings("\n") + "\n- 对战 -\n" +
                    groupPlayerStrings[1].JoinStrings("\n") +
                    $"\n\n综合战力: {combatPowerSums[0]} 对 {combatPowerSums[1]}";

                detailedText.Add(detail);

                idleGroups.Add(fightingGroups[winnersIndex]);

                if (playerGroups.Count < 2 && idleGroups.Count > 0)
                {
                    playerGroups.AddRange(idleGroups);
                    idleGroups.Clear();
                }
                fightNumber += 1;
            }

            // 保存并通知结果
            var combinedShortText = shortText.JoinStrings(" 晋级\n") + " 最终获胜";
            var arenaResult = new ArenaResult()
            {
                FromGroup = fromGroup.Id,
                CreatedAt = currentCSTDate,
                Content = combinedShortText
            };

            context.Insert(arenaResult);

            for (int i = 0; i < detailedText.Count; i++)
            {
                var item = detailedText[i];
                context.Insert(new ArenaDetail() { ArenaResultId = arenaResult.Id, Content = item, FightOrder = i + 1 });

                fromGroup.SendGroupMessage(item);
            }

            return combinedShortText;
        }

        public enum BattleStyle
        {
            偷, 稳, 谐, 莽, 狗
        }

        public class RPArenaPlayer
        {
            public long QQ { get; set; }

            public string Nickname { get; set; }

            public BattleStyle Style { get; set; }

            public int Jrrp { get; set; }

            private static BattleStyle[][] advantages = new BattleStyle[][]
            {
                new BattleStyle[] { BattleStyle.偷, BattleStyle.稳 },
                new BattleStyle[] { BattleStyle.稳, BattleStyle.谐 },
                new BattleStyle[] { BattleStyle.谐, BattleStyle.莽 },
                new BattleStyle[] { BattleStyle.莽, BattleStyle.狗 },
                new BattleStyle[] { BattleStyle.狗, BattleStyle.偷 },
            };

            private static BattleStyle[][] supports = new BattleStyle[][]
            {
                new BattleStyle[] { BattleStyle.偷, BattleStyle.谐 },
                new BattleStyle[] { BattleStyle.谐, BattleStyle.狗 },
                new BattleStyle[] { BattleStyle.狗, BattleStyle.稳 },
                new BattleStyle[] { BattleStyle.稳, BattleStyle.莽 },
                new BattleStyle[] { BattleStyle.莽, BattleStyle.偷 },
            };

            public int GetAdvantageCount(IEnumerable<RPArenaPlayer> targets)
            {
                var advantagedTo = advantages.First(relation => relation[0] == this.Style)[1];
                return targets.Count(p => p.Style == advantagedTo);
            }

            public int GetSupportCount(IEnumerable<RPArenaPlayer> teammates)
            {
                var supportedBy = supports.First(relation => relation[1] == this.Style)[0];
                return teammates.Where(p => p.QQ != this.QQ).Count(p => p.Style == supportedBy);
            }

            public override string ToString()
            {
                return $"{Nickname}({Style}:{Jrrp})";
            }
        }
    }
}
