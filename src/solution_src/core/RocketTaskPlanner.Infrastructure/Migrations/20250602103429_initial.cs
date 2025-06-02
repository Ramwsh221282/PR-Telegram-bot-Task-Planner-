using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RocketTaskPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification_receivers",
                columns: table => new
                {
                    receiver_id = table.Column<long>(type: "bigint", nullable: false),
                    receiver_name = table.Column<string>(type: "text", nullable: false),
                    receiver_zone_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_receivers", x => x.receiver_id);
                });

            migrationBuilder.CreateTable(
                name: "owners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_owners", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "time_zone_db_providers",
                columns: table => new
                {
                    time_zone_db_provider_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_zone_db_providers", x => x.time_zone_db_provider_id);
                });

            migrationBuilder.CreateTable(
                name: "general_chat_subjects",
                columns: table => new
                {
                    general_chat_subject_id = table.Column<long>(type: "bigint", nullable: false),
                    general_chat_id = table.Column<long>(type: "bigint", nullable: false),
                    subject_periodic = table.Column<bool>(type: "boolean", nullable: false),
                    subject_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subject_notify = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subject_message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_general_chat_subjects", x => x.general_chat_subject_id);
                    table.ForeignKey(
                        name: "FK_general_chat_subjects_notification_receivers_general_chat_id",
                        column: x => x.general_chat_id,
                        principalTable: "notification_receivers",
                        principalColumn: "receiver_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "receiver_themes",
                columns: table => new
                {
                    theme_id = table.Column<long>(type: "bigint", nullable: false),
                    receiver_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receiver_themes", x => x.theme_id);
                    table.ForeignKey(
                        name: "FK_receiver_themes_notification_receivers_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "notification_receivers",
                        principalColumn: "receiver_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "external_chats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    owner_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_chats", x => x.id);
                    table.ForeignKey(
                        name: "FK_external_chats_owners_owner_id",
                        column: x => x.owner_id,
                        principalTable: "owners",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "time_zones",
                columns: table => new
                {
                    time_zone_id = table.Column<string>(type: "text", nullable: false),
                    provider_id = table.Column<string>(type: "text", nullable: false),
                    time_zone_name = table.Column<string>(type: "text", nullable: false),
                    time_zone_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time_zone_time_stamp = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "theme_chat_subjects",
                columns: table => new
                {
                    theme_chat_subject_id = table.Column<long>(type: "bigint", nullable: false),
                    theme_id = table.Column<long>(type: "bigint", nullable: false),
                    subject_periodic = table.Column<bool>(type: "boolean", nullable: false),
                    subject_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subject_notify = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    subject_message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_theme_chat_subjects", x => x.theme_chat_subject_id);
                    table.ForeignKey(
                        name: "FK_theme_chat_subjects_receiver_themes_theme_id",
                        column: x => x.theme_id,
                        principalTable: "receiver_themes",
                        principalColumn: "theme_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_external_chats_owner_id",
                table: "external_chats",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_general_chat_subjects_general_chat_id",
                table: "general_chat_subjects",
                column: "general_chat_id",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_general_chat_subjects_subject_message",
                table: "general_chat_subjects",
                column: "subject_message",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_notification_receivers_receiver_name",
                table: "notification_receivers",
                column: "receiver_name",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_receiver_themes_receiver_id",
                table: "receiver_themes",
                column: "receiver_id",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_theme_chat_subjects_subject_message",
                table: "theme_chat_subjects",
                column: "subject_message",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_theme_chat_subjects_theme_id",
                table: "theme_chat_subjects",
                column: "theme_id",
                descending: new bool[0]);

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
                name: "external_chats");

            migrationBuilder.DropTable(
                name: "general_chat_subjects");

            migrationBuilder.DropTable(
                name: "theme_chat_subjects");

            migrationBuilder.DropTable(
                name: "time_zones");

            migrationBuilder.DropTable(
                name: "owners");

            migrationBuilder.DropTable(
                name: "receiver_themes");

            migrationBuilder.DropTable(
                name: "time_zone_db_providers");

            migrationBuilder.DropTable(
                name: "notification_receivers");
        }
    }
}
