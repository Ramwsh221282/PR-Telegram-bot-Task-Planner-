using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RocketTaskPlanner.Infrastructure.Migrations.PermissionsDb
{
    /// <inheritdoc />
    public partial class PermissionsColumnsLowerCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "permissions",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "permissions",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "permissions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "permissions",
                newName: "Id");
        }
    }
}
