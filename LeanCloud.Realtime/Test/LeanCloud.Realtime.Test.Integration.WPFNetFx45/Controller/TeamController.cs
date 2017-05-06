using LeanCloud.Realtime.Test.Integration.WPFNetFx45.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeanCloud.Storage.Internal;

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45.Controller
{
    public class TeamController
    {
        public AVIMClient client { get; set; }
        public int limit = 100;
        public async Task<IEnumerable<AVIMConversation>> LoadAllConversationsInTeamAsync(string teamId = null, Team team = null)
        {
            var wrappedTeam = wrapTeam(teamId, team);
            var query = new AVQuery<AVObject>("_Conversation")
               .WhereEqualTo("team", wrappedTeam).Limit(300);

            var result = await query.FindAsync();

            return result.Select(x =>
            {
                return AVIMConversation.CreateWithData(x, client);
            });
        }

        public async Task<IEnumerable<AVUser>> LoadAllUsersInTeam(string teamId = null, Team team = null)
        {
            var wrappedTeam = wrapTeam(teamId, team);
            var query = new AVQuery<AVObject>("Team_User")
              .WhereEqualTo("team", wrappedTeam)
              .Limit(limit)
              .Include("user");

            var tu = await query.FindAsync();

            return tu.Select(x => x.Get<AVUser>("user"));
        }

        public async Task<IEnumerable<Team>> LoadAllTeamsByUser(string userId = null, AVUser user = null)
        {
            var wrappedTeam = warpUser(userId, user);
            var query = new AVQuery<AVObject>("Team_User")
              .WhereEqualTo("user", wrappedTeam)
              .Limit(limit)
              .Include("team");

            var tu = await query.FindAsync();

            return tu.Select(x =>
            {
                var team = x.Get<Team>("team");
                return team;
            });
        }

        private Team wrapTeam(string teamId = null, Team team = null)
        {
            if (team == null)
            {
                if (string.IsNullOrEmpty(teamId))
                {
                    throw new ArgumentNullException("team id and team can NOT both be null at the same time");
                }
                team = new Team()
                {
                    ObjectId = teamId
                };
            }
            return team;
        }
        private AVUser warpUser(string userId = null, AVUser user = null)
        {
            if (user == null)
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentNullException("team id and team can NOT both be null at the same time");
                }
                user = AVUser.CreateWithoutData<AVUser>(userId);
            }
            return user;
        }
    }
}
