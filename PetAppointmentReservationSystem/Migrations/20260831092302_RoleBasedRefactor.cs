using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetAppointmentReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class RoleBasedRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PetName",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "StaffMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoPath",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PetId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "StaffMembers",
                keyColumn: "StaffId",
                keyValue: 1,
                columns: new[] { "Name", "UserId" },
                values: new object[] { "Dr. Alice Tan", null });

            migrationBuilder.UpdateData(
                table: "StaffMembers",
                keyColumn: "StaffId",
                keyValue: 2,
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "StaffMembers",
                keyColumn: "StaffId",
                keyValue: 3,
                column: "UserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PetId",
                table: "Appointments",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Pets_PetId",
                table: "Appointments",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Pets_PetId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PetId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StaffMembers");

            migrationBuilder.DropColumn(
                name: "PetId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoPath",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PetName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "StaffMembers",
                keyColumn: "StaffId",
                keyValue: 1,
                column: "Name",
                value: "Dr. Jacqueline Chong");
        }
    }
}
