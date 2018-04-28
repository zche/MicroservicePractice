using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Project.Api.Application.Queries
{
    public class ProjectQueriesService : IProjectQueries
    {
        private readonly string _connStr;
        public ProjectQueriesService(string connStr)
        {
            _connStr = connStr;
        }
        public async Task<dynamic> GetProjectDetail(int projectId)
        {
            string sql = @" SELECT 
            proj.Company,proj.City,proj.AreaName,proj.Province,proj.FinancingStage,proj.FinancingMoney,
            proj.Valuation,proj.StockPercentage,proj.Introduction,proj.UserId,proj.Income,proj.Revenue,
            proj.UserName,proj.Avatar,proj.BrokerageMode,rule.Tags,rule.Visible 
            FROM projects proj INNER JOIN projectvisiblerules rule ON proj.Id=rule.ProjectId 
            WHERE proj.Id=@projectId";
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                var result = await conn.QueryAsync<dynamic>(sql, new { projectId });
                return result;
            }
        }

        public async Task<dynamic> GetProjectsByUserIdAsync(int userId)
        {
            string sql = @" SELECT Projects.Id,Projects.Avatar,Projects.Company,Projects.FinancingStage,Projects.Introduction,
 Projects.IsShowedSecurityInfo,Projects.CreatedTime FROM Projects WHERE Projects.UserId=@userId";
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                var result = await conn.QueryAsync<dynamic>(sql, new { userId });
                return result;
            }
        }
    }
}
