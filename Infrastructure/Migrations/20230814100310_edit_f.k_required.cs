using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class edit_fk_required : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Drivers_DriverId",
                table: "Rentals");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("36041f05-5f49-4168-b151-650d7bbd36a1"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("c42f587e-f172-48ed-965e-a7428f2c9af7"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("c941d6c1-2621-4054-8eef-359dbddd16a8"));

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("92cc4a11-8d9c-4256-bc66-686ecf59cffa"));

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("a54cbc66-fabb-4293-b7a6-ba6ba61a4dd5"));

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("c186dc1b-8f57-4b9f-b9b8-9ece143299c3"));

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "Rentals",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("5801e7c9-2daa-4f91-9158-e2bbcda0bd0e"), "Customer3" },
                    { new Guid("7806670b-dac7-4051-97e2-61e0a043b8a6"), "Customer2" },
                    { new Guid("87ef817e-7604-42d4-b6a0-1fe7711a6f0e"), "Customer1" }
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "IsAvailable", "Name", "SubstituteId" },
                values: new object[,]
                {
                    { new Guid("20b4da7b-2dfb-4758-8d1d-b18cfaf9a5b7"), true, "driver1", null },
                    { new Guid("6ad5c888-778c-444d-9692-3768d0de7bb5"), true, "driver3", null },
                    { new Guid("ff11e306-41fe-4290-bffc-d8fc5fa05b84"), true, "driver2", null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Drivers_DriverId",
                table: "Rentals",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Drivers_DriverId",
                table: "Rentals");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("5801e7c9-2daa-4f91-9158-e2bbcda0bd0e"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("7806670b-dac7-4051-97e2-61e0a043b8a6"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("87ef817e-7604-42d4-b6a0-1fe7711a6f0e"));

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("20b4da7b-2dfb-4758-8d1d-b18cfaf9a5b7"));

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("6ad5c888-778c-444d-9692-3768d0de7bb5"));

            migrationBuilder.DeleteData(
                table: "Drivers",
                keyColumn: "Id",
                keyValue: new Guid("ff11e306-41fe-4290-bffc-d8fc5fa05b84"));

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "Rentals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("36041f05-5f49-4168-b151-650d7bbd36a1"), "Customer2" },
                    { new Guid("c42f587e-f172-48ed-965e-a7428f2c9af7"), "Customer3" },
                    { new Guid("c941d6c1-2621-4054-8eef-359dbddd16a8"), "Customer1" }
                });

            migrationBuilder.InsertData(
                table: "Drivers",
                columns: new[] { "Id", "IsAvailable", "Name", "SubstituteId" },
                values: new object[,]
                {
                    { new Guid("92cc4a11-8d9c-4256-bc66-686ecf59cffa"), false, "driver1", null },
                    { new Guid("a54cbc66-fabb-4293-b7a6-ba6ba61a4dd5"), false, "driver3", null },
                    { new Guid("c186dc1b-8f57-4b9f-b9b8-9ece143299c3"), false, "driver2", null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Drivers_DriverId",
                table: "Rentals",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
