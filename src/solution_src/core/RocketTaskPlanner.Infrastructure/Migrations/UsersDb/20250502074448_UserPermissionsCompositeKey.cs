using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RocketTaskPlanner.Infrastructure.Migrations.UsersDb
{
    /// <inheritdoc />
    public partial class UserPermissionsCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_user_permissions",
                table: "user_permissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_permissions",
                table: "user_permissions",
                columns: new[] { "id", "user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_user_permissions",
                table: "user_permissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_permissions",
                table: "user_permissions",
                column: "id");
        }
    }
}
