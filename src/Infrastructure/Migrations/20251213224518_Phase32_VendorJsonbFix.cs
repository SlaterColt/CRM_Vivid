using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Vivid.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase32_VendorJsonbFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attributes",
                table: "Vendors",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attributes",
                table: "Vendors");
        }
    }
}
