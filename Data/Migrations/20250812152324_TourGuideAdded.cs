using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class TourGuideAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TourGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourGuides_Destinations_TourId",
                        column: x => x.TourId,
                        principalTable: "Destinations",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "51e4872b-ac22-438f-8f93-092d04619f01", "AQAAAAIAAYagAAAAEFnRnpTm5dNBvLUzcPJUqlPa/DJNdwxRFg8L3qXXc8B3cVjk0MaE0knWlDQG/GEiXA==", "a5a2a79b-a6bc-4845-ad35-4cc698a23938" });

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 12, 18, 23, 24, 30, DateTimeKind.Local).AddTicks(5220));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 12, 18, 23, 24, 30, DateTimeKind.Local).AddTicks(5285));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 12, 18, 23, 24, 30, DateTimeKind.Local).AddTicks(5288));

            migrationBuilder.CreateIndex(
                name: "IX_TourGuides_TourId",
                table: "TourGuides",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourGuides");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "df1c3a0f-1234-4cde-bb55-d5f15a6aabcd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "698d82a8-b2dc-4f5f-9217-b89377581f44", "AQAAAAIAAYagAAAAENG4PZ6ZnV+T2K/tLDBNDC3PJdGmWChPfFUE3YS1w8GiphFynQfWQsIquao3eCeXMA==", "e197de2a-33f2-43b4-bc44-762dda716913" });

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 12, 13, 28, 51, 639, DateTimeKind.Local).AddTicks(91));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 12, 13, 28, 51, 639, DateTimeKind.Local).AddTicks(174));

            migrationBuilder.UpdateData(
                table: "Destinations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 12, 13, 28, 51, 639, DateTimeKind.Local).AddTicks(177));
        }
    }
}
