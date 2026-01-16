using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement.Migrations.AppDB
{
    /// <inheritdoc />
    public partial class RemoveDuplicateLocationUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationUser",
                schema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationUser",
                schema: "dbo",
                columns: table => new
                {
                    LocationsLocationid = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationUser", x => new { x.LocationsLocationid, x.UsersId });
                    table.ForeignKey(
                        name: "FK_LocationUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "security",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationUser_Location_LocationsLocationid",
                        column: x => x.LocationsLocationid,
                        principalSchema: "dbo",
                        principalTable: "Location",
                        principalColumn: "Locationid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationUser_UsersId",
                schema: "dbo",
                table: "LocationUser",
                column: "UsersId");
        }
    }
}
