using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManagement.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Unique_CodeInternal_On_Properties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up existing duplicates: keep the most recently created row per CodeInternal
            migrationBuilder.Sql(@"
WITH dups AS (
    SELECT Id,
           ROW_NUMBER() OVER (PARTITION BY CodeInternal ORDER BY CreatedAtUtc DESC, Id DESC) AS rn
    FROM PTY.Properties
)
DELETE FROM PTY.Properties
WHERE Id IN (SELECT Id FROM dups WHERE rn > 1);
");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_CodeInternal",
                schema: "PTY",
                table: "Properties",
                column: "CodeInternal",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_CodeInternal",
                schema: "PTY",
                table: "Properties");
        }
    }
}
