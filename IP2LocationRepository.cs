using Dapper;
using IP2Location.API.Data;
using IP2Location.API.Models.Domain;
using IP2Location.API.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Emit;

namespace IP2Location.API.Repositories.Implementation
{
	public class IP2LocationRepository : IIP2Location
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly IDbConnection _dbConnection;

		public IP2LocationRepository(ApplicationDbContext dbContext, IConfiguration configuration)
		{
			_dbContext = dbContext;
			_dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
		}
		public async Task<List<IpData>> GetAllAsync()
		{
			return await _dbContext.IpData.ToListAsync();
		}
		public async Task<IEnumerable<IpData>> GetIpByZipCode(string zipCode)
		{
			//return await _dbContext.IpData.ToListAsync();
			var parameters = new DynamicParameters();
			parameters.Add("Zip", zipCode, DbType.String, ParameterDirection.Input);
			return await _dbConnection.QueryAsync<IpData>("GetIpByZipCode", parameters, commandType: CommandType.StoredProcedure);
		}
		public async Task<IpData?> GetByZipCodeAsync(string zipCode)
		{
			return await _dbContext.IpData.FirstOrDefaultAsync(x => x.Zip == zipCode);
		}
		public async Task<IpData?> GetByIpAsync(string ip)
		{
			return await _dbContext.IpData.FirstOrDefaultAsync(x => x.IP == ip);
		}
		public async Task CreateZipData(ZipData zipData)
		{
			var query = "INSERT INTO ZipData (Zip) VALUES (@Zip)";
			var parameters = new DynamicParameters();
			parameters.Add("Zip", zipData.Zip, DbType.String);
			await _dbConnection.ExecuteAsync(query, parameters);
		}
		//public async Task<IEnumerable<ZipData>> GetZipByZipCodeAsync(string zipCode)
		//{
		//	var parameters = new DynamicParameters();
		//	parameters.Add("Zip", zipCode, DbType.String, ParameterDirection.Input);
		//	return await _dbConnection.QueryAsync<ZipData>("GetZipByZipCode", parameters, commandType: CommandType.StoredProcedure);
		//}
		public async Task<ZipData?> GetZipByZipCodeAsync(string zipCode)
		{
			return await _dbContext.ZipData.FirstOrDefaultAsync(x => x.Zip == zipCode);
		}
	}
}
