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
                    receiver_id = table.Column<long>(type: "INTEGER", nullable: false),
                    receiver_name = table.Column<string>(type: "TEXT", nullable: false),
                    receiver_zone_time_stamp = table.Column<long>(type: "INTEGER", nullable: false),
                    receiver_zone_name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_receivers", x => x.receiver_id);
                });

            migrationBuilder.CreateTable(
                name: "general_chat_subjects",
                columns: table => new
                {
                    general_chat_subject_id = table.Column<long>(type: "INTEGER", nullable: false),
                    general_chat_id = table.Column<long>(type: "INTEGER", nullable: false),
                    subject_periodic = table.Column<bool>(type: "INTEGER", nullable: false),
                    subject_period = table.Column<long>(type: "INTEGER", nullable: false),
                    subject_created = table.Column<long>(type: "INTEGER", nullable: false),
                    subject_notify = table.Column<long>(type: "INTEGER", nullable: false),
                    subject_message = table.Column<string>(type: "TEXT", nullable: false)
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
                    theme_id = table.Column<long>(type: "INTEGER", nullable: false),
                    receiver_id = table.Column<long>(type: "INTEGER", nullable: false)
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
                name: "theme_chat_subjects",
                columns: table => new
                {
                    theme_chat_subject_id = table.Column<long>(type: "INTEGER", nullable: false),
                    theme_id = table.Column<long>(type: "INTEGER", nullable: false),
                    subject_periodic = table.Column<bool>(type: "INTEGER", nullable: false),
                    subject_period = table.Column<long>(type: "INTEGER", nullable: false),
                    subject_created = table.Column<long>(type: "INTEGER", nullable: false),
                    subject_notify = table.Column<long>(type: "INTEGER", nullable: false),
                    subject_message = table.Column<string>(type: "TEXT", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "general_chat_subjects");

            migrationBuilder.DropTable(
                name: "theme_chat_subjects");

            migrationBuilder.DropTable(
                name: "receiver_themes");

            migrationBuilder.DropTable(
                name: "notification_receivers");
        }
    }
}
