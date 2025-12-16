using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Vivid.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase36_ActivityStreamEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "EmailLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "EmailLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_EventId",
                table: "EmailLogs",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_VendorId",
                table: "EmailLogs",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailLogs_Events_EventId",
                table: "EmailLogs",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailLogs_Vendors_VendorId",
                table: "EmailLogs",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailLogs_Events_EventId",
                table: "EmailLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_EmailLogs_Vendors_VendorId",
                table: "EmailLogs");

            migrationBuilder.DropIndex(
                name: "IX_EmailLogs_EventId",
                table: "EmailLogs");

            migrationBuilder.DropIndex(
                name: "IX_EmailLogs_VendorId",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "EmailLogs");
        }
    }
}
