using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAppointmentReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class AdminApprovalAndPetPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PetPhotos",
                columns: table => new
                {
                    PetPhotoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetPhotos", x => x.PetPhotoId);
                    table.ForeignKey(
                        name: "FK_PetPhotos_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "FullName", "IsApproved", "Password", "Role", "Username" },
                values: new object[] { 1, "admin@petconnect.local", "System Admin", true, "Admin@123", "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_PetPhotos_PetId",
                table: "PetPhotos",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetPhotos");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Users");
        }
    }
}
