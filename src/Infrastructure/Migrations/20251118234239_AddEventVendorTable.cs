using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Vivid.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventVendorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventVendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventVendors_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventVendors_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventVendors_EventId_VendorId",
                table: "EventVendors",
                columns: new[] { "EventId", "VendorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventVendors_VendorId",
                table: "EventVendors",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventVendors");
        }
    }
}
