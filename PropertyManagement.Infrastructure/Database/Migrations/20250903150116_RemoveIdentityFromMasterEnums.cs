using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropertyManagement.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdentityFromMasterEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys that reference the tables we are about to recreate
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_PropertyStatuses_StatusId",
                schema: "PTY",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Owners_IdentificationTypes_IdentificationTypeId",
                schema: "PTY",
                table: "Owners");

            // Create temp tables without IDENTITY on Id
            migrationBuilder.CreateTable(
                name: "IdentificationTypes_Temp",
                schema: "PTY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentificationTypes_Temp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyStatuses_Temp",
                schema: "PTY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyStatuses_Temp", x => x.Id);
                });

            // Copy data from original tables into temp tables
            migrationBuilder.Sql(@"INSERT INTO PTY.IdentificationTypes_Temp (Id, Name)
                                   SELECT Id, Name FROM PTY.IdentificationTypes;");

            migrationBuilder.Sql(@"INSERT INTO PTY.PropertyStatuses_Temp (Id, Name)
                                   SELECT Id, Name FROM PTY.PropertyStatuses;");

            // Drop original tables with IDENTITY
            migrationBuilder.DropTable(
                name: "IdentificationTypes",
                schema: "PTY");

            migrationBuilder.DropTable(
                name: "PropertyStatuses",
                schema: "PTY");

            // Rename temp tables to original names
            migrationBuilder.RenameTable(
                name: "IdentificationTypes_Temp",
                schema: "PTY",
                newName: "IdentificationTypes",
                newSchema: "PTY");

            migrationBuilder.RenameTable(
                name: "PropertyStatuses_Temp",
                schema: "PTY",
                newName: "PropertyStatuses",
                newSchema: "PTY");

            // Recreate foreign keys to the recreated tables
            migrationBuilder.AddForeignKey(
                name: "FK_Properties_PropertyStatuses_StatusId",
                schema: "PTY",
                table: "Properties",
                column: "StatusId",
                principalSchema: "PTY",
                principalTable: "PropertyStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Owners_IdentificationTypes_IdentificationTypeId",
                schema: "PTY",
                table: "Owners",
                column: "IdentificationTypeId",
                principalSchema: "PTY",
                principalTable: "IdentificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys referencing the current tables
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_PropertyStatuses_StatusId",
                schema: "PTY",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Owners_IdentificationTypes_IdentificationTypeId",
                schema: "PTY",
                table: "Owners");

            // Create temp tables WITH IDENTITY on Id
            migrationBuilder.CreateTable(
                name: "IdentificationTypes_Temp",
                schema: "PTY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentificationTypes_Temp", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyStatuses_Temp",
                schema: "PTY",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyStatuses_Temp", x => x.Id);
                });

            // Copy data back
            // Enabling IDENTITY_INSERT is required to insert explicit values into identity columns
            migrationBuilder.Sql(@"SET IDENTITY_INSERT PTY.IdentificationTypes_Temp ON;
                                   INSERT INTO PTY.IdentificationTypes_Temp (Id, Name)
                                   SELECT Id, Name FROM PTY.IdentificationTypes;
                                   SET IDENTITY_INSERT PTY.IdentificationTypes_Temp OFF;");

            migrationBuilder.Sql(@"SET IDENTITY_INSERT PTY.PropertyStatuses_Temp ON;
                                   INSERT INTO PTY.PropertyStatuses_Temp (Id, Name)
                                   SELECT Id, Name FROM PTY.PropertyStatuses;
                                   SET IDENTITY_INSERT PTY.PropertyStatuses_Temp OFF;");

            // Drop current tables
            migrationBuilder.DropTable(
                name: "IdentificationTypes",
                schema: "PTY");

            migrationBuilder.DropTable(
                name: "PropertyStatuses",
                schema: "PTY");

            // Rename temp tables back to original names
            migrationBuilder.RenameTable(
                name: "IdentificationTypes_Temp",
                schema: "PTY",
                newName: "IdentificationTypes",
                newSchema: "PTY");

            migrationBuilder.RenameTable(
                name: "PropertyStatuses_Temp",
                schema: "PTY",
                newName: "PropertyStatuses",
                newSchema: "PTY");

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(
                name: "FK_Properties_PropertyStatuses_StatusId",
                schema: "PTY",
                table: "Properties",
                column: "StatusId",
                principalSchema: "PTY",
                principalTable: "PropertyStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Owners_IdentificationTypes_IdentificationTypeId",
                schema: "PTY",
                table: "Owners",
                column: "IdentificationTypeId",
                principalSchema: "PTY",
                principalTable: "IdentificationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
