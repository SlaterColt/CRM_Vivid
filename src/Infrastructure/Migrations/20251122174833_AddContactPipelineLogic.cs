using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Vivid.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactPipelineLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConnectionStatus",
                table: "Contacts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FollowUpCount",
                table: "Contacts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLead",
                table: "Contacts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastContactedAt",
                table: "Contacts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Contacts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "Contacts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_IsLead",
                table: "Contacts",
                column: "IsLead");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Stage",
                table: "Contacts",
                column: "Stage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contacts_IsLead",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_Stage",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ConnectionStatus",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "FollowUpCount",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "IsLead",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "LastContactedAt",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "Contacts");
        }
    }
}
