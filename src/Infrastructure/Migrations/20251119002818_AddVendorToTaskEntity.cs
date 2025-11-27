using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Vivid.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorToTaskEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_VendorId",
                table: "Tasks",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Vendors_VendorId",
                table: "Tasks",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Vendors_VendorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_VendorId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Tasks");
        }
    }
}
