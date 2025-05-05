using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RocketTaskPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class External_Chats_ParentId_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "parent_id",
                table: "external_chats",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "parent_id",
                table: "external_chats");
        }
    }
}
