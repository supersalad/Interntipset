using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace inti2008.Web.api.inti
{
    /// <summary>
    /// DTO for client side apis for UserTeams
    /// </summary>
    public class UserTeamDTO
    {
        public string Id { get; set; }
        public string CurrentVersionId { get; set; }
        public string NextVersionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }

        public PlayerDTO[] CurrentTeamVersion { get; set; }
        public PlayerDTO[] NextTeamVersion { get; set; }
    }


    /// <summary>
    /// DTO for client side apis for player
    /// </summary>
    public class PlayerDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Club { get; set; }
        public string Position { get; set; }
        public int PositionSortOrder { get; set; }
        public int Price { get; set; }

    }
}