using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RocketTaskPlanner.Infrastructure.Migrations.ApplicationTimeDb
{
    /// <inheritdoc />
    public partial class TimeZoneDbProviderAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "time_zone_db_providers",
                columns: table => new
                {
                    time_zone_db_provider_id = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_zone_db_providers", x => x.time_zone_db_provider_id);
                });

            migrationBuilder.CreateTable(
                name: "time_zones",
                columns: table => new
                {
                    time_zone_id = table.Column<string>(type: "TEXT", nullable: false),
                    provider_id = table.Column<string>(type: "TEXT", nullable: false),
                    time_zone_name = table.Column<string>(type: "TEXT", nullable: false),
                    time_zone_date_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    time_zone_time_stamp = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_zones", x => x.time_zone_id);
                    table.ForeignKey(
                        name: "FK_time_zones_time_zone_db_providers_provider_id",
                        column: x => x.provider_id,
                        principalTable: "time_zone_db_providers",
                        principalColumn: "time_zone_db_provider_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_time_zones_provider_id",
                table: "time_zones",
                column: "provider_id");

            migrationBuilder.CreateIndex(
                name: "IX_time_zones_time_zone_name",
                table: "time_zones",
                column: "time_zone_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "time_zones");

            migrationBuilder.DropTable(
                name: "time_zone_db_providers");
        }
    }
}
