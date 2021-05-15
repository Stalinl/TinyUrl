namespace TinyUrl.Core
{
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Dapper;
    using EnsureThat;

    [ExcludeFromCodeCoverage]
    public sealed class SqlRepository : IRepository
    {
        private readonly string connectionString;

        public SqlRepository(string connectionString)
        {
            this.connectionString = EnsureArg.IsNotNullOrEmpty(connectionString, nameof(connectionString));
        }

        /// <inheritdoc/>
        public async Task<string> GetByIdAsync(int id)
        {
            var query = "SELECT OriginalUrl FROM ShortUrl WHERE Id = @id";
            var param = new { id = id };

            using (var connection = new SqlConnection(this.connectionString))
            {
                return await connection
                    .QueryFirstOrDefaultAsync<string>(query, param, commandType: CommandType.Text)
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetByPathAsync(string originalUrl)
        {
            var query = "SELECT Id FROM ShortUrl WHERE OriginalUrl = @OriginalUrl";
            var param = new { originalUrl = originalUrl };

            using (var connection = new SqlConnection(this.connectionString))
            {
                return await connection
                    .QueryFirstOrDefaultAsync<int>(query, param, commandType: CommandType.Text)
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task<int> CreateAsync(string originalUrl)
        {
            const string query = @"DECLARE @InsertedRows AS TABLE (Id int);
                INSERT INTO ShortUrl(OriginalUrl) OUTPUT Inserted.Id INTO @InsertedRows
                VALUES (@originalUrl);
                SELECT Id FROM @InsertedRows";

            using (var connection = new SqlConnection(this.connectionString))
            {
                return await connection
                    .QuerySingleAsync<int>(query, new { originalUrl = originalUrl }, commandType: CommandType.Text)
                    .ConfigureAwait(false);
            }
        }
    }
}