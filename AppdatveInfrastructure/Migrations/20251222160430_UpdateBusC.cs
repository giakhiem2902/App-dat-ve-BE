using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppdatveInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBusC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Model",
                table: "Buses");

            migrationBuilder.RenameColumn(
                name: "Registration",
                table: "Buses",
                newName: "LicensePlate");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Buses",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "TotalSeats",
                table: "Buses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BusCompanies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "BusCompanies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "BusCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "BusCompanies",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSeats",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "BusCompanies");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "BusCompanies");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Buses",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "LicensePlate",
                table: "Buses",
                newName: "Registration");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Buses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "BusCompanies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUrl",
                table: "BusCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
