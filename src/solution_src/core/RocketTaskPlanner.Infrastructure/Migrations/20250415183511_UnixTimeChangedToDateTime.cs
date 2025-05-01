using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RocketTaskPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UnixTimeChangedToDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "subject_period",
                table: "theme_chat_subjects");

            migrationBuilder.DropColumn(
                name: "subject_period",
                table: "general_chat_subjects");

            migrationBuilder.AlterColumn<DateTime>(
                name: "subject_notify",
                table: "theme_chat_subjects",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "subject_created",
                table: "theme_chat_subjects",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "subject_notify",
                table: "general_chat_subjects",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "subject_created",
                table: "general_chat_subjects",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "subject_notify",
                table: "theme_chat_subjects",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "subject_created",
                table: "theme_chat_subjects",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<long>(
                name: "subject_period",
                table: "theme_chat_subjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "subject_notify",
                table: "general_chat_subjects",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "subject_created",
                table: "general_chat_subjects",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<long>(
                name: "subject_period",
                table: "general_chat_subjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
